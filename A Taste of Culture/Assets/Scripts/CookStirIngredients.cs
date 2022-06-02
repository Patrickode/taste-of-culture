using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookStirIngredients : MonoBehaviour
{
    [SerializeField] private SpriteRenderer uncookedSprite;
    [SerializeField] private SpriteRenderer cookedSprite;
    [SerializeField] private float cookDuration;

    public float CookProgress { get; private set; }
    public bool DoneCooking { get => CookProgress >= 1; }

    private void Update()
    {
        if (!DoneCooking)
        {
            IncrementCookProgress();
        }
    }

    private void IncrementCookProgress()
    {
        CookProgress += Time.deltaTime / cookDuration;

        Color newC = uncookedSprite.color;
        newC.a = Mathf.Lerp(1, 0, CookProgress);
        uncookedSprite.color = newC;
    }
}
