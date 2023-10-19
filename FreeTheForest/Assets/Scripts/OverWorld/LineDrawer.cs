using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public void DrawLine(GameObject tileA, GameObject tileB)
    {
        if (tileA == null || tileB == null)
        {
            return;
        }
        RectTransform rectA = tileA.GetComponent<RectTransform>();
        RectTransform rectB = tileB.GetComponent<RectTransform>();

        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(tileA.transform, false);
        lineObject.transform.SetAsFirstSibling(); //makes sure the line is behind the node visually

        LineRenderer lr = lineObject.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, rectA.position);
        lr.SetPosition(1, rectB.position);
        lr.sortingOrder = 1;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.black;
        lr.startWidth = lr.endWidth = 0.1f;
    }
    public bool HasLineBetween(GameObject tileA, GameObject tileB)
    {
        Vector3 positionA = tileA.GetComponent<RectTransform>().position;
        Vector3 positionB = tileB.GetComponent<RectTransform>().position;

        foreach (Transform child in tileA.transform)
        {
            LineRenderer lr = child.GetComponent<LineRenderer>();
            if (lr != null)
            {
                Vector3 lineStart = lr.GetPosition(0);
                Vector3 lineEnd = lr.GetPosition(1);

                //check both orders since the line can start from A to B or B to A
                if ((ArePositionsEqual(lineStart, positionA) && ArePositionsEqual(lineEnd, positionB))
                    || (ArePositionsEqual(lineStart, positionB) && ArePositionsEqual(lineEnd, positionA)))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool ArePositionsEqual(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Distance(pos1, pos2) < 0.1f;
    }
}
