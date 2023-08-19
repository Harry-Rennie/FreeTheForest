using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineUtility : MonoBehaviour
{
    [SerializeField] private LineDrawer lineDrawer;

    public void ConnectNodes(List<GameObject> nodes)
    {
        List<GameObject> sortedNodes = nodes.OrderBy(n => n.GetComponent<RectTransform>().anchoredPosition.y).ToList();

        //connecting lower nodes to current node
        for (int i = 0; i < sortedNodes.Count; i++)
        {
            GameObject currentNode = sortedNodes[i];
            List<GameObject> lowerNodes = sortedNodes.Where(n => n.GetComponent<RectTransform>().anchoredPosition.y < currentNode.GetComponent<RectTransform>().anchoredPosition.y).ToList();
            if (!lowerNodes.Any()) continue;

            GameObject closestNode = lowerNodes.OrderBy(n => Vector2.Distance(n.GetComponent<RectTransform>().anchoredPosition, currentNode.GetComponent<RectTransform>().anchoredPosition)).First();
            lineDrawer.DrawLine(currentNode, closestNode);
        }

        //ensuring each node has a connection above
        foreach (GameObject node in sortedNodes)
        {
            List<GameObject> linesFromNode = GetLinesFromNode(node);
            bool hasConnectionAbove = linesFromNode.Any(line => line.GetComponent<LineRenderer>().GetPosition(1).y > node.GetComponent<RectTransform>().anchoredPosition.y);
            if (!hasConnectionAbove)
            {
                List<GameObject> upperNodes = sortedNodes.Where(n => n.GetComponent<RectTransform>().anchoredPosition.y > node.GetComponent<RectTransform>().anchoredPosition.y).ToList();
                if (!upperNodes.Any()) continue;

                GameObject closestNode = upperNodes.OrderBy(n => Vector2.Distance(n.GetComponent<RectTransform>().anchoredPosition, node.GetComponent<RectTransform>().anchoredPosition)).First();
                lineDrawer.DrawLine(node, closestNode);
            }
        }
    }

    public List<GameObject> GetLinesFromNode(GameObject node)
    {
        //retrieve all lines connected to a node
        return node.transform.Cast<Transform>().Where(child => child.name.StartsWith("Line")).Select(child => child.gameObject).ToList();
    }

    public void DrawLineBetweenNodes(GameObject nodeA, GameObject nodeB)
    {
        lineDrawer.DrawLine(nodeA, nodeB);
    }
}