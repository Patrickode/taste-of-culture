using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class FlavorVisualizer : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI labelText;
    [SerializeField] [Range(-2, 2)] private float labelXSpacing;
    [SerializeField] [Range(0, 360)] private int minimumValue = 2;

    private const int Segments = 360;

    private LineRenderer lineRef;
    private Coroutine gradualDisplay;

    private float radiusCache;
    private List<Vector3> pointsCache;
    private float speedCache;

    public void DisplayFlavorValue(float radius, float lineWidth, int value, Color flavorColor, float gradualDisplaySpeed)
    {
        if (value < minimumValue) { value = minimumValue; }

        speedCache = gradualDisplaySpeed;
        radiusCache = radius;

        lineRef = gameObject.GetComponent<LineRenderer>();

        lineRef.useWorldSpace = false;
        lineRef.startWidth = lineWidth;
        lineRef.endWidth = lineWidth;

        lineRef.startColor = flavorColor;
        lineRef.endColor = flavorColor;
        lineRef.material.color = flavorColor;

        pointsCache = new List<Vector3>();
        pointsCache.Add(new Vector3(Mathf.Sin(0) * radius, Mathf.Cos(0) * radius, 0));

        Vector3 labelDest = transform.position;
        labelDest.x += labelXSpacing;
        labelDest.y += radius;
        labelText.transform.position = labelDest;

        if (gradualDisplay == null)
        {
            gradualDisplay = StartCoroutine(GraduallyDisplay(
                pointsCache, radius, value));
        }
    }

    public void UpdateDisplay(int value, float gradualDisplaySpeed = -1)
    {
        if (gradualDisplay == null)
        {
            gradualDisplay = StartCoroutine(GraduallyDisplay(
                pointsCache, radiusCache, value, gradualDisplaySpeed));
        }
    }

    IEnumerator GraduallyDisplay(List<Vector3> points, float radius, int value, float interval = -1)
    {
        if (interval < 0)
            interval = speedCache;
        int counter = points.Count;

        //Untested code for updating visualizer to lower value
        if (counter >= value)
            while (counter > value)
            {
                lineRef.positionCount -= 1;
                counter--;
                yield return new WaitForSeconds(interval);
            }
        else
            while (counter < value)
            {
                var rad = Mathf.Deg2Rad * (counter * 360f / Segments);
                points.Add(new Vector3(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius, 0));

                yield return new WaitForSeconds(interval);

                lineRef.positionCount = points.Count;
                lineRef.SetPositions(points.ToArray());

                counter++;
            }

        gradualDisplay = null;
    }
}