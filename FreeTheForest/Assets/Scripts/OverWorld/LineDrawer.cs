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
        lineObject.transform.SetAsFirstSibling();

        LineRenderer lr = lineObject.AddComponent<LineRenderer>();
        lr.sortingOrder = 1;
        lr.textureMode = LineTextureMode.Tile;
        lr.startWidth = lr.endWidth = 0.075f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.material.color = Color.black;

        Vector3 p0 = rectA.position;
        Vector3 p2 = rectB.position;
        Vector3 midPoint = (p0 + p2) / 2;

        // Offset control point to get a curve. Adjust the Vector3.up value as needed.
        Vector3 p1 = midPoint + Vector3.up * 0.5f; // Change the multiplier to increase/decrease curve height

        int curveResolution = 20; // Increase for a smoother curve
        lr.positionCount = curveResolution;
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            lr.SetPosition(i, CalculateBezierPoint(t, p0, p1, p2));
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
    public bool HasLineBetween(GameObject tileA, GameObject tileB)
    {
        Vector3 positionA = tileA.GetComponent<RectTransform>().position;
        Vector3 positionB = tileB.GetComponent<RectTransform>().position;

        foreach (Transform child in tileA.transform)
        {
            LineRenderer lr = child.GetComponent<LineRenderer>();
            if (lr != null && lr.positionCount > 0)
            {
                Vector3 lineStart = lr.GetPosition(0);
                Vector3 lineEnd = lr.GetPosition(lr.positionCount - 1); // get the last position

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
