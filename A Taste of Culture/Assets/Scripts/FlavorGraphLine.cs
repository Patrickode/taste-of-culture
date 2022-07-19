using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorGraphLine : MonoBehaviour
{
    private enum GraphLine { Dynamic, Midline, Border }

    [SerializeField] private bool resetLineOnStart = true;
    [SerializeField] private bool initOnStart;
    [SerializeField] private bool animate;
    [SerializeField] private bool animateOnStart;
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

    //Sanitize inspector values
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
            //Only init the array we'll need; non-text or text
            if (labelPrefab.TryGetComponent(out txtLabelPrefab))
                txtLabels = new TMPro.TextMeshProUGUI[flavsToDisplay.Length];
            else
                labels = new RectTransform[flavsToDisplay.Length];

            labelOffsets = new Vector3[flavsToDisplay.Length];
        }

        //We'll show the line again when it's finished initialization
        if (line)
            line.enabled = false;

        if (initOnStart)
            SetGraphLine(animateOnStart ? -1 : 0, resetLineOnStart);
        else if (resetLineOnStart && line)
            ResetLine(flavsToDisplay.Length);
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

    public void SetGraphLine(float animDuration = -1, bool shouldResetLine = false)
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
            //Offset by half of the apothem for equal start/opposite padding (distance from refBounds edges)
            var halfApothem = (radius - (radius * Mathf.Cos(Mathf.PI / numPoints))) / 2;
            var dirToStartAng = Quaternion.AngleAxis(startAngle, Vector3.forward) * Vector3.right;
            center += -dirToStartAng * halfApothem;
        }

        if (shouldResetLine && line)
            ResetLine(center, flavsToDisplay.Length);

        points = new Vector3[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            //2PI split into flavsToDisplay number of segments
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
            if (animate)
            {
                pointsAnim = StartCoroutine(AnimLinePositions(points, center, animDuration >= 0
                    ? animDuration
                    : defaultAnimTime));
                return;
            }

            line.enabled = true;
            TrimLineAdjDupes(points);
            //If there are only two points, the end's already connected to the start, no need to loop
            line.loop = line.positionCount > 2;
            return;
        }

        UtilFunctions.RemoveAdjacentDuplicatesNonAlloc(points, finalizedPointsCache);
    }

    private IEnumerator AnimLinePositions(Vector3[] targetPoints, Vector3 zeroPoint, float duration)
    {
        line.positionCount = targetPoints.Length;

        Vector3[] startPoints = new Vector3[line.positionCount];
        line.GetPositions(startPoints);

        Vector3 nextPos;
        Transform labelCache;
        int targZeros = 0;

        bool[] matches = new bool[startPoints.Length];
        bool willAnim = false;

        for (int i = 0; i < startPoints.Length; i++)
        {
            //Vector3.zero is a valid init point, but when it is, zeroPoint should be equal to it.
            if (startPoints[i] == Vector3.zero)
                startPoints[i] = zeroPoint;

            if (targetPoints[i] == zeroPoint)
                targZeros++;

            if (startPoints[i] == targetPoints[i])
                matches[i] = true;
            else
                willAnim = true;
        }

        //Don't loop the line unless there are two or more non-zero target points.
        line.loop = targZeros < targetPoints.Length - 1;

        //If not animating, set duration to zero; we're essentially already done.
        duration = willAnim ? duration : 0;
        //If the line's already disabled and we're not animating, leave it disabled.
        line.enabled = willAnim || line.enabled;

        //Note progress's initialization. The loop is skipped if duration is too short.
        for (float progress = duration > 0 ? 0 : 1; progress <= 1; progress += Time.deltaTime / duration)
        {
            for (int i = 0; i < startPoints.Length; i++)
            {
                //Even if this point won't move, don't skip the first loop, to ensure proper initialization
                if (progress > 0 && matches[i]) continue;

                nextPos = Vector3.Lerp(startPoints[i], targetPoints[i], progress);
                line.SetPosition(i, nextPos);

                labelCache = LabelAtIndex(i);
                if (labelCache)
                    labelCache.position = nextPos + labelOffsets[i];
            }

            yield return null;
        }

        //Remove adjacent duplicates and reset the line's points to the result *after* the animation.
        //  This ensures several points, adjacent or not, can converge to zero before removal.
        TrimLineAdjDupes(targetPoints);
    }

    private Vector3 RepositionPoint(int index, Vector3 zeroPoint, Vector3 point, out float valueUsed)
    {
        float interpolant = 0;
        valueUsed = 0;

        //Static line; use percentOfMax
        if (lineType == GraphLine.Border || lineType == GraphLine.Midline)
        {
            interpolant = percentOfMax;
            valueUsed = maxValue * percentOfMax;
        }

        //Dynamic line; use flavor data's value
        else if (!useDataValues)
        {
            interpolant = Mathf.InverseLerp(0, maxValue, flavVals[index]);
            valueUsed = flavVals[index];
        }

        //Dynamic line; use inspector value
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

        //If the label isn't text, no further adjustment's necessary.
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

    private void TrimLineAdjDupes(Vector3[] pointsToTrim)
    {
        UtilFunctions.RemoveAdjacentDuplicatesNonAlloc(pointsToTrim, finalizedPointsCache);
        line.positionCount = finalizedPointsCache.Count;
        line.SetPositions(finalizedPointsCache.ToArray());
    }
}
