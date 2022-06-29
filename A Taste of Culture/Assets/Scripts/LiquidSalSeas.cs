using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSalSeas : SaladSeasoning
{
    [Header("Liquid Seasoning Fields")]
    [Tooltip("The minimum scale of this object will be localScale * minimumScaleFactor.")]
    [SerializeField] [Range(0, 1)] private float minimumScaleFactor;
    private Vector3 originalScale;
    private float originalAlpha;

    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
        originalAlpha = spRend.color.a;
    }

    protected override void MixAction()
    {
        Color newColor = spRend.color;
        newColor.a = Mathf.Lerp(originalAlpha, 0, MixProgress);
        spRend.color = newColor;

        transform.localScale = Vector3.Lerp(originalScale, originalScale * minimumScaleFactor, MixProgress);
    }
}