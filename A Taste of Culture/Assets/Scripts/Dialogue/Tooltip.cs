using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Text tooltipText;
    [SerializeField] private RectTransform backgroundRectTransform;
    [Space(5)]
    [SerializeField] float textPaddingSize = 12.5f;

    private static Tooltip instance;
    private RectTransform prntRectRef;
    private Vector2 localPointCache;

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

        localPointCache.x -= 50;
        localPointCache.y += 100;
        transform.localPosition = localPointCache;
    }

    private void ShowTooltip(string tooltipString)
    {
        gameObject.SetActive(true);

        tooltipText.text = tooltipString;
        Vector2 backgroundSize = new Vector2(
            tooltipText.preferredWidth + textPaddingSize * 2f,
            tooltipText.preferredHeight + textPaddingSize * 2f);
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
