using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlavorVisualizer : MonoBehaviour
{
    [SerializeField] [Range(.3f, 5f)] float radius = 1f;
    [SerializeField] [Range(.01f, .5f)] float lineWidth = .05f;

    public void DrawCircle(int value)
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        int segments = 360;
        
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        int pointCount = segments + 1;                              // Add extra point to close circle
        Vector3[] points = new Vector3[pointCount];

        // for (int i = 0; i < pointCount; i++)
        for (int i = 0; i < value; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
        }

        line.SetPositions(points);
    }
}
