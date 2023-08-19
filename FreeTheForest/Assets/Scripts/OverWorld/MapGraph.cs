using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapGraph : MonoBehaviour
{
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private LineManager lineManager;

    //configuration of graph
    private RectTransform mGraph;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private int numberOfNodes;
    [SerializeField] private int gridSizeX = 8;
    [SerializeField] private int gridSizeY = 7;
    private float graphHeight;
    private float graphWidth;

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
        GenerateGraph();
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

    private List<GameObject> SpawnNodes(List<Vector2> nodeLocations)
    {
        //spawn nodes based locations and type
        List<GameObject> nodes = new List<GameObject>();
        nodes.Add(nodeManager.SpawnBossNode(nodeLocations[0], graphContainer));

        for (int i = 1; i < nodeLocations.Count - 3; i++)
        {
            nodes.Add(nodeManager.SpawnRandomNode(nodeLocations[i], graphContainer, nodes));
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
        List<GameObject> nodes = SpawnNodes(nodeLocations);
        lineManager.ConnectNodes(nodes);
    }

    public void RegenerateNodes()
    {
        //regenerate nodes in graph
        foreach (Transform child in graphContainer)
        {
            if (child.name != "GraphContainerBackground")
            {
                Destroy(child.gameObject);
            }
        }
        GenerateGraph();
    }
}