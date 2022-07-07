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
    private Vector3[] points;
    private List<Vector3> pointsToBorder = new List<Vector3>();

    private void Start()
    {
        refBounds = posSizeRef.GetWorldBounds();
        radius = Mathf.Min(refBounds.extents.x, refBounds.extents.y);

        startAngInRads = startAngle * Mathf.Deg2Rad * (clockwise ? -1 : 1);
        int numPoints = flavsToDisplay.Length;
        Vector3 center = refBounds.center;

        //For odd-numbered n-gons, the point opposite of start will be the middle of an edge instead of
        //a vertex, so said opposite point will not be the same distance from the center (apothem <= radius).
        if (numPoints % 2 > 0)
        {
            //Offset by half of the apothem for equal start/opposite padding.
            var halfApothem = (radius - (radius * Mathf.Cos(Mathf.PI / numPoints))) / 2;
            var dirToStartAng = Quaternion.AngleAxis(startAngle, Vector3.forward) * Vector3.right;
            center += -dirToStartAng * halfApothem;
        }

        points = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float angle = startAngInRads + 2 * Mathf.PI * i / numPoints;
            points[i] = LerpPointByData(
                flavsToDisplay[i],
                center,
                center + new Vector3(
                    radius * Mathf.Cos(angle),
                    radius * Mathf.Sin(angle)));
        }

        UtilFunctions.RemoveAdjacentDuplicatesNonAlloc(points, pointsToBorder);

        //If the last point is roughly equal to the center, we don't need to loop (and shouldn't if
        //we want to avoid weird line renderer shenanigans)
        border.loop = !pointsToBorder[pointsToBorder.Count - 1].EqualWithinRange(center, 0.01f);
        border.positionCount = pointsToBorder.Count;
        border.SetPositions(pointsToBorder.ToArray());
    }

    private Vector3 LerpPointByData(FlavorType type, Vector3 zeroPoint, Vector3 point)
    {
        if (FlavorProfileData.Instance.TryGetFlav(type, out int flavValue))
            return Vector3.Lerp(zeroPoint, point, Mathf.InverseLerp(0, maxValue, flavValue));

        return refBounds.center;
    }
}
