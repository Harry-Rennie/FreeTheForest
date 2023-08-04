using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class MapGraph : MonoBehaviour
{
    private RectTransform mGraph;
    [SerializeField] RectTransform graphContainer;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] int numberOfNodes = 10;
    private float graphHeight;
    private float graphWidth;

    [SerializeField] int gridSizeX = 8;
    [SerializeField] int gridSizeY = 7;
    [SerializeField] float cellWidth = 0.5f;
    [SerializeField] float cellHeight = 0.5f;
    //decide max encounters per level
    // populate a list of encounters, with appropriate spacing between nodes, maybe keep track of the 'floor' number (Row)
    // decide on which types of encounters we are going to have (boss, elite, normal mob, rest, reward etc.)
    // assign a probability of each encounter type spawning to determine the node type to spawn.
    // after the first row, we can have a random chance of spawning a node that is not a normal mob, but a rest, elite, boss, etc.
    // the last and highest Y position node will always be the final boss of the stage.

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
        SpawnNodes(nodeLocations);
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

        // Generate three nodes in the second row
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

        // generate nodes between the second row and top -1 (padding)
        for (int i = 3; i < numberOfNodes - 1; i++)
        {
            int gridX;
            int gridY;
            Vector2 newNodePos;

            do
            {
                gridX = Random.Range(1, gridSizeX - 1);
                gridY = Random.Range(2, gridSizeY - 1);

                // calculate the position of new node
                newNodePos = new Vector2(gridX * cellWidth, gridY * cellHeight);
            }
            while (CheckNodeViolation(randomNodeLocations, newNodePos));

            randomNodeLocations.Add(newNodePos);
        }

        // generate a node at the top (always boss)
        int topGridX;
        Vector2 topNodePos;

        do
        {
            topGridX = Random.Range(1, gridSizeX - 1);
            topNodePos = new Vector2(topGridX * cellWidth, (gridSizeY - 1) * cellHeight);
        }
        while (CheckNodeViolation(randomNodeLocations, topNodePos));

        randomNodeLocations.Add(topNodePos);

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

    private void SpawnNodes(List<Vector2> nodeLocations)
    {
        List<GameObject> nodes = new List<GameObject>();
        for (int i = 0; i < nodeLocations.Count; i++)
        {
            GameObject newNode = SpawnTile(nodeLocations[i]);
            nodes.Add(newNode);
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject nextHigherNode = FindNextHigherNode(nodes, i);

            if (nextHigherNode != null)
            {
                DrawLine(nodes[i], nextHigherNode);
            }
        }
    }

    private GameObject FindNextHigherNode(List<GameObject> nodes, int currentIndex)
    {
        float currentY = nodes[currentIndex].GetComponent<RectTransform>().anchoredPosition.y;
        GameObject nextHigherNode = null;

        for (int i = currentIndex + 1; i < nodes.Count; i++)
        {
            float testY = nodes[i].GetComponent<RectTransform>().anchoredPosition.y;
            if (testY > currentY)
            {
                currentY = testY;
                nextHigherNode = nodes[i];
                break;
            }
        }

        return nextHigherNode;
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
        SpawnNodes(nodeLocations);
    }

    private List<Vector2> SortNodes(List<Vector2> nodeLocations)
    {
        nodeLocations.Sort((node1, node2) =>
        {
            // compare y
            int yComparison = node1.y.CompareTo(node2.y);
            if (yComparison != 0)
            {
                // if y is different, order by y (ascending)
                return yComparison;
            }
            else
            {
                // if y-coordinates is the same, order by x
                return node1.x.CompareTo(node2.x);
            }
        });

        return nodeLocations;
    }

    private void DrawLine(GameObject tileA, GameObject tileB)
    {
        // get rect transforms of tiles to draw from
        RectTransform rectA = tileA.GetComponent<RectTransform>();
        RectTransform rectB = tileB.GetComponent<RectTransform>();

        // add line renderer component
        LineRenderer lr;
        if (tileA.GetComponent<LineRenderer>() != null)
        {
            lr = tileA.GetComponent<LineRenderer>();
        }
        else
        {
            lr = tileA.AddComponent<LineRenderer>();
        }

        // set number of points in LineRenderer
        lr.positionCount = 2;

        // set positions with rect transforms position, to get world space pos
        // A start, B end
        lr.SetPosition(0, rectA.position);
        lr.SetPosition(1, rectB.position);
        lr.sortingOrder = 1;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.black;
        lr.startWidth = lr.endWidth = 0.1f;
    }
}
