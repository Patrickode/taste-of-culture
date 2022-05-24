using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Chart inspo: https://www.google.com/url?sa=i&url=https%3A%2F%2Fslidebazaar.com%2Fitems%2Fradial-bar-chart-template-powerpoint%2F&psig=AOvVaw1BRl9uyHiGFeYYZWkEsBQD&ust=1653495891375000&source=images&cd=vfe&ved=0CAwQjRxqFwoTCLDi0q3G-PcCFQAAAAAdAAAAABAj

[RequireComponent(typeof(LineRenderer))]
public class SpiceChartVisualizer : MonoBehaviour
{
    [SerializeField] [Range(.3f, 5f)] float radius = 1f;
    [SerializeField] [Range(.01f, .5f)] float lineWidth = .05f;
    [SerializeField] int spiceCount = 9;

    // SpriteRenderer spriteRenderer;

    private void Awake() 
    { 
        // spriteRenderer = GetComponent<SpriteRenderer>();

        DrawCircle(radius, lineWidth); 
        DrawDivisions();
    }

    private void Start() { }

    private void Update() { DrawCircle(radius, lineWidth); }

    public void DrawCircle(float radius, float lineWidth)
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        int segments = 360;
        
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        int pointCount = segments + 1;                              // Add extra point to close circle
        Vector3[] points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0);
        }

        line.SetPositions(points);
    }

    public void DrawDivisions()
    {

    }
}
