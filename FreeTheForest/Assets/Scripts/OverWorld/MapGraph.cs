using System.Collections;
using System.Collections.Generic;
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
    //decide max encounters per level
    // populate a list of encounters, with appropriate spacing between nodes, maybe keep track of the 'floor' number (Row)
    // decide on which types of encounters we are going to have (boss, elite, normal mob, rest, reward etc.)
    // assign a probability of each encounter type spawning to determine the node type to spawn.
    // after the first row, we can have a random chance of spawning a node that is not a normal mob, but a rest, elite, boss, etc.
    // the last and highest Y position node will always be the final boss of the stage.
    private void Start()
    {
        mGraph = GetComponent<RectTransform>();
        //setting graphContainer to correct dimension and position of parent object
        graphHeight = mGraph.sizeDelta.y;
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        graphContainer.sizeDelta = new Vector2(graphContainer.sizeDelta.x, graphHeight);
        List<int> nodeLocations = GenerateRandomNodeLocations(numberOfNodes);
        SpawnNodes(nodeLocations);
    }

    private List<int> GenerateRandomNodeLocations(int numberOfNodes)
    {
        List<int> randomNodeLocations = new List<int>();

        for (int i = 0; i < numberOfNodes; i++)
        {
            // Make sure the randomYPos is within the graphContainer's height
            int randomYPos = Random.Range(50, (int)graphContainer.sizeDelta.y - 50);
            randomNodeLocations.Add(randomYPos);
        }

        return randomNodeLocations;
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

    private void SpawnNodes(List<int> nodeLocations)
    {
        float xSpace = graphContainer.sizeDelta.x / (nodeLocations.Count - 1);

        for (int i = 0; i < nodeLocations.Count; i++)
        {
            // Calculate positions
            float xPosition = i * xSpace;
            float normalizedYPos = (float)nodeLocations[i] / graphContainer.sizeDelta.y;
            // If it's one of the first 3 nodes, set y position to the bottom margin, otherwise normalize between 0 and 1
            float yPosition = i < 3 ? 50f : normalizedYPos * graphHeight;

            SpawnTile(new Vector2(xPosition, yPosition));
        }
    }
}
