using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithUIElement : MonoBehaviour
{
    public enum AlignAnchor
    {
        Center,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
    }
    public enum AlignStyle { Collider, Rendered }

    [SerializeField] private RectTransform target;
    [SerializeField] private AlignAnchor alignAnchor;
    [SerializeField] private AlignStyle wayToAlign;
    [Space(5)]
    [SerializeField] private bool alignOnStart;
    [SerializeField] private bool alignOnUpdate;

    private Collider2D subjectColl;
    private SpriteRenderer subjectSpRend;
    private Bounds targetBoundsCache;
    private Bounds subjectBoundsCache;

    private void Start()
    {
        switch (wayToAlign)
        {
            case AlignStyle.Collider:
                if (TryGetComponent(out subjectColl))
                    subjectBoundsCache = subjectColl.bounds;
                else
                {
                    Debug.LogError($"{name} wants to align with a collider, but it has no colliders on it.");
                    alignOnStart = false;
                    alignOnUpdate = false;
                }
                break;

            case AlignStyle.Rendered:
                if (TryGetComponent(out subjectSpRend))
                    subjectBoundsCache = subjectSpRend.GetBoundsSansPadding();
                else
                {
                    Debug.LogError($"{name} wants to align with a sprite renderer, but it has no sprite renderers on it.");
                    alignOnStart = false;
                    alignOnUpdate = false;
                }
                break;

            default:
                alignOnStart = false;
                alignOnUpdate = false;
                break;
        }

        if (alignOnStart)
            Align();
    }
    private void Update()
    {
        if (alignOnUpdate)
            Align();
    }

    private void Align()
    {
        targetBoundsCache = target.GetWorldBounds();
        Vector2 tAlignPoint, sAlignPoint;

        switch (alignAnchor)
        {
            case AlignAnchor.Center:
                tAlignPoint = targetBoundsCache.center;
                sAlignPoint = subjectBoundsCache.center;
                break;
            case AlignAnchor.TopLeft:
                tAlignPoint = new Vector2(targetBoundsCache.min.x, targetBoundsCache.max.y);
                sAlignPoint = new Vector2(subjectBoundsCache.min.x, subjectBoundsCache.max.y);
                break;
            case AlignAnchor.TopRight:
                tAlignPoint = targetBoundsCache.max;
                sAlignPoint = subjectBoundsCache.max;
                break;
            case AlignAnchor.BottomRight:
                tAlignPoint = new Vector2(targetBoundsCache.max.x, targetBoundsCache.min.y);
                sAlignPoint = new Vector2(subjectBoundsCache.max.x, subjectBoundsCache.min.y);
                break;
            case AlignAnchor.BottomLeft:
                tAlignPoint = targetBoundsCache.min;
                sAlignPoint = subjectBoundsCache.min;
                break;

            default: return;
        }

        transform.position += (Vector3)(tAlignPoint - sAlignPoint);
    }
}