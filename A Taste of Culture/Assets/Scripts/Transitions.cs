using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transitions : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float screenToEmitterRatio = 1.1236f;
    [SerializeField] private float zPosition = 10;

    private Camera mainCam;
    private Keyframe startOfPeakKey;
    private Keyframe endOfPeakKey;
    private bool midpointPause;

    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="bool"/>: Should we pause when we hit the midpoint?
    /// </summary>
    public static System.Action<bool> TransitionStart;
    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="bool"/>: Are we paused now that we've hit the midpoint?
    /// </summary>
    public static System.Action<bool> TransitionMid;
    public static System.Action TransitionEnd;
    public static System.Action ContinueTransition;
    private Coroutine waitForMid;

    private void Start()
    {
        InitPeakKeys();
        SetValsBasedOnCam();

        //Demonstration
        /*Coroutilities.DoAfterSequence(this, () => ContinueTransition?.Invoke(),
            () => Coroutilities.DoAfterDelay(this, () => TransitionStart?.Invoke(true), 2),
            () => new WaitUntil(() => midpointPause),
            () => new WaitForSeconds(3));*/
    }

    private void OnEnable()
    {
        TransitionStart += OnTransitionStart;
        ContinueTransition += OnContinueTransition;
    }
    private void OnDisable()
    {
        TransitionStart -= OnTransitionStart;
        ContinueTransition -= OnContinueTransition;
    }
    private void OnParticleSystemStopped() => TransitionEnd?.Invoke();

    private void OnTransitionStart(bool pauseOnMid)
    {
        particles.Play();

        Coroutilities.TryStopCoroutine(this, ref waitForMid);
        waitForMid = Coroutilities.DoAfterDelay(this,
            () =>
            {
                TransitionMid?.Invoke(pauseOnMid);
                midpointPause = pauseOnMid;

                if (pauseOnMid)
                    particles.Pause();
            },
            particles.main.duration / 2 + particles.main.startLifetime.constant / 2);
    }

    private void OnContinueTransition()
    {
        if (!midpointPause)
        {
            Debug.LogWarning("Attempted to continue a transition when there isn't one in progress; " +
                "starting a transition instead.");
            OnTransitionStart(false);
            return;
        }

        particles.Play();
    }

    private void InitPeakKeys()
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

    private void SetValsBasedOnCam()
    {
        mainCam = Camera.main;
        Vector3 destination = mainCam.ScreenToWorldPoint(Vector3.zero);
        Vector3 camCenterToMin = destination - mainCam.transform.position;

        destination.x = mainCam.transform.position.x;
        destination.y -= startOfPeakKey.value * particles.main.startSizeMultiplier / 2;
        destination.z = zPosition;
        transform.position = destination;

        var shapeCache = particles.shape;
        float sizeToRate = particles.emission.rateOverTime.constant / shapeCache.scale.x;
        shapeCache.scale = new Vector3(Mathf.Abs(camCenterToMin.x) * screenToEmitterRatio, 1, 1);

        var emissionCache = particles.emission;
        emissionCache.rateOverTime = shapeCache.scale.x * sizeToRate;
    }
}
