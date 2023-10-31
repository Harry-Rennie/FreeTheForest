using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapGraph : MonoBehaviour
{
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private LineManager lineManager;

    [SerializeField] private LineDrawer lineDrawer;
    [SerializeField] private GraphLayoutManager graphLayoutManager;

    //configuration of graph
    private RectTransform mGraph;
    private ScrollRect scrollRect;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private int numberOfNodes;
    [SerializeField] private int gridSizeX = 7;
    [SerializeField] private int gridSizeY = 7;
    private float graphHeight;
    private float graphWidth;
    private float cellHeight;
    private float cellWidth;
    private List<List<Vector2?>> nodeGrid = new List<List<Vector2?>>();
    private List<Vector2> nodeLocations;
    private List<GameObject> nodes;
    private PlayerInfoController gameManager;
    private Vector2 lastNodePos;
    private float scrollToPosition;
    private void Awake()
    {
        //initializing graph and dimensions
        mGraph = GetComponent<RectTransform>();
        graphHeight = mGraph.rect.height;
        graphWidth = mGraph.rect.width;
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        gameManager = FindObjectOfType<PlayerInfoController>();
    }

    private void Start()
    {
        //check if layout data is available (an existing list of serialized nodes)
        List<SerializableNode> layoutData = graphLayoutManager.LoadGraphLayout();
        if (layoutData.Count > 0)
        {
            SpawnFromSave(layoutData);
        }
        if (layoutData.Count == 0)
        {
            //no layout data - generate a new map
            nodeLocations = GenerateRandomNodeLocations(numberOfNodes);
            nodeLocations = NodeUtility.SortNodes(nodeLocations);
            GenerateNodeGrid(nodeLocations);
            nodes = SpawnNodes(nodeLocations, nodeGrid);
            lineManager.ConnectNodes(nodes);
            CheckRespawn(nodes);
            SaveData(nodes);
        }
        if(gameManager.floorNumber == 0 && gameManager.lastPosition != null)
        {
            //if you have no progress, enable first row of nodes.
            CheckProgress(nodes);
        }
        if(gameManager.lastPosition != null && gameManager.floorNumber > 0)
        {
            lastNodePos = gameManager.lastPosition;
            IncrementFloor(nodes);
            //move the map down by the amount of rows you have progressed.
            float scrollGridPositionChange = (graphHeight / gridSizeY) * gameManager.floorNumber + 50f;
            graphContainer.anchoredPosition = new Vector2(graphContainer.anchoredPosition.x, graphContainer.anchoredPosition.y - scrollGridPositionChange);
            lineManager.SnapLines(scrollGridPositionChange);
        }
    }

    /// <summary>
    /// Increments the floor number and updates the interactability accordingly.
    /// </summary>
    /// <param name="nodes"></param>
    private void IncrementFloor(List<GameObject> nodes)
    {
        List<Vector2> progressPositions = new List<Vector2>();
        foreach (GameObject node in nodes)
        {
            progressPositions.Add(node.GetComponent<RectTransform>().anchoredPosition);
        }
        List<List<Vector2>> nodeCheck = GenerateNodeGrid(progressPositions);
        for (int row = 0; row < nodeCheck.Count; row++)
        {
            for (int col = 0; col < nodeCheck[row].Count; col++)
            {
                Vector2 nodeLocation = nodeCheck[row][col];
                if (nodeLocation.x != float.NegativeInfinity && nodeLocation.y != float.NegativeInfinity)
                {
                    foreach(GameObject node in nodes)
                    {
                        if(node.GetComponent<RectTransform>().anchoredPosition == nodeLocation)
                        {
                            if(row <= gameManager.floorNumber)
                            {
                                //this ensures every node below the floor you are currently on is disabled.
                                node.GetComponent<Button>().interactable = false;
                            }
                            if(row >= gameManager.floorNumber && lastNodePos != null)
                            {
                                //using utility from line manager to determine which nodes are above the last node visited, and connected via a line.
                                GameObject lastNode = nodes.Find(x => x.GetComponent<RectTransform>().anchoredPosition == lastNodePos);
                                List<GameObject> upperNodes = lineManager.GetUpperNodes(lastNode, nodes);
                                foreach(GameObject parentNode in upperNodes)
                                {
                                    if(lineDrawer.HasLineBetween(lastNode, parentNode) || lineDrawer.HasLineBetween(parentNode, lastNode))
                                    {
                                        parentNode.GetComponent<Button>().interactable = true;
                                        scrollToPosition = parentNode.GetComponent<RectTransform>().anchoredPosition.y;
                                    }
                                }
                            }

                        }
                            
                    }
                }
            }
        }
    }
    /// <summary>
    /// Initializes first row of nodes to be interactable for start of game.
    /// </summary>
    /// <param name="nodes"></param>
    private void CheckProgress(List<GameObject> nodes)
    {   
        List<Vector2> progressPositions = new List<Vector2>();
        foreach (GameObject node in nodes)
        {
            progressPositions.Add(node.GetComponent<RectTransform>().anchoredPosition);
        }
        List<List<Vector2>> nodeCheck = GenerateNodeGrid(progressPositions);
        for (int row = 0; row < nodeCheck.Count; row++)
        {
            for (int col = 0; col < nodeCheck[row].Count; col++)
            {
                Vector2 nodeLocation = nodeCheck[row][col];
                if (nodeLocation.x != float.NegativeInfinity && nodeLocation.y != float.NegativeInfinity)
                {
                    foreach(GameObject node in nodes)
                    {
                        if(node.GetComponent<RectTransform>().anchoredPosition == nodeLocation)
                        {
                            if(row <= 1)
                            {
                                node.GetComponent<Button>().interactable = true;
                            }
                        }
                    }
                }
            }
        }
    }

    //parses the data we need to recreate the map from the list of nodes.
    //may need to serialize node grid as well.
    private void SaveData(List<GameObject> nodes)
    {
        List<SerializableNode> serializedNodes = new List<SerializableNode>();
        foreach (GameObject node in nodes)
        {
            Vector2 nodePosition = node.GetComponent<RectTransform>().anchoredPosition;
            string prefabName = node.name;
            SerializableNode serializedNode = new SerializableNode(nodePosition, prefabName);
            List<GameObject> lines = lineManager.GetLinesFromNode(node);
            //probably will remove line saving, might be useful for now.
            foreach (GameObject line in lines)
            {
                Vector2 startPoint = line.GetComponent<LineRenderer>().GetPosition(0);
                Vector2 endPoint = line.GetComponent<LineRenderer>().GetPosition(1);
                SerializableLine serializedLine = new SerializableLine(startPoint, endPoint);
                serializedNode.associatedLines.Add(serializedLine);
            }
            serializedNodes.Add(serializedNode);
        }
        graphLayoutManager.SaveGraphLayout(serializedNodes);
    }

    ///<summary>
    /// Spawn nodes based on the loaded layout data.
    ///</summary>
    //// <param name="layoutData">A custom serialized list of node objects with relevant data necessary to recreate a previous session from file.</param>
    private void SpawnFromSave(List<SerializableNode> layoutData)
    {
        List<GameObject> respawnNodes = new List<GameObject> ();

        foreach (SerializableNode serializedNode in layoutData)
        {
            //spawn nodes based on the loaded layout
            Vector2 nodePosition = serializedNode.position.ToVector2();
            GameObject prefab = nodeManager.ParsePrefabName(serializedNode.prefabName);
            GameObject spawnedNode = nodeManager.SpawnSpecificNode(prefab, nodePosition, graphContainer);
            respawnNodes.Add(spawnedNode);
        }
        //instead of connecting the lines from save, currently just reconnecting as the logic is the same - have tested.
        lineManager.ConnectNodes(respawnNodes);
        nodes = respawnNodes;
    }

    ///<summary>
    /// Generate random positions for nodes based on grid dimensions.
    ///</summary>
    //// <param name="numberOfNodes">The amount of nodes to be spawned on the map.</param>
    private List<Vector2> GenerateRandomNodeLocations(int numberOfNodes)
    {
        //generate random positions for nodes based on grid dimensions
        List<Vector2> randomNodeLocations = new List<Vector2>();

        float cellWidth = graphContainer.sizeDelta.x / gridSizeX;
        float cellHeight = graphContainer.sizeDelta.y / gridSizeY;

        //position for top node (boss)
        Vector2 topNodePos = new Vector2(gridSizeX * 0.5f * cellWidth, (gridSizeY - 1) * cellHeight);
        randomNodeLocations.Add(topNodePos);
        numberOfNodes--;

        //create 3 random positions in second row (starting row)
        for (int i = 0; i < 3; i++)
        {
            int gridX;
            Vector2 newNodePos;

            do
            {
                gridX = UnityEngine.Random.Range(1, gridSizeX - 1);
                newNodePos = new Vector2(gridX * cellWidth, 1 * cellHeight);
            }
            while (NodeUtility.CheckNodeViolation(randomNodeLocations, newNodePos));

            randomNodeLocations.Add(newNodePos);
        }

        //generate rest of nodes
        numberOfNodes -= 3;

        for (int i = 0; i < numberOfNodes; i++)
        {
            int gridX, gridY;
            Vector2 newNodePos;

            do
            {
                gridX = UnityEngine.Random.Range(1, gridSizeX - 1);
                gridY = UnityEngine.Random.Range(2, gridSizeY - 1);
                newNodePos = new Vector2(gridX * cellWidth, gridY * cellHeight);
            }
            while (NodeUtility.CheckNodeViolation(randomNodeLocations, newNodePos));

            randomNodeLocations.Add(newNodePos);
        }

        return randomNodeLocations;
    }

    ///<summary>
    ////Spawn nodes based locations and type inside of the grid.
    ///</summary>
    //// <param name="nodeLocations">Semi-random list of vectors used to determine where nodes will spawn.</param>
    //// <param name="nodeGrid">2D list that keeps track of which cell contains a node (or doesn't).</param>
    private List<GameObject> SpawnNodes(List<Vector2> nodeLocations, List<List<Vector2?>> nodeGrid)
    {
        
        List<GameObject> nodes = new List<GameObject>();
        nodes.Add(nodeManager.SpawnBossNode(nodeLocations[0], graphContainer));
        for (int i = 1; i < nodeLocations.Count - 3; i++)
        {
            var (row, col) = GetNodeGridPosition(nodeLocations[i], nodeGrid);
            nodes.Add(nodeManager.SpawnRandomNode(nodeLocations[i], graphContainer, nodeLocations, row, col, nodeGrid));
        }

        for (int i = nodeLocations.Count - 3; i < nodeLocations.Count; i++)
        {
            nodes.Add(nodeManager.SpawnBasicNode(nodeLocations[i], graphContainer));
        }

        return nodes;
    }

    ///<summary>
    ////Spawns a new graph of nodes.
    ///</summary>
    private void GenerateGraph()
    {
        //generate nodes and connect them
        List<Vector2> nodeLocations = GenerateRandomNodeLocations(numberOfNodes);
        nodeLocations = NodeUtility.SortNodes(nodeLocations);
        GenerateNodeGrid(nodeLocations);
        List<GameObject> nodes = SpawnNodes(nodeLocations, nodeGrid);
        lineManager.ConnectNodes(nodes);
        CheckRespawn(nodes);
        SaveData(nodes);
        if(gameManager.floorNumber == 0)
        {
            CheckProgress(nodes);
        }
    }

    /// <summary>
    ////utility to grab correct prefab from string
    /// </summary>
    public List<string> GetPrefabNamesFromMap()
    {
        List<string> prefabNames = new List<string>();

        foreach (KeyValuePair<GameObject, GameObject> entry in lineManager.nodeParentMap)
        {
            GameObject childNode = entry.Key;
            GameObject parentNode = entry.Value;
            string prefabName = childNode.name;
            prefabNames.Add(prefabName);
        }

        return prefabNames;
    }

    /// <summary>
    ////compares tags of parent child, respawns if they match condition, updates dictionary and list of nodes.
    /// </summary>
    private void CheckRespawn(List<GameObject> nodes)
    {
        bool hasDuplicate;
        List<GameObject> updatedNodes = new List<GameObject>(); //store updated nodes

        do
        {
            hasDuplicate = false;
            List<GameObject> keysToUpdate = new List<GameObject>();
            Dictionary<GameObject, GameObject> newEntries = new Dictionary<GameObject, GameObject>();

            foreach (KeyValuePair<GameObject, GameObject> entry in new Dictionary<GameObject, GameObject>(lineManager.nodeParentMap))
            {
                GameObject childNode = entry.Key;
                GameObject parentNode = entry.Value;
                string tag = childNode.tag;

                if (childNode.name == parentNode.name && (tag == "Heal" || tag == "Upgrade"))
                {
                    hasDuplicate = true; //keep checking
                    GameObject newChildNode = nodeManager.Respawn(childNode, graphContainer, tag);
                    keysToUpdate.Add(childNode);
                    newEntries[newChildNode] = parentNode;
                    Destroy(childNode);
                }
            }
            //update the dictionary
            foreach (var key in keysToUpdate)
            {
                lineManager.nodeParentMap.Remove(key);
            }
            foreach (var entry in newEntries)
            {
                lineManager.nodeParentMap[entry.Key] = entry.Value;
            }
        }
        while (hasDuplicate);
        //updating dependencies and saving
        updatedNodes.AddRange(nodes);
        foreach (var entry in lineManager.nodeParentMap)
        {
            updatedNodes.Add(entry.Key);
        }
        nodes.Clear();
        nodes.AddRange(updatedNodes);        
    }

    /// <summary>
    /// Generates grid of nodes based on node locations.
    /// </summary>
    //// <param name="nodeLocations">Semi-random list of vectors used to determine where nodes will spawn.</param>
private List<List<Vector2>> GenerateNodeGrid(List<Vector2> nodeLocations)
{
    List<List<Vector2>> nodeGrid = new List<List<Vector2>>();

    for (int y = 0; y < gridSizeY; y++)
    {
        List<Vector2> newRow = new List<Vector2>();
        int nodesInRow = 0; //initialize the count of nodes in this row
        for (int x = 0; x < gridSizeX; x++)
        {
            newRow.Add(Vector2.negativeInfinity);
        }

        foreach (var location in nodeLocations)
        {
            int row = Mathf.FloorToInt(location.y / (graphHeight / gridSizeY));
            int col = Mathf.FloorToInt(location.x / (graphWidth / gridSizeX));

            //check if the location is within this row
            if (row == y)
            {
                newRow[col] = location;
                nodesInRow++; //increment the count of nodes in this row
            }
        }

        nodeGrid.Add(newRow);
    }

    return nodeGrid;
}

    

    /// <summary>
    /// Utility function to get the grid position of a node.
    /// </summary>
    //// <param name="nodeLocation"></param>
    //// <param name="nodeGrid"></param>
    public (int row, int col) GetNodeGridPosition(Vector2 nodeLocation, List<List<Vector2?>> nodeGrid)
    {
        //loop through each row in the grid
        for (int row = 0; row < nodeGrid.Count; row++)
        {
            //loop through each column in the row
            for (int col = 0; col < nodeGrid[row].Count; col++)
            {
                //check if the grid cell is null before comparing
                if (nodeGrid[row][col].HasValue)
                {
                    //check if the node location matches the grid cell value
                    if (Vector2.Distance(nodeGrid[row][col].Value, nodeLocation) < 1f)
                    {
                        return (row, col);  //return co-ords
                    }
                }
            }
        }

        return (-1, -1);  //if not found
    }

    /// <summary>
    /// A Cleanup function before regenerating the graph.
    /// </summary>
    public void RegenerateNodes()
    {
        //delete the current layout data from GraphLayoutManager to generate a new random layout next time (changing map)
        graphLayoutManager.DeleteGraphLayout();

        foreach (Transform child in graphContainer)
        {
            if (child.name != "GraphContainerBackground")
            {
                Destroy(child.gameObject);
            }
        }
        nodeManager.CleanNodes();
        GenerateGraph();
    }
}