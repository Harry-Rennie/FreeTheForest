using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public Dictionary<GameObject, GameObject> nodeParentMap = new Dictionary<GameObject, GameObject>();
    [SerializeField] private LineDrawer lineDrawer;
    //connect given nodes based on their position
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