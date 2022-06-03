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

    private static Dictionary<CookStirIngredients, float> progressDict = new Dictionary<CookStirIngredients, float>();
    private static List<float> _progressList = new List<float>();
    public static List<float> AllProgress
    {
        get
        {
            _progressList.Clear();
            _progressList.AddRange(progressDict.Values);
            return _progressList;
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
        progressDict[this] = CookProgress;

        Color newC = rendToChange.color;
        newC.a = Mathf.Lerp(1, 0, CookProgress + lerpOffset);
        rendToChange.color = newC;
    }
}
