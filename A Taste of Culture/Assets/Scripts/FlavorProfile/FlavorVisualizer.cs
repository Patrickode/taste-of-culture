using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class FlavorVisualizer : MonoBehaviour
{
    public GameObject label;
    public TextMeshProUGUI labelText;

    public void DrawCircle(float radius, float lineWidth, int value, Color flavorColor)
    {
        if(value == 0) { return; }

        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        int segments = 360;
        
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = value;

        line.startColor = flavorColor;
        line.endColor = flavorColor;
        line.material.color = flavorColor;

        int pointCount = value;
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
        }

        label.transform.position = new Vector2(label.transform.position.x, points[0].y);         // Place label at same height as circle
        // label.transform.localScale = new Vector2(label.transform.localScale.x, lineWidth);

        line.SetPositions(points);
    }
}
