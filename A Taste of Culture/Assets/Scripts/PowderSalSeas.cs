using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowderSalSeas : SaladSeasoning
{
    [Header("Powder Seasoning Fields")]
    private float originalAlpha;

    protected override void Start()
    {
        base.Start();
        originalAlpha = spRend.color.a;
    }

    protected override void MixAction()
    {
        Color newColor = spRend.color;
        newColor.a = Mathf.Lerp(originalAlpha, 0, MixProgress);
        spRend.color = newColor;
    }
}