using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LineManager : MonoBehaviour
{
    public Dictionary<GameObject, GameObject> nodeParentMap = new Dictionary<GameObject, GameObject>();
    [SerializeField] private LineDrawer lineDrawer;
    private ScrollRect scrollRect;
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] public float scrollPositionSensitivity; // Adjust this factor as needed        
    public Vector2 previousScrollPosition;
    private float scrollPositionChange;
    private float initialGraphY;
    //connect given nodes based on their position

    public void Start()
    {
        initialGraphY = graphContainer.anchoredPosition.y;
        scrollRect = graphContainer.transform.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            previousScrollPosition = scrollRect.normalizedPosition;
        }
    }

public void Update()
{
    float graphContainerPositionY = graphContainer.anchoredPosition.y;
    // Calculate the change in graphContainerPositionY relative to the initial position
    float scrollPositionChange = (graphContainerPositionY - initialGraphY) * scrollPositionSensitivity;
        AdjustLines(scrollPositionChange);

        // Update the initial position
        initialGraphY = graphContainerPositionY;
}

    public void OnScrollValueChanged(Vector2 value)
    {
        previousScrollPosition = value;
    }

    public void AdjustLines(float scrollPositionChange)
{
    // Get all line objects from graph container, they are children of objects inside of graph container
    List<GameObject> lines = graphContainer.transform.Cast<Transform>()
        .SelectMany(child => child.Cast<Transform>())
        .Where(child => child.name.StartsWith("Line"))
        .Select(child => child.gameObject)
        .ToList();
    Vector3 offset = new Vector3(0f, scrollPositionChange, 0f);
    // Offset each line by the scroll position change
    foreach (GameObject line in lines)
    {
        LineRenderer lr = line.GetComponent<LineRenderer>();
        for (int i = 0; i < lr.positionCount; i++)
        {
            Vector3 position = lr.GetPosition(i);
            lr.SetPosition(i, position + offset);
        }
    }
}
public void SnapLines(float scrollLinePositionChange)
{
    // Get all line objects from the graph container
    List<GameObject> lines = graphContainer.transform.Cast<Transform>()
        .SelectMany(child => child.Cast<Transform>())
        .Where(child => child.name.StartsWith("Line"))
        .Select(child => child.gameObject)
        .ToList();

    // Offset each line by the scroll position change
    foreach (GameObject line in lines)
    {
        LineRenderer lr = line.GetComponent<LineRenderer>();
        Vector3 lineStart = lr.GetPosition(0);
        Vector3 lineEnd = lr.GetPosition(1);

        // Create a Vector3 offset with the Y component adjusted by scrollLinePositionChange
        Vector3 offset = new Vector3(0f, -scrollLinePositionChange * scrollPositionSensitivity, 0f); // Use a negative offset to move the lines down
        // Offset each line by the scroll position change
        for (int i = 0; i < lr.positionCount; i++)
        {
            Vector3 position = lr.GetPosition(i);
            lr.SetPosition(i, position + offset);
        }
    }
}
    public void ConnectNodes(List<GameObject> nodes)
    {
        //sort nodes based on y position
        List<GameObject> sortedNodes = nodes.OrderBy(n => n.GetComponent<RectTransform>().anchoredPosition.y).ToList();

        //connect each node to closest lower node
        for (int i = 0; i < sortedNodes.Count; i++)
        {
            GameObject currentNode = sortedNodes[i];
            GameObject closestLowerNode = GetClosestNode(currentNode, GetLowerNodes(currentNode, sortedNodes));

            if (closestLowerNode != null)
                DrawLineBetweenNodes(currentNode, closestLowerNode);
        }

        //ensure each node has connection above
        foreach (GameObject node in sortedNodes)
        {
            if (!HasConnectionAbove(node))
            {
                GameObject closestUpperNode = GetClosestNode(node, GetUpperNodes(node, sortedNodes));
                if (closestUpperNode != null)
                    DrawLineBetweenNodes(node, closestUpperNode);
            }
        }
    }

    public GameObject GetClosestNode(GameObject referenceNode, List<GameObject> candidates)
    {
        return candidates.OrderBy(n => Vector2.Distance(n.GetComponent<RectTransform>().anchoredPosition, referenceNode.GetComponent<RectTransform>().anchoredPosition)).FirstOrDefault();
    }

    public GameObject GetClosestUpperNode(GameObject referenceNode, List<GameObject> candidates)
    {
        var referencePosition = referenceNode.GetComponent<RectTransform>().anchoredPosition;
        //filter out nodes that are not strictly above the reference node
        var upperNodes = candidates.Where(n => n.GetComponent<RectTransform>().anchoredPosition.y > referencePosition.y).ToList();
        //find the closest upper nodes based on x axis
        var closestNode = upperNodes.OrderBy(n => Mathf.Abs(n.GetComponent<RectTransform>().anchoredPosition.x - referencePosition.x)).FirstOrDefault();
        return closestNode;
    }

    public List<GameObject> GetLowerNodes(GameObject node, List<GameObject> allNodes)
    {
        return allNodes.Where(n => n.GetComponent<RectTransform>().anchoredPosition.y < node.GetComponent<RectTransform>().anchoredPosition.y).ToList();
    }

    public List<GameObject> GetUpperNodes(GameObject node, List<GameObject> allNodes)
    {
        return allNodes.Where(n => n.GetComponent<RectTransform>().anchoredPosition.y > node.GetComponent<RectTransform>().anchoredPosition.y).ToList();
    }

    private bool HasConnectionAbove(GameObject node)
    {
        List<GameObject> linesFromNode = GetLinesFromNode(node);
        return linesFromNode.Any(line => line.GetComponent<LineRenderer>().GetPosition(1).y > node.GetComponent<RectTransform>().anchoredPosition.y);
    }

    //retrieve all lines connected to a given node
    public List<GameObject> GetLinesFromNode(GameObject node)
    {
        return node.transform.Cast<Transform>().Where(child => child.name.StartsWith("Line")).Select(child => child.gameObject).ToList();
    }

    public void DrawLineBetweenNodes(GameObject nodeA, GameObject nodeB)
    {
        nodeParentMap[nodeB] = nodeA; //updating the dictionary with the dependency added (based from line drawn)
        lineDrawer.DrawLine(nodeA, nodeB);
    }
}