using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSalSeas : SaladSeasoning
{
    [Header("Liquid Seasoning Fields")]
    [SerializeField] [Range(0, 1)] private float minimumScaleFactor;
    private Vector3 originalScale;

    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
    }

    protected override void MixAction()
    {
        transform.localScale = Vector3.Lerp(originalScale, originalScale * minimumScaleFactor, MixProgress);
    }
}