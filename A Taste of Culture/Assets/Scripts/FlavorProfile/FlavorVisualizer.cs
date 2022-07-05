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
    [SerializeField] [Min(0)] public float minimumSpeed = 0;

    private const int Segments = 360;

    private LineRenderer lineRef;
    private Coroutine gradualDisplay;

    private float storedRadius;
    private List<Vector3> storedPoints;

    public void DisplayFlavorValue(float radius, float lineWidth, int segments,
        Color flavorColor, float duration)
    {
        if (duration <= 0) return;
        if (segments < minimumSegments) { segments = minimumSegments; }

        lineRef = gameObject.GetComponent<LineRenderer>();
        storedRadius = radius;

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
            gradualDisplay = StartCoroutine(GraduallyDisplay(segments, duration));
        }
    }

    public void UpdateDisplay(int segments, float duration)
    {
        if (duration <= 0) return;
        if (segments < minimumSegments) { segments = minimumSegments; }

        if (gradualDisplay == null)
        {
            gradualDisplay = StartCoroutine(GraduallyDisplay(segments, duration));
        }
    }

    private IEnumerator GraduallyDisplay(int segments, float duration)
    {
        if (duration <= 0 || lineRef.positionCount == segments)
        {
            gradualDisplay = null;
            yield break;
        }

        float speed = UtilFunctions.ClampOutside((segments - lineRef.positionCount) / duration, -minimumSpeed, minimumSpeed);
        float floatifiedCount = lineRef.positionCount;

        while (lineRef.positionCount != segments)
        {
            floatifiedCount += speed * Time.deltaTime;
            //Ensure that, regardless of any wacky deltaTimes, the count cannot exceed our target
            floatifiedCount = speed > 0
                ? Mathf.Min(floatifiedCount, segments)
                : Mathf.Max(floatifiedCount, segments);

            //Whenever our float counter crosses an int threshold (the cast cuts off decimals), increment/decrement.
            //  For high speeds, this'll call behavior several times per frame. (SetSegments prevents overshooting.)
            //  For low speeds, this'll skip calls to behavior on some frames.
            while (lineRef.positionCount != (int)floatifiedCount)
                SetSegments(speed > 0, segments);

            yield return null;
        }

        gradualDisplay = null;
    }

    private void SetSegments(bool add, int target)
    {
        if (lineRef.positionCount == target) return;

        if (!add)
        {
            storedPoints.RemoveAt(storedPoints.Count - 1);
            lineRef.positionCount--;
            return;
        }

        var rad = Mathf.Deg2Rad * (storedPoints.Count * 360f / Segments);
        storedPoints.Add(new Vector3(
            Mathf.Sin(rad) * storedRadius,
            Mathf.Cos(rad) * storedRadius,
            0));

        lineRef.positionCount = storedPoints.Count;
        lineRef.SetPositions(storedPoints.ToArray());
    }
}