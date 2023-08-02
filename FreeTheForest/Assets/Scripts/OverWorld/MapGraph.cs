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
        SpawnNodes(nodeLocations);
    }

    private List<Vector2> GenerateRandomNodeLocations(int numberOfNodes)
    {
        List<Vector2> randomNodeLocations = new List<Vector2>();

        // Define minimum and maximum spacing for the first three nodes
        float minFirstThreeNodeSpace = 200f;  // Minimum spacing
        float maxFirstThreeNodeSpace = 450f;  // Maximum spacing

        float[] firstThreeNodeSpaces = new float[3];

        // Calculate random space for the first three nodes
        for (int i = 0; i < 3; i++)
        {
            do
            {
                firstThreeNodeSpaces[i] = (i == 0 ? 0 : firstThreeNodeSpaces[i - 1]) + Random.Range(minFirstThreeNodeSpace, maxFirstThreeNodeSpace);
            }
            while (firstThreeNodeSpaces[i] > graphContainer.sizeDelta.x - 50f);

            Vector2 newNodePos = new Vector2(firstThreeNodeSpaces[i], 50f);  // Y position set to bottom
            randomNodeLocations.Add(newNodePos);
        }

        for (int i = 3; i < numberOfNodes - 1; i++)
        {
            Vector2 newNodePos;
            do
            {
                int randomXPos = Random.Range(50, (int)graphContainer.sizeDelta.x - 50);
                int randomYPos = Random.Range(50, (int)graphContainer.sizeDelta.y - 150);
                newNodePos = new Vector2(randomXPos, randomYPos);
            }
            while (CheckNodeViolation(randomNodeLocations, newNodePos));

            randomNodeLocations.Add(newNodePos);
        }

        // Ensure that one node is always the highest
        Vector2 finalNodePos;
        do
        {
            int finalNodeXPos = Random.Range(50, (int)graphContainer.sizeDelta.x - 50);
            finalNodePos = new Vector2(finalNodeXPos, (int)graphContainer.sizeDelta.y - 50);
        }
        while (CheckNodeViolation(randomNodeLocations, finalNodePos));

        randomNodeLocations.Add(finalNodePos);

        return randomNodeLocations;
    }

    private bool CheckNodeViolation(List<Vector2> existingNodes, Vector2 newNode)
    {
        foreach (var node in existingNodes)
        {
            //if the node is too close to another node, return true and try again.
            if (Mathf.Abs(node.x - newNode.x) < 200f && Mathf.Abs(node.y - newNode.y) < 200f)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnTile(Vector2 anchoredPos)
    {
        GameObject tile = Instantiate(tilePrefab, graphContainer);
        RectTransform tileTransform = tile.GetComponent<RectTransform>();
        Image tileImage = tile.GetComponent<Image>();
        // Set the size of the tile using the rectTransform
        tileTransform.sizeDelta = new Vector2(50, 50);
        tileTransform.anchoredPosition = anchoredPos;

        // Starting from the bottom left of graph
        tileTransform.anchorMin = new Vector2(0, 0);
        tileTransform.anchorMax = new Vector2(0, 0);
    }

    private void SpawnNodes(List<Vector2> nodeLocations)
    {
        for (int i = 0; i < nodeLocations.Count; i++)
        {
            SpawnTile(nodeLocations[i]);
        }
    }

    public void RegenerateNodes()
    {
        // Clear the current graph first
        foreach (Transform child in graphContainer)
        {
           if(child.name != "GraphContainerBackground")
             {
                Destroy(child.gameObject);
             }
        }
        // Regenerate nodes and spawn a new graph
        List<Vector2> nodeLocations = GenerateRandomNodeLocations(numberOfNodes);
        SpawnNodes(nodeLocations);
    }
}
