using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorGraphLine : MonoBehaviour
{
    private enum GraphLine { Value, Midline, Border }

    [SerializeField] private bool initOnStart;
    [SerializeField] private bool animate;
    [Space(5)]
    [SerializeField] private LineRenderer line;
    [SerializeField] private RectTransform posSizeRef;
    [SerializeField] private FlavorType[] flavsToDisplay;
    [Space(5)]
    [SerializeField] private GraphLine lineType;
    [SerializeField] private RectTransform labelPrefab;
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
    private TMPro.TextMeshProUGUI labelText;

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        labelAutoSizeOptns = Vector4.Max(labelAutoSizeOptns, new Vector4(0, 0, 0, Mathf.NegativeInfinity));

        labelAutoSizeOptns.x = Mathf.Min(labelAutoSizeOptns.x, labelAutoSizeOptns.y);
        labelAutoSizeOptns.y = Mathf.Max(labelAutoSizeOptns.y, labelAutoSizeOptns.x);
        labelAutoSizeOptns.z = Mathf.Min(labelAutoSizeOptns.z, 50);
        labelAutoSizeOptns.w = Mathf.Max(labelAutoSizeOptns.w, 0);
    });

    private void Start()
    {
        UtilFunctions.SafeSetActive(line, false);

        if (initOnStart)
            SetGraphLine();
    }

    private void SetGraphLine()
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

        if (line)
        {
            line.gameObject.SetActive(true);
            line.positionCount = pointsToBorder.Count;
            line.SetPositions(pointsToBorder.ToArray());
        }
    }

    private Vector3 RepositionPoint(FlavorType type, Vector3 zeroPoint, Vector3 point, out float valueUsed)
    {
        float interpolant = 0;
        valueUsed = 0;

        if (lineType == GraphLine.Border || lineType == GraphLine.Midline)
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

        //No padding
        if (labelPadding.x <= 0 && labelPadding.y <= 0)
        {
            labelPos = points[index];
            labelRot = rotateLabels
                ? Quaternion.LookRotation(Vector3.forward, awayFrmCenter)
                : Quaternion.identity;
        }
        //Uninverted padding direction
        else if (Vector3.Dot(Vector3.up, awayFrmCenter) >= 0)
        {
            labelPos = points[index] + awayFrmCenter * labelPadding.x;
            labelRot = rotateLabels
                ? Quaternion.LookRotation(Vector3.forward, awayFrmCenter)
                : Quaternion.identity;
        }
        //Inverted padding direction
        else
        {
            labelPos = points[index] + awayFrmCenter * labelPadding.y;
            labelRot = rotateLabels
                ? Quaternion.LookRotation(Vector3.forward, -awayFrmCenter)
                : Quaternion.identity;
        }

        //Check if we have a text component for the label cached. If not, and we can't find one, bail out
        //immediately after spawning the label.
        if (!labelText && !labelPrefab.TryGetComponent(out labelText))
        {
            Instantiate(labelPrefab, labelPos, labelRot, posSizeRef);
            return;
        }

        var newLabel = Instantiate(labelText, labelPos, labelRot, posSizeRef);

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
