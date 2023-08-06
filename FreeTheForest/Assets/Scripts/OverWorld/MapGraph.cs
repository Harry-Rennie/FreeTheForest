using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapGraph : MonoBehaviour
{
    //Todo: refactor more of this class into LineDrawer.cs
    private RectTransform mGraph;
    [SerializeField] RectTransform graphContainer;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] int numberOfNodes;
    [SerializeField] private LineDrawer lineDrawer;
    private float graphHeight;
    private float graphWidth;
    [SerializeField] int gridSizeX = 8;
    [SerializeField] int gridSizeY = 7;

    private void Awake()
    {
        mGraph = GetComponent<RectTransform>();
        graphHeight = mGraph.rect.height;
        graphWidth = mGraph.rect.width;
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
    }
    private void Start()
    {
        List<Vector2> nodeLocations = GenerateRandomNodeLocations(numberOfNodes);
        nodeLocations = SortNodes(nodeLocations);
        List<GameObject> nodes = SpawnNodes(nodeLocations);
        ConnectNodes(nodes);
        List<List<GameObject>> groups = GroupConnectedNodes(nodes);
        ConnectGroups(groups);
    }

    private List<Vector2> GenerateRandomNodeLocations(int numberOfNodes)
    {
        List<Vector2> randomNodeLocations = new List<Vector2>();

        // calculate the size of each cell in the grid
        float cellWidth = graphContainer.sizeDelta.x / gridSizeX;
        float cellHeight = graphContainer.sizeDelta.y / gridSizeY;

        // ensure at least one node at top and three at bottom
        if (numberOfNodes < 5)
        {
            return randomNodeLocations;
        }

        //top node always centered
        Vector2 topNodePos = new Vector2(gridSizeX * 0.5f * cellWidth, (gridSizeY - 1) * cellHeight);
        randomNodeLocations.Add(topNodePos);
        numberOfNodes--;

        //generate three nodes in the second row from the bottom
        for (int i = 0; i < 3; i++)
        {
            int gridX;
            Vector2 newNodePos;

            do
            {
                gridX = Random.Range(1, gridSizeX - 1);
                newNodePos = new Vector2(gridX * cellWidth, 1 * cellHeight);
            }
            while (CheckNodeViolation(randomNodeLocations, newNodePos));

            randomNodeLocations.Add(newNodePos);
        }

        // adjust to account for already added nodes
        numberOfNodes -= 3;

        //generate remaining nodes
        for (int i = 0; i < numberOfNodes; i++)
        {
            int gridX, gridY;
            Vector2 newNodePos;

            do
            {
                gridX = Random.Range(1, gridSizeX - 1);
                gridY = Random.Range(2, gridSizeY - 1);  //excluding top and second row

                newNodePos = new Vector2(gridX * cellWidth, gridY * cellHeight);
            }
            while (CheckNodeViolation(randomNodeLocations, newNodePos));

            randomNodeLocations.Add(newNodePos);
        }

        return randomNodeLocations;
    }

    private bool CheckNodeViolation(List<Vector2> existingNodes, Vector2 newNode)
    {
        foreach (var node in existingNodes)
        {
            //if the position of the node is already occupied, return true and try again.
            if (node.x == newNode.x && node.y == newNode.y)
            {
                return true;
            }
        }
        return false;
    }

    //Instantiates tile with properties, anchored to 0,0 of graphContainer
    private GameObject SpawnTile(Vector2 anchoredPos)
    {
        GameObject tile = Instantiate(tilePrefab, graphContainer);
        RectTransform tileTransform = tile.GetComponent<RectTransform>();
        Image tileImage = tile.GetComponent<Image>();
        tileTransform.sizeDelta = new Vector2(50, 50);
        tileTransform.anchoredPosition = anchoredPos;
        tileTransform.anchorMin = new Vector2(0, 0);
        tileTransform.anchorMax = new Vector2(0, 0);
        return tile;
    }

    private List<GameObject> SpawnNodes(List<Vector2> nodeLocations)
    {
        List<GameObject> nodes = new List<GameObject>();
        for (int i = 0; i < nodeLocations.Count; i++)
        {
            GameObject newNode = SpawnTile(nodeLocations[i]);
            nodes.Add(newNode);
        }
        return nodes;
    }
    private void ConnectNodes(List<GameObject> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject node = nodes[i];
            Vector2 nodePosition = node.GetComponent<RectTransform>().anchoredPosition;

            //get nodes in row below the current node
            List<GameObject> nodesInNextRow = GetNodesInRow(nodes, nodePosition.y);

            //check if there are nodes in the row below
            if (nodesInNextRow.Count > 0)
            {
                //find the closest node in the row below and draw a line
                GameObject closestNode = GetClosestNode(nodePosition, nodesInNextRow);
                DrawLineBetweenNodes(node, closestNode);
            }
        }
        GroupConnectedNodes(nodes);
    }

    private List<GameObject> GetNodesInRow(List<GameObject> nodes, float currentY)
    {
        //dont do an exact comparison on two floats
        float rowHeightTolerance = 0.1f;
        //for each node get absolute value of node in row and return as list
        return nodes.Where(n => Mathf.Abs(n.GetComponent<RectTransform>().anchoredPosition.y - currentY) > rowHeightTolerance).ToList();
    }

    //sorting to find closest node
    private GameObject GetClosestNode(Vector2 nodePosition, List<GameObject> nodes)
    {
        GameObject closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (var node in nodes)
        {
            Vector2 thisNodePosition = node.GetComponent<RectTransform>().anchoredPosition;
            float distance = Vector2.Distance(nodePosition, thisNodePosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }
    private void DrawLineBetweenNodes(GameObject nodeA, GameObject nodeB)
    {
        lineDrawer.DrawLine(nodeA, nodeB);
    }
    //functions to perform depth first search on nodes with lines drawn between them, appending them to lists of groups of lists of nodes.
    private List<List<GameObject>> GroupConnectedNodes(List<GameObject> nodes)
    {
        HashSet<GameObject> visitedNodes = new HashSet<GameObject>(); //this keeps track of the nodes that the depth first search finds.
        List<List<GameObject>> groups = new List<List<GameObject>>();

        foreach (var node in nodes)
        {
            if (!visitedNodes.Contains(node))
            {
                List<GameObject> group = new List<GameObject>();
                DFS(node, group, nodes, visitedNodes);
                groups.Add(group);
            }
        }

        //logging information about the groups - remove when finished.
        Debug.Log($"Total number of groups: {groups.Count}");
        for (int i = 0; i < groups.Count; i++)
        {
            Debug.Log($"Group {i + 1} has {groups[i].Count} nodes.");
        }

        return groups;
    }

    private void DFS(GameObject currentNode, List<GameObject> group, List<GameObject> nodes, HashSet<GameObject> visitedNodes)
    {
        if (visitedNodes.Contains(currentNode))
            return;

        visitedNodes.Add(currentNode);
        group.Add(currentNode);

        //get neighboring nodes (directly connected nodes)
        List<GameObject> neighbors = GetConnectedNeighbors(currentNode, nodes);

        foreach (var neighbor in neighbors)
        {
            DFS(neighbor, group, nodes, visitedNodes);
        }
    }

    private List<GameObject> GetConnectedNeighbors(GameObject node, List<GameObject> nodes)
    {
        List<GameObject> neighbors = new List<GameObject>();
        Vector2 nodePosition = node.GetComponent<RectTransform>().anchoredPosition;

        //checks nodes in the same row and nodes below for potential neighbours
        List<GameObject> possibleNeighbors = GetNodesInRow(nodes, nodePosition.y);
        possibleNeighbors.AddRange(GetNodesInRow(nodes, nodePosition.y - 1));  // -1 row

        foreach (var neighbor in possibleNeighbors)
        {
            if (IsDirectlyConnected(node, neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    //using node locations to check if line renderer intersects with a node
    private bool IsDirectlyConnected(GameObject node1, GameObject node2)
    {
        //get all LineRenderers in the scene
        LineRenderer[] lineRenderers = GameObject.FindObjectsOfType<LineRenderer>();

        foreach (LineRenderer lr in lineRenderers)
        {
            Vector3 startPoint = lr.GetPosition(0);
            Vector3 endPoint = lr.GetPosition(1);

            //check if either point of the line is within bounds of node1 or node2
            if (IsPointInsideNodeBounds(startPoint, node1) && IsPointInsideNodeBounds(endPoint, node2) ||
                IsPointInsideNodeBounds(startPoint, node2) && IsPointInsideNodeBounds(endPoint, node1))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointInsideNodeBounds(Vector3 point, GameObject node)
    {
        RectTransform rt = node.GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        bool insideX = point.x >= corners[0].x && point.x <= corners[2].x;
        bool insideY = point.y >= corners[0].y && point.y <= corners[2].y;

        return insideX && insideY;
    }

    private void ConnectGroups(List<List<GameObject>> groups)
    {
        //todo
    }

    public void RegenerateNodes()
    {
        // Clear the current graph first
        foreach (Transform child in graphContainer)
        {
            if (child.name != "GraphContainerBackground")
            {
                Destroy(child.gameObject);
            }
        }
        // Regenerate nodes and spawn a new graph
        List<Vector2> nodeLocations = GenerateRandomNodeLocations(numberOfNodes);
        nodeLocations = SortNodes(nodeLocations);
        List<GameObject> nodes = SpawnNodes(nodeLocations);
        ConnectNodes(nodes);
        List<List<GameObject>> groups = GroupConnectedNodes(nodes);
        ConnectGroups(groups);
    }

    //sorts from top to bottom
    private List<Vector2> SortNodes(List<Vector2> nodeLocations)
    {
        nodeLocations.Sort((node1, node2) =>
        {
            // compare y
            int yComparison = node2.y.CompareTo(node1.y);
            if (yComparison != 0)
            {
                return yComparison;
            }
            else
            {
                return node1.x.CompareTo(node2.x);
            }
        });

        return nodeLocations;
    }
}