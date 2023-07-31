using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGraph : MonoBehaviour
{
    [SerializeField] RectTransform graphContainer;
    [SerializeField] private GameObject tilePrefab;
    //decide max encounters per level
    // populate a list of encounters, with appropriate spacing between nodes, maybe keep track of the 'floor' number (Row)
    // decide on which types of encounters we are going to have (boss, elite, normal mob, rest, reward etc.)
    // assign a probability of each encounter type spawning to determine the node type to spawn.
    // after the first row, we can have a random chance of spawning a node that is not a normal mob, but a rest, elite, boss, etc.
    // the last and highest Y position node will always be the final boss of the stage.
    private void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        List<int> nodeLocations = new List<int>() { 250, 950, 250, 1200, 1600, 600, 950, 250, 600};
        SpawnNodes(nodeLocations);
    }

    private void SpawnTile(Vector2 anchoredPos)
    {
        GameObject tile = Instantiate(tilePrefab, graphContainer);

        RectTransform tileTransform = tile.GetComponent<RectTransform>();
        // Get the Image component instead of SpriteRenderer
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
        //distance between each node on axis
        float xSpace = 150f;

        //define top height of graph
        float graphHeight = graphContainer.sizeDelta.y;
        float ySpace = 404.5f;

        for (int i = 0; i < nodeLocations.Count; i++)
        {
            float xPosition = i * xSpace;
            //offset first nodes
            if (i < 2)
            {
                xPosition = i * xSpace + 150f;
            }
            // normalization of graph height
            float yPosition = (nodeLocations[i] / ySpace) * (graphHeight + 150f);
            SpawnTile(new Vector2(xPosition, yPosition));
        }
    }
}