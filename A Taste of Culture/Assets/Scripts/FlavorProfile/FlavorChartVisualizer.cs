using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlavorChartVisualizer : MonoBehaviour
{
    [SerializeField] [Range(.3f, 5f)] float radius = 1f;
    [SerializeField] [Range(.01f, .5f)] float lineWidth = .05f;

    [SerializeField] int bitterness;
    [SerializeField] int spiciness;
    [SerializeField] int sweetness;
    [SerializeField] int saltiness;

    int[] flavors = new int[4];

    // Start is called before the first frame update
    void Start()
    {
        flavors[0] = bitterness;
        flavors[1] = spiciness;
        flavors[2] = sweetness;
        flavors[3] = saltiness;

        DetermineFlavorFractions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DetermineFlavorFractions()
    {
        // TODO: Get flavor values from save...
        int totalFlavors = bitterness + spiciness + sweetness + saltiness;

        foreach(int flavor in flavors)
        {
            float flavorFraction = flavor / totalFlavors;
            int segments = Mathf.RoundToInt(360 * flavorFraction);

            DrawCircle(radius, segments, lineWidth);
        }

        // float bitternessFrac = bitterness / totalFlavors;
        // float spicinessFrac = spiciness / totalFlavors;
        // float sweetnessFrac = sweetness / totalFlavors;
        // float saltinessFrac = saltiness / totalFlavors;
    }

    public void DrawCircle(float radius, int segments, float lineWidth)
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        // int segments = 360;
        
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
}
