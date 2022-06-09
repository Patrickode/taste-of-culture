using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class FlavorVisualizer : MonoBehaviour
{
    public GameObject label;
    public TextMeshProUGUI labelText;

    float displaySpeed;

    public void DisplayFlavorValue(float radius, float lineWidth, int value, Color flavorColor, float gradualDisplaySpeed)
    {
        if(value == 0) { return; }

        displaySpeed = gradualDisplaySpeed;

        LineRenderer line = gameObject.GetComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.startColor = flavorColor;
        line.endColor = flavorColor;
        line.material.color = flavorColor;

        int segments = 360;
        List<Vector3> points = new List<Vector3>();

        var rad = Mathf.Deg2Rad * (0 * 360f / segments);
        points.Add(new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0));
        label.transform.position = new Vector2(label.transform.position.x, (label.transform.position.y / 2) + points[0].y);         // Place label at same height as circle

        StartCoroutine(GraduallyDisplayFlavor(line, radius, segments, points, value));
    }

    IEnumerator GraduallyDisplayFlavor(LineRenderer line, float radius, int segments, List<Vector3> points, int value)
    {
        int pointCount = 1; 

        while(pointCount < value)
        {
            var rad = Mathf.Deg2Rad * (pointCount * 360f / segments);
            points.Add(new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0));

            yield return new WaitForSeconds(displaySpeed);

            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());

            pointCount++;
        }
    }
}
