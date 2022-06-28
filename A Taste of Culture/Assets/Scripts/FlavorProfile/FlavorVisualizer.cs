using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class FlavorVisualizer : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI labelText;
    [SerializeField] [Range(-2, 2)] private float labelXSpacing;
    [SerializeField] [Range(0, 360)] private int minimumSegments = 2;

    private const int Segments = 360;

    private LineRenderer lineRef;
    private Coroutine gradualDisplay;

    private float storedRadius;
    private List<Vector3> storedPoints;
    private float storedInterval;

    public void DisplayFlavorValue(float radius, float lineWidth, int segments,
        Color flavorColor, float speed)
    {
        if (speed <= 0) return;
        if (segments < minimumSegments) { segments = minimumSegments; }

        storedInterval = speed;
        storedRadius = radius;

        lineRef = gameObject.GetComponent<LineRenderer>();

        lineRef.useWorldSpace = false;
        lineRef.startWidth = lineWidth;
        lineRef.endWidth = lineWidth;

        lineRef.startColor = flavorColor;
        lineRef.endColor = flavorColor;
        lineRef.material.color = flavorColor;

        storedPoints = new List<Vector3>();
        storedPoints.Add(new Vector3(Mathf.Sin(0) * radius, Mathf.Cos(0) * radius, 0));

        Vector3 labelDest = transform.position;
        labelDest.x += labelXSpacing;
        labelDest.y += radius;
        labelText.transform.position = labelDest;

        if (gradualDisplay == null)
        {
            gradualDisplay = StartCoroutine(GraduallyDisplay(segments, speed));
        }
    }

    public void UpdateDisplay(int segments, float speed)
    {
        if (speed <= 0) return;
        if (segments < minimumSegments) { segments = minimumSegments; }

        if (gradualDisplay == null)
        {
            gradualDisplay = StartCoroutine(GraduallyDisplay(segments, speed));
        }
    }

    private IEnumerator GraduallyDisplay(int segments, float speed)
    {
        if (speed <= 0 || lineRef.positionCount == segments)
        {
            gradualDisplay = null;
            yield break;
        }

        int counter = 0;
        int loopNum = 1;

        if (lineRef.positionCount >= segments)
            while (lineRef.positionCount > segments)
            {
                //If speed is 2, this will run twice per frame.
                //If speed is 0.5, this will run once, and then it won't run at all on the next loop.
                while (counter < speed * loopNum)
                {
                    lineRef.positionCount--;
                    counter++;
                }

                loopNum++;
                yield return new WaitForEndOfFrame();
            }
        else
            while (lineRef.positionCount < segments)
            {
                while (counter < speed * loopNum)
                {
                    var rad = Mathf.Deg2Rad * (counter * 360f / Segments);
                    storedPoints.Add(new Vector3(
                        Mathf.Sin(rad) * storedRadius,
                        Mathf.Cos(rad) * storedRadius,
                        0));

                    lineRef.positionCount = storedPoints.Count;
                    lineRef.SetPositions(storedPoints.ToArray());

                    counter++;
                }

                loopNum++;
                yield return new WaitForEndOfFrame();
            }

        gradualDisplay = null;
    }
}