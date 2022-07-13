using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorGraphLine : MonoBehaviour
{
    private enum GraphLine { Primary, Midline, Border }

    [SerializeField] private LineRenderer border;
    [SerializeField] private RectTransform posSizeRef;
    [SerializeField] private FlavorType[] flavsToDisplay;
    [Space(5)]
    [SerializeField] private GraphLine lineType;
    [SerializeField] private TMPro.TextMeshProUGUI labelPrefab;
    [SerializeField] private bool useNumberLabels;
    [SerializeField] private bool rotateLabels;
    [SerializeField] [VectorLabels("Bottom", "Top")] private Vector2 labelPadding;
    [Space(5)]
    [SerializeField] private bool adjustLabelAutoSize;
    [VectorLabels("Min", "Max", "WD%", "Line")]
    [SerializeField] private Vector4 labelAutoSizeOptns;
    [Space(10)]
    [SerializeField] private bool clockwise;
    [SerializeField] [Range(0, 360)] private float startAngle;
    [SerializeField] [Min(1)] private float maxValue = 10;
    [SerializeField] [Range(0, 1)] private float percentOfMax = 1;
    private float startAngInRads;

    private Bounds refBounds;
    private float radius;
    private Vector3[] points;
    private List<Vector3> pointsToBorder = new List<Vector3>();

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        labelAutoSizeOptns.x = Mathf.Min(labelAutoSizeOptns.x, labelAutoSizeOptns.y);
        labelAutoSizeOptns.y = Mathf.Max(labelAutoSizeOptns.y, labelAutoSizeOptns.x);
        labelAutoSizeOptns.z = Mathf.Clamp(labelAutoSizeOptns.z, 0, 50);
        labelAutoSizeOptns.w = Mathf.Clamp(labelAutoSizeOptns.w, Mathf.NegativeInfinity, 0);
    });

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
            points[i] = RepositionPoint(
                flavsToDisplay[i],
                center,
                center + new Vector3(
                    radius * Mathf.Cos(angle),
                    radius * Mathf.Sin(angle)),
                out float val);

            MakeLabel(i, center, val);
        }

        UtilFunctions.RemoveAdjacentDuplicatesNonAlloc(points, pointsToBorder);

        border.positionCount = pointsToBorder.Count;
        border.SetPositions(pointsToBorder.ToArray());
    }

    private Vector3 RepositionPoint(FlavorType type, Vector3 zeroPoint, Vector3 point, out float valueUsed)
    {
        float interpolant = 0;
        valueUsed = 0;

        if (lineType != GraphLine.Primary)
        {
            interpolant = percentOfMax;
            valueUsed = maxValue * percentOfMax;
        }

        else if (FlavorProfileData.Instance.TryGetFlav(type, out int flavValue))
        {
            interpolant = Mathf.InverseLerp(0, maxValue, flavValue);
            valueUsed = flavValue;
        }

        return Vector3.Lerp(zeroPoint, point, interpolant);
    }

    private void MakeLabel(int index, Vector3 center, float value)
    {
        if (!labelPrefab) return;

        var awayFrmCenter = (points[index] - center).normalized;
        Vector3 labelPos;
        Quaternion labelRot;

        if (labelPadding.x <= 0 && labelPadding.y <= 0)
        {
            labelPos = points[index];
            labelRot = rotateLabels ? Quaternion.LookRotation(Vector3.forward, awayFrmCenter) : Quaternion.identity;
        }
        else if (Vector3.Dot(Vector3.up, awayFrmCenter) >= 0)
        {
            labelPos = points[index] + awayFrmCenter * labelPadding.x;
            labelRot = rotateLabels ? Quaternion.LookRotation(Vector3.forward, awayFrmCenter) : Quaternion.identity;
        }
        else
        {
            labelPos = points[index] + awayFrmCenter * labelPadding.y;
            labelRot = rotateLabels ? Quaternion.LookRotation(Vector3.forward, -awayFrmCenter) : Quaternion.identity;
        }

        var newLabel = Instantiate(labelPrefab, labelPos, labelRot, posSizeRef);

        newLabel.text = useNumberLabels
            ? value.ToString()
            : System.Enum.GetName(typeof(FlavorType), flavsToDisplay[index]);

        if (adjustLabelAutoSize)
        {
            newLabel.fontSizeMin = labelAutoSizeOptns.x;
            newLabel.fontSizeMax = labelAutoSizeOptns.y;
            //I don't know if these are the right things to set, so they'll stay commented out for now
            /*newLabel.characterWidthAdjustment = labelAutoSizeOptns.z;
            newLabel.lineSpacingAdjustment = labelAutoSizeOptns.w;*/
        }
    }
}
