using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookStirIngredients : MonoBehaviour
{
    [SerializeField] private SpriteRenderer uncookedSprite;
    [SerializeField] private SpriteRenderer cookedSprite;
    [SerializeField] private SpriteRenderer burnedSprite;
    [Space(5)]
    [SerializeField] private float cookDuration;
    [SerializeField] private float burnDuration;

    public float CookProgress { get; private set; }
    public bool DoneCooking { get => CookProgress >= 1; }

    private static float _maxCookProgress = Mathf.NegativeInfinity;
    public static float MaxCookProgress
    {
        get => _maxCookProgress;
        private set
        {
            if (value > _maxCookProgress)
                _maxCookProgress = value;
        }
    }

    private void Update()
    {
        if (!DoneCooking)
            IncrementProgress(cookDuration, ref uncookedSprite);
        else
            IncrementProgress(burnDuration, ref cookedSprite, -1);
    }

    private void IncrementProgress(float duration, ref SpriteRenderer rendToChange, float lerpOffset = 0)
    {
        CookProgress += Time.deltaTime / duration;
        MaxCookProgress = CookProgress;

        Color newC = rendToChange.color;
        newC.a = Mathf.Lerp(1, 0, CookProgress + lerpOffset);
        rendToChange.color = newC;
    }
}
