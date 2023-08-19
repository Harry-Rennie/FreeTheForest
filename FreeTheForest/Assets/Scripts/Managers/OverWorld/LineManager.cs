using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineManager : MonoBehaviour
{
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
                lineDrawer.DrawLine(currentNode, closestLowerNode);
        }

        //ensure each node has connection above
        foreach (GameObject node in sortedNodes)
        {
            if (!HasConnectionAbove(node))
            {
                GameObject closestUpperNode = GetClosestNode(node, GetUpperNodes(node, sortedNodes));
                if (closestUpperNode != null)
                    lineDrawer.DrawLine(node, closestUpperNode);
            }
        }
    }

    private GameObject GetClosestNode(GameObject referenceNode, List<GameObject> candidates)
    {
        return candidates.OrderBy(n => Vector2.Distance(n.GetComponent<RectTransform>().anchoredPosition, referenceNode.GetComponent<RectTransform>().anchoredPosition)).FirstOrDefault();
    }

    private List<GameObject> GetLowerNodes(GameObject node, List<GameObject> allNodes)
    {
        return allNodes.Where(n => n.GetComponent<RectTransform>().anchoredPosition.y < node.GetComponent<RectTransform>().anchoredPosition.y).ToList();
    }

    private List<GameObject> GetUpperNodes(GameObject node, List<GameObject> allNodes)
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
        lineDrawer.DrawLine(nodeA, nodeB);
    }
}