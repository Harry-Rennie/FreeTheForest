using System;
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
                gridX = UnityEngine.Random.Range(1, gridSizeX - 1);
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
                gridX = UnityEngine.Random.Range(1, gridSizeX - 1);
                gridY = UnityEngine.Random.Range(2, gridSizeY - 1);  //excluding top and second row
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

        nodes = nodes.OrderByDescending(n => n.GetComponent<RectTransform>().anchoredPosition.y)
                    .ThenBy(n => n.GetComponent<RectTransform>().anchoredPosition.x)
                    .ToList();

        return nodes;
    }

    private void ConnectNodes(List<GameObject> nodes)
    {
        //initial connections from each node to the closest node below it
        List<GameObject> sortedNodes = nodes.OrderBy(n => n.GetComponent<RectTransform>().anchoredPosition.y).ToList();

        for (int i = 0; i < sortedNodes.Count; i++)
        {
            GameObject currentNode = sortedNodes[i];

            //get all nodes in the rows below the current node
            List<GameObject> lowerNodes = sortedNodes.Where(n => n.GetComponent<RectTransform>().anchoredPosition.y < currentNode.GetComponent<RectTransform>().anchoredPosition.y).ToList();

            //if there are no nodes below the current node, move to the next node
            if (!lowerNodes.Any())
            {
                continue;
            }
            //find the closest node in the rows below and draw line between them.
            GameObject closestNode = lowerNodes.OrderBy(n => Vector2.Distance(n.GetComponent<RectTransform>().anchoredPosition, currentNode.GetComponent<RectTransform>().anchoredPosition)).First();
            DrawLineBetweenNodes(currentNode, closestNode);
        }
        //loop through all nodes and ensure each node has at least one connection above it
        foreach (GameObject node in sortedNodes)
        {
            List<GameObject> linesFromNode = GetLinesFromNode(node);

            //check if there are lines going to a node above the current node
            bool hasConnectionAbove = linesFromNode.Any(line => line.GetComponent<LineRenderer>().GetPosition(1).y > node.GetComponent<RectTransform>().anchoredPosition.y);
            //if there are no connections above - connect the node to the closest node above it
            if (!hasConnectionAbove)
            {
                //get all nodes above the current node
                List<GameObject> upperNodes = sortedNodes.Where(n => n.GetComponent<RectTransform>().anchoredPosition.y > node.GetComponent<RectTransform>().anchoredPosition.y).ToList();
                //if there are no nodes above skip to the next node
                if (!upperNodes.Any())
                {
                    continue;
                }
                //find the closest node among all nodes above
                GameObject closestNode = upperNodes.OrderBy(n => Vector2.Distance(n.GetComponent<RectTransform>().anchoredPosition, node.GetComponent<RectTransform>().anchoredPosition)).First();
                //connect nodes
                DrawLineBetweenNodes(node, closestNode);
            }
        }
    }

    public List<GameObject> GetLinesFromNode(GameObject node)
    {
        List<GameObject> lines = new List<GameObject>();
        foreach (Transform child in node.transform)
        {
            if (child.name.StartsWith("Line"))
            {
                lines.Add(child.gameObject);
            }
        }
        return lines;
    }

    private void DrawLineBetweenNodes(GameObject nodeA, GameObject nodeB)
    {
        lineDrawer.DrawLine(nodeA, nodeB);
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