using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transitions : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Canvas backingCanv;
    [SerializeField] private UnityEngine.UI.Image backingImg;
    [Space(5)]
    [SerializeField] private float defaultSpeedMult = 1;
    [SerializeField] private float screenToRateRatio = 1.1236f;
    [SerializeField] private float zPosition = 10;

    private ParticleSystem.MainModule mainCache;
    private ParticleSystem.ShapeModule shapeCache;
    private ParticleSystem.EmissionModule emitCache;
    private float[] origVals = { 0, 0, 0, 0 };

    private Camera mainCam;
    private Keyframe startOfPeakKey;
    private Keyframe endOfPeakKey;
    private float rateForSize;
    private bool pausedAtMidpoint;

    private static Transitions duplicationPreventer = null;

    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="int"/>: The index of the scene to load. (<i>Relative if negative; 
    /// -2 = two scenes ahead of this one.</i>)<br/>
    /// - <see cref="float"/>: The speed of the transition. Pass &lt;= 0 to use default speed 
    /// (as set in the inspector).
    /// </summary>
    public static System.Action<int, float> LoadWithTransition;
    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="bool"/>: Should we pause when we hit the midpoint?<br/>
    /// - <see cref="float"/>: The speed of the transition. Pass &lt;= 0 to use default speed 
    /// (as set in the inspector).
    /// </summary>
    public static System.Action<bool, float> StartTransition;
    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="bool"/>: Are we paused now that we've hit the midpoint?
    /// </summary>
    public static System.Action<bool> MidTransition;
    public static System.Action EndTransition;
    public static System.Action ContinueTransition;
    private Coroutine waitForMidCorout;
    private Coroutine midPauseCorout;

    public static void UI_LoadWithTransition(int index) => LoadWithTransition?.Invoke(index, -1);

    private void Start()
    {
        if (!duplicationPreventer)
        {
            duplicationPreventer = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        mainCam = Camera.main;
        backingCanv.worldCamera = mainCam;
        backingCanv.gameObject.SetActive(false);

        GetPeakKeys();
        InitParticleValues();
    }

    private void OnEnable()
    {
        LoadWithTransition += OnLoadWithTransition;
        StartTransition += OnStartTransition;
        MidTransition += OnMidTransition;
        ContinueTransition += OnContinueTransition;
    }
    private void OnDisable()
    {
        LoadWithTransition -= OnLoadWithTransition;
        StartTransition -= OnStartTransition;
        MidTransition -= OnMidTransition;
        ContinueTransition -= OnContinueTransition;
    }
    private void OnParticleSystemStopped() => EndTransition?.Invoke();

    private void OnLoadWithTransition(int index, float speed = 0)
    {
        index = index >= 0 ? index : SceneManager.GetActiveScene().buildIndex - index;
        StartTransition?.Invoke(true, speed);

        MidTransition += LoadOnMidpoint;
        void LoadOnMidpoint(bool _)
        {
            MidTransition -= LoadOnMidpoint;
            SceneManager.LoadScene(index);
            SceneManager.sceneLoaded += ContinueWhenDone;
        }

        void ContinueWhenDone(Scene dontCare, LoadSceneMode didntAsk)
        {
            SceneManager.sceneLoaded -= ContinueWhenDone;
            ContinueTransition?.Invoke();
        }
    }

    private void OnStartTransition(bool pauseOnMid, float speed = 0)
    {
        //Unity gets peeved if speed is set while the system's running. Since the system shouldn't be
        //running anyway, stop it just in case (for example, this is called twice in rapid succession)
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        SetSpeed(speed);
        particles.Play();

        //Wait until we hit the midpoint of the transition (not just the middle of the system
        //duration; give the midpoint particles a chance to get onscreen)
        float midTime = mainCache.duration / 2 + mainCache.startLifetime.constant / 2;

        Coroutilities.TryStopCoroutine(this, ref waitForMidCorout);
        waitForMidCorout = Coroutilities.DoAfterDelay(this,
            () =>
            {
                //then note that we got there to everyone who's listening.
                MidTransition?.Invoke(pauseOnMid);
                pausedAtMidpoint = pauseOnMid;

                if (pauseOnMid)
                    particles.Pause();
            },
            midTime);

        FadeBackingImg(pauseOnMid, midTime);
    }

    private void OnMidTransition(bool pauseOnMid)
    {
        if (!pauseOnMid) return;

        Coroutilities.TryStopCoroutine(this, ref midPauseCorout);
        midPauseCorout = Coroutilities.DoWhen(this, () => particles.Play(), () => !pausedAtMidpoint);
    }

    private void OnContinueTransition() => pausedAtMidpoint = false;

    private void FadeBackingImg(bool pauseOnMid, float midTime)
    {
        backingCanv.gameObject.SetActive(true);

        //First, set up a progress tracker, then determine the duration (end time - start time).
        //  End = duration of the transition p system, plus some time for the last particles to leave the screen
        //  Start = Visual mid; the time when the particle sizes peak, plus some time for those peak particles to get on screen
        float fadeProgress = 0;
        float visualMid = Mathf.Lerp(0, mainCache.duration, startOfPeakKey.time) + mainCache.startLifetime.constant / 4;
        float duration = (mainCache.duration + mainCache.startLifetime.constant / 2) - visualMid;

        //Once we get to the start of the particle size peak, start the fade process.
        Coroutilities.DoAfterDelay(this, FadeBasedOnPause, visualMid);

        //Disable the backing canvas when the fading's done.
        Coroutilities.DoWhen(this, () => backingCanv.gameObject.SetActive(false), () => fadeProgress >= 1);

        void FadeBasedOnPause()
        {
            //If we're pausing at the mid point,
            if (pauseOnMid)
            {
                //fade to white (the middle of the fade process), then wait till the transition resumes.
                FadeUntilT(0.5f);
                ContinueTransition += ResumeFade;
                return;
            }

            //If not, no need to wait, just go all the way through.
            FadeUntilT(1);
        }

        void FadeUntilT(float t) => Coroutilities.DoUntil(this, FadeImgColor, () => fadeProgress >= t);

        void FadeImgColor()
        {
            fadeProgress += Time.deltaTime / duration;
            Color newColor = backingImg.color;
            newColor.a = UtilFunctions.Lerp3Point(0, 1, 0, fadeProgress);
            backingImg.color = newColor;
        }

        void ResumeFade()
        {
            ContinueTransition -= ResumeFade;
            FadeUntilT(1);
        }
    }

    private void GetPeakKeys()
    {
        //Init the peak keys to low values so they can be overwritten
        startOfPeakKey = endOfPeakKey = new Keyframe(0, float.MinValue);

        var maxKeys = particles.main.startSize.curveMax.keys;
        foreach (var key in maxKeys)
        {
            //The FIRST highest value key (ties DON'T overwrite)
            if (key.value > startOfPeakKey.value)
                startOfPeakKey = key;
            //The LAST highest value key (ties DO overwrite)
            if (key.value >= endOfPeakKey.value)
                endOfPeakKey = key;
        }
    }

    private void InitParticleValues()
    {
        Vector3 destination = mainCam.ScreenToWorldPoint(Vector3.zero);
        Vector3 camCenterToMin = destination - mainCam.transform.position;

        destination.x = mainCam.transform.position.x;
        destination.y -= startOfPeakKey.value * particles.main.startSizeMultiplier / 2;
        destination.z = zPosition;
        transform.position = destination;

        shapeCache = particles.shape;
        float sizeToRate = particles.emission.rateOverTime.constant / shapeCache.scale.x;
        shapeCache.scale = new Vector3(Mathf.Abs(camCenterToMin.x) * screenToRateRatio, 1, 1);

        emitCache = particles.emission;
        emitCache.rateOverTime = shapeCache.scale.x * sizeToRate;
        rateForSize = emitCache.rateOverTime.constant;

        mainCache = particles.main;

        origVals[0] = mainCache.startSpeed.constant;
        origVals[1] = emitCache.rateOverTime.constant;
        origVals[2] = mainCache.startLifetimeMultiplier;
        origVals[3] = mainCache.duration;

        SetSpeed();
    }
    private void SetSpeed(float newSpeed = 0)
    {
        if (newSpeed <= 0)
            newSpeed = defaultSpeedMult;

        mainCache.startSpeed = origVals[0] * newSpeed;
        emitCache.rateOverTime = origVals[1] * newSpeed;

        mainCache.startLifetimeMultiplier = origVals[2] / newSpeed;
        mainCache.duration = origVals[3] / newSpeed;
    }
}
