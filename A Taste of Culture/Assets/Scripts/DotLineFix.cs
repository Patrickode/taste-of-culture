using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DotLineFix : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRef;
    private float widthCache;

    private void Update()
    {
        if (widthCache != lineRef.widthMultiplier)
        {
            widthCache = lineRef.widthMultiplier;
            lineRef.material.mainTextureScale = Vector2.right * (1 / widthCache) + Vector2.up;
        }
    }
}
