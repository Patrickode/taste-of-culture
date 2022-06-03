using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookStirIngredients : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbRef;
    [SerializeField] private SpriteRenderer uncookedSprite;
    [SerializeField] private SpriteRenderer cookedSprite;
    [SerializeField] private SpriteRenderer burnedSprite;
    [SerializeField] private ParticleSystem steamPSystem;
    [SerializeField] private ParticleSystem donePSystem;
    [Space(5)]
    [SerializeField] private float cookDuration;
    [SerializeField] private float burnDuration;
    [Space(5)]
    [Tooltip("The speed this ingredient needs to be moving at to be considered \"stirred.\"")]
    [SerializeField] [Min(0)] private float speedAtWhichStirred;
    [SerializeField] private float unstirredPeakFactor;
    [SerializeField] [Min(0)] private float unstirredStartTime;
    [Tooltip("The time it'll take to go from a factor of 1 to `unstirredPeakFactor`, after `unstirredStartTime`" +
        " seconds of not being stirred.")]
    [SerializeField] [Min(0)] private float unstirredPeakTime;
    [SerializeField] private Color unstirredParticleColor;

    private Coroutine unstirredCorout = null;
    private bool isUnstirred = false;
    private float unstirredProgress = 0;
    private float unstirFactor = 1;
    private Color originalSteamColor;

    public float CookProgress { get; private set; }
    public bool DoneCooking { get => CookProgress >= 1; }

    private static Dictionary<CookStirIngredients, float> progressDict
        = new Dictionary<CookStirIngredients, float>();

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

    private void Start()
    {
        originalSteamColor = steamPSystem.main.startColor.color;
    }

    private void Update()
    {
        if (!DoneCooking)
            IncrementCookProgress(cookDuration, ref uncookedSprite);
        else
            IncrementCookProgress(burnDuration, ref cookedSprite, -1);

        UpdateStirState();

        if (steamPSystem)
        {
            var pSysMain = steamPSystem.main;
            pSysMain.startColor = Color.Lerp(originalSteamColor, unstirredParticleColor, unstirredProgress);
        }

        if (donePSystem && CookProgress >= 1)
        {
            donePSystem.Play();
            donePSystem = null;
        }
    }

    private void IncrementCookProgress(float duration, ref SpriteRenderer rendToChange, float lerpOffset = 0)
    {
        CookProgress += (Time.deltaTime / duration) * unstirFactor;
        progressDict[this] = CookProgress;

        Color newC = rendToChange.color;
        newC.a = Mathf.Lerp(1, 0, CookProgress + lerpOffset);
        rendToChange.color = newC;
    }

    private void UpdateStirState()
    {
        //If there's not a timer for how long this ingredient's been left unstirred, start one.
        if (unstirredCorout == null)
        {
            unstirredCorout = Coroutilities.DoAfterDelay(this,
                () => isUnstirred = true,
                unstirredStartTime);
        }

        //If this ingredient moves fast enough, we consider it stirred. Reset the unstirred timer + related vars.
        if (CookProgress < 2 && rbRef.velocity.sqrMagnitude >= speedAtWhichStirred * speedAtWhichStirred)
        {
            Coroutilities.TryStopCoroutine(this, ref unstirredCorout);
            isUnstirred = false;
            unstirredProgress = 0;
        }
        //Otherwise, climb towards the peak cook progress multiplier while unstirred.
        //Also consider it unstirred if cook progress is at the max, for cosmetic burned effect.
        else if (CookProgress >= 2 || isUnstirred)
        {
            unstirredProgress += Time.deltaTime / unstirredPeakTime;
        }

        unstirFactor = Mathf.Lerp(1, unstirredPeakFactor, unstirredProgress);
    }
}
