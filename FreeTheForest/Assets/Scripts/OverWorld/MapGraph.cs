using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGraph : MonoBehaviour
{
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private LineManager lineManager;
    [SerializeField] private GraphLayoutManager graphLayoutManager;

    //configuration of graph
    private RectTransform mGraph;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private int numberOfNodes;
    [SerializeField] private int gridSizeX = 8;
    [SerializeField] private int gridSizeY = 7;
    private float graphHeight;
    private float graphWidth;
    private List<List<Vector2?>> nodeGrid = new List<List<Vector2?>>();
    private List<Vector2> nodeLocations;
    private void Awake()
    {
        //initializing graph and dimensions
        mGraph = GetComponent<RectTransform>();
        graphHeight = mGraph.rect.height;
        graphWidth = mGraph.rect.width;
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
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
            List<GameObject> nodes = SpawnNodes(nodeLocations, nodeGrid);
            lineManager.ConnectNodes(nodes);
            CheckRespawn(nodes);
        }
    }

    //parses the data we need to recreate the map from the list of nodes.
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
    }

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

    private List<GameObject> SpawnNodes(List<Vector2> nodeLocations, List<List<Vector2?>> nodeGrid)
    {
        //spawn nodes based locations and type
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

    private void GenerateGraph()
    {
        //generate nodes and connect them
        List<Vector2> nodeLocations = GenerateRandomNodeLocations(numberOfNodes);
        nodeLocations = NodeUtility.SortNodes(nodeLocations);
        GenerateNodeGrid(nodeLocations);
        List<GameObject> nodes = SpawnNodes(nodeLocations, nodeGrid);
        lineManager.ConnectNodes(nodes);
        CheckRespawn(nodes);
    }

    //utility to grab correct prefab from string
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

    //compares tags of parent child, respawns if they match condition, updates dictionary and list of nodes.
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
        SaveData(nodes);
    }

    private List<List<Vector2?>> GenerateNodeGrid(List<Vector2> nodeLocations)
    {
        List<List<Vector2?>> nodeGrid = new List<List<Vector2?>>();

        for (int y = 0; y < gridSizeY; y++)
        {
            List<Vector2?> newRow = new List<Vector2?>();
            for (int x = 0; x < gridSizeX; x++)
            {
                newRow.Add(null);
            }
            nodeGrid.Add(newRow);
        }

        foreach (var location in nodeLocations)
        {
            int row = Mathf.FloorToInt(location.y / (graphHeight / gridSizeY));
            int col = Mathf.FloorToInt(location.x / (graphWidth / gridSizeX));
            nodeGrid[row][col] = location;
        }

        // Mark empty cells based on node locations and dimensions
        foreach (var location in nodeLocations)
        {
            int row = Mathf.FloorToInt(location.y / (graphHeight / gridSizeY));
            int col = Mathf.FloorToInt(location.x / (graphWidth / gridSizeX));

            // Check if the cell above is empty and mark it
            if (row > 0 && !nodeGrid[row - 1][col].HasValue)
            {
                nodeGrid[row - 1][col] = Vector2.negativeInfinity;
            }
        }
        return nodeGrid;
    }
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
                    if (Vector2.Distance(nodeGrid[row][col].Value, nodeLocation) < 0.1f)
                    {
                        return (row, col);  //return co-ords
                    }
                }
            }
        }

        return (-1, -1);  //if not found
    }
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