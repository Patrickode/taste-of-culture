using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlavorVisualizer : MonoBehaviour
{
    public void DrawCircle(float radius, float lineWidth, int value, Color flavorColor)
    {
        if(value == 0) { return; }

        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        int segments = 360;
        
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.startColor = flavorColor;
        line.endColor = flavorColor;
        line.positionCount = segments + 1;

        // Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        // line.material = whiteDiffuseMat;
        line.material.color = flavorColor;

        // int pointCount = segments + 1;                              // Add extra point to close circle
        int pointCount = value;
        Vector3[] points = new Vector3[pointCount];

        // Vector3[] points = new Vector3[value];

        for (int i = 0; i < pointCount; i++)
        // for (int i = 0; i < value; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
        }

        line.SetPositions(points);
    }
}
