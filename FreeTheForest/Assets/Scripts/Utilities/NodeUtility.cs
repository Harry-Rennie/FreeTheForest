using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NodeUtility
{
    public static bool CheckNodeViolation(List<Vector2> existingNodes, Vector2 newNode)
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

    //sorts from top to bottom
    public static List<Vector2> SortNodes(List<Vector2> nodeLocations)
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
