using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Text tooltipText;
    [SerializeField] private RectTransform backgroundRectTransform;
    [Space(5)]
    [SerializeField] private float textPaddingSize = 12.5f;
    [SerializeField] private int maxLength = 300;
    [Space(5)]
    [Tooltip("0 = min (left/bottom), 1 = max (right/top).")]
    [SerializeField] private Vector2 offsetAnchor = Vector2.one * 0.5f;
    [SerializeField] private int xOffset = -150;
    [SerializeField] private int yOffset = 100;

    private static Tooltip instance;
    private RectTransform prntRectRef;
    private Vector2 localPointCache;

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        offsetAnchor.x = Mathf.Clamp01(offsetAnchor.x);
        offsetAnchor.y = Mathf.Clamp01(offsetAnchor.y);
    });

    private void Awake()
    {
        instance = this;
        prntRectRef = transform.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            prntRectRef,
            Input.mousePosition,
            uiCamera,
            out localPointCache);

        localPointCache.x += xOffset;
        localPointCache.y += yOffset;

        //0 = min on axis, 1 = max, and no offset = center based, 0.5.
        //Therefore, subtract/add half the size if anchor is 0/1; shift range over to [-0.5, 0.5].
        //  NOTE: We use maxLength for X because we handle whitespace from < maxLength widths in the next block
        localPointCache.x -= maxLength * (offsetAnchor.x - 0.5f);
        localPointCache.y -= backgroundRectTransform.sizeDelta.y * (offsetAnchor.y - 0.5f);
        transform.localPosition = localPointCache;

        //Assuming bgRect's pivot is (0, 0), if  less than maxLength, this will distribute the leftover space to
        //each side based on offsetAnchor
        localPointCache.x = maxLength - backgroundRectTransform.sizeDelta.x;
        transform.localPosition += Vector3.right * (localPointCache.x * offsetAnchor.x);

        //Not quite sure why we have to halve the offscreen offset but if we don't it doesn't behave right so shrug
        localPointCache = UtilFunctions.PixelsOffscreen(backgroundRectTransform.GetWorldBounds(), uiCamera, true);
        transform.localPosition -= (Vector3)localPointCache * 0.5f;
    }

    private void ShowTooltip(string tooltipString)
    {
        gameObject.SetActive(true);

        tooltipText.text = tooltipString;
        Vector2 backgroundSize = new Vector2(
            tooltipText.preferredWidth + textPaddingSize * 2f,
            tooltipText.preferredHeight + textPaddingSize * 2f);
        if (backgroundSize.x > maxLength)
        {
            backgroundSize.x = maxLength;
        }
        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipString)
    {
        instance.ShowTooltip(tooltipString);
    }

    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }
}
