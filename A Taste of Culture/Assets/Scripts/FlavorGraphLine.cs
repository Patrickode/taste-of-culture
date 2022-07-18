using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorGraphLine : MonoBehaviour
{
    private enum GraphLine { Dynamic, Midline, Border }

    [SerializeField] private bool resetLineOnStart = true;
    [SerializeField] private bool initOnStart;
    [SerializeField] private bool animate;
    [SerializeField] private bool autoUpdate;
    [SerializeField] [Min(0)] private float defaultAnimTime;
    [Space(5)]
    [SerializeField] private LineRenderer line;
    [SerializeField] private RectTransform posSizeRef;
    [SerializeField] private FlavorType[] flavsToDisplay;
    [SerializeField] private bool useDataValues;
    [SerializeField] private int[] flavVals;
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
    private RectTransform[] labels = null;
    private TMPro.TextMeshProUGUI[] txtLabels = null;
    private Vector3[] labelOffsets;

    private TMPro.TextMeshProUGUI txtLabelPrefab;
    private List<Vector3> finalizedPointsCache = new List<Vector3>();
    private Coroutine pointsAnim;

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        labelAutoSizeOptns = Vector4.Max(labelAutoSizeOptns, new Vector4(0, 0, 0, Mathf.NegativeInfinity));

        labelAutoSizeOptns.x = Mathf.Min(labelAutoSizeOptns.x, labelAutoSizeOptns.y);
        labelAutoSizeOptns.y = Mathf.Max(labelAutoSizeOptns.y, labelAutoSizeOptns.x);
        labelAutoSizeOptns.z = Mathf.Min(labelAutoSizeOptns.z, 50);
        labelAutoSizeOptns.w = Mathf.Max(labelAutoSizeOptns.w, 0);

        if (flavVals.Length != flavsToDisplay.Length)
        {
            int[] resizer = new int[flavsToDisplay.Length];
            System.Array.Copy(flavVals, resizer, Mathf.Min(flavVals.Length, resizer.Length));
            flavVals = resizer;
        }
    });

    private void OnEnable()
    {
        if (autoUpdate)
            FlavorProfileData.FlavorUpdated += OnFlavorUpdate;
    }
    private void OnDisable()
    {
        FlavorProfileData.FlavorUpdated -= OnFlavorUpdate;
    }
    private void OnFlavorUpdate(FlavorType dontCare, int didntAsk) => SetGraphLine();

    private void Start()
    {
        refBounds = posSizeRef.GetWorldBounds();

        if (labelPrefab)
        {
            if (labelPrefab.TryGetComponent(out txtLabelPrefab))
                txtLabels = new TMPro.TextMeshProUGUI[flavsToDisplay.Length];
            else
                labels = new RectTransform[flavsToDisplay.Length];

            labelOffsets = new Vector3[flavsToDisplay.Length];
        }

        if (line && resetLineOnStart)
        {
            ResetLine(flavsToDisplay.Length);
            line.gameObject.SetActive(false);
        }

        if (initOnStart)
            SetGraphLine();
    }

    private void ResetLine(int newCount = -1) => ResetLine(refBounds.center, newCount);
    private void ResetLine(Vector3 resetPoint, int newCount = -1)
    {
        if (newCount >= 0)
            line.positionCount = newCount;

        for (int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, resetPoint);
        }
    }

    private Transform LabelAtIndex(int index)
    {
        if (!labelPrefab)
            return null;

        if (labels != null)
            return labels[index];

        if (txtLabels != null)
            return txtLabels[index] ? txtLabels[index].transform : null;

        return null;
    }

    public void SetGraphLine(float animDuration = -1)
    {
        Coroutilities.TryStopCoroutine(this, ref pointsAnim);

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
                i,
                center,
                center + new Vector3(
                    radius * Mathf.Cos(angle),
                    radius * Mathf.Sin(angle)),
                out float val);

            if (!LabelAtIndex(i))
                MakeLabel(i, center, val);
        }

        if (line)
        {
            line.gameObject.SetActive(true);
            line.positionCount = points.Length;

            if (animate)
            {
                pointsAnim = StartCoroutine(AnimLinePositions(points, animDuration >= 0
                    ? animDuration
                    : defaultAnimTime));
                return;
            }

            UtilFunctions.RemoveAdjacentDuplicatesNonAlloc(points, finalizedPointsCache);
            line.SetPositions(finalizedPointsCache.ToArray());
            return;
        }

        UtilFunctions.RemoveAdjacentDuplicatesNonAlloc(points, finalizedPointsCache);
    }

    private IEnumerator AnimLinePositions(Vector3[] targetPoints, float duration)
    {
        Vector3[] startPoints = new Vector3[line.positionCount];
        Vector3 nextPos;
        Transform labelCache;

        line.GetPositions(startPoints);

        if (duration > 0)
            for (float progress = 0; progress <= 1; progress += Time.deltaTime / duration)
            {
                for (int i = 0; i < startPoints.Length; i++)
                {
                    nextPos = Vector3.Lerp(startPoints[i], targetPoints[i], progress);
                    line.SetPosition(i, nextPos);

                    labelCache = LabelAtIndex(i);
                    if (labelCache)
                        labelCache.position = nextPos + labelOffsets[i];
                }

                yield return null;
            }

        //Now that the animation's done, remove any adjacent duplicates and reset the line's points to that.
        //  This ensures several points can converge to zero in the animation, then be removed if needed.
        UtilFunctions.RemoveAdjacentDuplicatesNonAlloc(targetPoints, finalizedPointsCache);
        line.positionCount = finalizedPointsCache.Count;
        line.SetPositions(finalizedPointsCache.ToArray());
    }

    private Vector3 RepositionPoint(int index, Vector3 zeroPoint, Vector3 point, out float valueUsed)
    {
        float interpolant = 0;
        valueUsed = 0;

        if (lineType == GraphLine.Border || lineType == GraphLine.Midline)
        {
            interpolant = percentOfMax;
            valueUsed = maxValue * percentOfMax;
        }

        else if (!useDataValues)
        {
            interpolant = Mathf.InverseLerp(0, maxValue, flavVals[index]);
            valueUsed = flavVals[index];
        }

        else if (FlavorProfileData.Instance.TryGetFlav(flavsToDisplay[index], out int flavValue))
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
        Quaternion labelRot;

        //No padding
        if (labelPadding.x <= 0 && labelPadding.y <= 0)
        {
            labelOffsets[index] = Vector3.zero;
            labelRot = rotateLabels
                ? Quaternion.LookRotation(Vector3.forward, awayFrmCenter)
                : Quaternion.identity;
        }
        //Uninverted padding direction - use bottom padding
        else if (Vector3.Dot(Vector3.up, awayFrmCenter) >= 0)
        {
            labelOffsets[index] = awayFrmCenter * labelPadding.x;
            labelRot = rotateLabels
                ? Quaternion.LookRotation(Vector3.forward, awayFrmCenter)
                : Quaternion.identity;
        }
        //Inverted padding direction - use top padding
        else
        {
            labelOffsets[index] = awayFrmCenter * labelPadding.y;
            labelRot = rotateLabels
                ? Quaternion.LookRotation(Vector3.forward, -awayFrmCenter)
                : Quaternion.identity;
        }

        //If we the label isn't text, no further adjustment's necessary.
        if (!txtLabelPrefab)
        {
            labels[index] = Instantiate(labelPrefab, points[index] + labelOffsets[index], labelRot, posSizeRef);
            return;
        }

        txtLabels[index] = Instantiate(txtLabelPrefab, points[index] + labelOffsets[index], labelRot, posSizeRef);

        txtLabels[index].text = useNumberLabels
            ? value.ToString()
            : System.Enum.GetName(typeof(FlavorType), flavsToDisplay[index]);

        if (adjustLabelAutoSize)
        {
            txtLabels[index].fontSizeMin = labelAutoSizeOptns.x;
            txtLabels[index].fontSizeMax = labelAutoSizeOptns.y;
            txtLabels[index].characterWidthAdjustment = labelAutoSizeOptns.z;
            txtLabels[index].lineSpacingAdjustment = labelAutoSizeOptns.w;
        }
    }
}
