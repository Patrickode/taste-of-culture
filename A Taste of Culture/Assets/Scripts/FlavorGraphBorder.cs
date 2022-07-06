using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorGraphBorder : MonoBehaviour
{
    [SerializeField] private LineRenderer border;
    [SerializeField] private RectTransform posSizeRef;
    [SerializeField] private FlavorType[] flavsToDisplay;
    [Space(5)]
    [SerializeField] private bool clockwise;
    [SerializeField] [Range(0, 360)] private float startAngle;
    [SerializeField] private int maxValue = 10;
    private float startAngInRads;

    private Bounds refBounds;
    private float radius;

    private void Start()
    {
        refBounds = posSizeRef.GetWorldBounds();
        radius = Mathf.Min(refBounds.extents.x, refBounds.extents.y);
        Debug.DrawRay(refBounds.center, Vector3.up * radius, Color.cyan, 100);
        Debug.DrawRay(refBounds.center, Vector3.down * radius, Color.blue, 100);
        Debug.DrawRay(refBounds.center, Vector3.down * (radius - radius / 5), Color.magenta, 100);
        Debug.DrawRay(refBounds.center, Quaternion.AngleAxis(120, Vector3.forward) * (Vector3.up * radius), Color.green, 100);
        Debug.DrawRay(refBounds.center, Quaternion.AngleAxis(120 * 2, Vector3.forward) * (Vector3.up * radius), Color.green, 100);

        startAngInRads = startAngle * Mathf.Deg2Rad * (clockwise ? -1 : 1);
        int numPoints = flavsToDisplay.Length;

        Vector3 center = refBounds.center;
        if (numPoints % 2 > 0)
            center.y -= (radius - (radius * Mathf.Cos(Mathf.PI / numPoints))) / 2;

        var points = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float angle = startAngInRads + 2 * Mathf.PI * i / numPoints;
            points[i] = center + new Vector3(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle));
        }

        border.positionCount = points.Length;
        border.SetPositions(points);
    }
}
