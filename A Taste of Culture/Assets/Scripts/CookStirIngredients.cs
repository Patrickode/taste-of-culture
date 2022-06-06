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
    [Space(5)]
    [SerializeField] private float cookDuration;
    [Space(5)]
    [SerializeField] [Min(0)] private float timeUntilUnstirred;
    [Tooltip("The speed this ingredient needs to be moving at to be considered \"stirred.\"")]
    [SerializeField] [Min(0)] private float speedToBeStirred;
    [Tooltip("How long it'll take for the ingredient to start burning when sitting unstirred.")]
    [SerializeField] [Min(0)] private float burnStartup;
    [Tooltip("Measured in percentage points per second. Duration = 1/this.")]
    [SerializeField] [Range(0, 1)] private float burnPerSecond;
    [UnityEngine.Serialization.FormerlySerializedAs("unstirredParticleColor")]
    [SerializeField] private Color burningParticleColor;

    private float cookProgress;
    private static bool cookingStopped;

    private Coroutine unstirredCorout = null;
    private float burnStartupPercent;
    private bool isUnstirred = false;
    private Color originalSteamColor;

    public static System.Action DoneCooking;

    public float BurnPercent { get; private set; }

    private static Dictionary<CookStirIngredients, float> burnAmountDict
        = new Dictionary<CookStirIngredients, float>();

    private static List<float> _burnAmountList = new List<float>();
    public static List<float> BurnAmounts
    {
        get
        {
            _burnAmountList.Clear();
            _burnAmountList.AddRange(burnAmountDict.Values);
            return _burnAmountList;
        }
    }

    public static bool CookingPaused { get; set; }

    private void Start()
    {
        originalSteamColor = steamPSystem.main.startColor.color;
    }

    private void Update()
    {
        if (CookingPaused) return;

        if (!cookingStopped)
        {
            cookProgress += Time.deltaTime / cookDuration;
            if (cookProgress >= 1)
            {
                DoneCooking?.Invoke();
                cookingStopped = true;
            }

            LerpAlphaByProgress(ref uncookedSprite, 1, 0, cookProgress);
        }

        UpdateStirState();
    }

    private void LerpAlphaByProgress(ref SpriteRenderer rendToHide,
        float start, float end, float t, float tOffset = 0)
    {
        Color newC = rendToHide.color;
        newC.a = Mathf.Lerp(start, end, t + tOffset);
        rendToHide.color = newC;
    }

    private void UpdateStirState()
    {
        //If there's not a timer for how long this ingredient's been left unstirred, start one.
        if (unstirredCorout == null)
        {
            unstirredCorout = Coroutilities.DoAfterDelay(this,
                () => isUnstirred = true,
                timeUntilUnstirred);
        }

        //If this ingredient moves fast enough, we consider it stirred. Reset the burn startup + related vars.
        if (rbRef.velocity.sqrMagnitude >= speedToBeStirred * speedToBeStirred)
        {
            Coroutilities.TryStopCoroutine(this, ref unstirredCorout);
            isUnstirred = false;
            burnStartupPercent = 0;
        }
        //Otherwise, start burning; once burning has started (startup >= 1), add to burn percent.
        else if (isUnstirred)
        {
            burnStartupPercent += Time.deltaTime / burnStartup;

            if (burnStartupPercent >= 1)
            {
                BurnPercent += burnPerSecond * Time.deltaTime;
                burnAmountDict[this] = BurnPercent;
                LerpAlphaByProgress(ref burnedSprite, 0, 1, BurnPercent);
            }
        }

        //If we've got an active steam p system, make it reflect the amount of burnination happening.
        if (steamPSystem && steamPSystem.gameObject.activeInHierarchy)
        {
            //pSystem.main is read only, so it *must* be set indirectly like this.
            //This isn't a hack, it's intended.
            var pSysMain = steamPSystem.main;
            pSysMain.startColor = Color.Lerp(originalSteamColor, burningParticleColor, burnStartupPercent);
        }
    }
}
