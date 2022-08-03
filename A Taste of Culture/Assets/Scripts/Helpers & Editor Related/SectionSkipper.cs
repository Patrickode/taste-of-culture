using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SectionSkipper : Singleton<SectionSkipper>
{
#if UNITY_EDITOR
    [Header("Section Skipper Fields")]
    [SerializeField] Canvas skipCanvas;
    [SerializeField] Button skipButton;
    [Space(5)]
    [SerializeField] private bool useSkipButton;
    [SerializeField] private bool useSkipKeystroke;
    [Space(5)]
    [SerializeField] private bool transitionOnSkip;
    [SerializeField] private float transitionSpeed = -1;

    private bool[] keystrokeKeysDown = { false, false, false, false };

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        if (!Application.isPlaying && skipCanvas && !skipCanvas.worldCamera
            && GameObject.FindGameObjectWithTag("MainCamera").TryGetComponent(out Camera mainCam))
        {
            skipCanvas.worldCamera = mainCam;
        }
    });

    private void Start()
    {
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(useSkipButton);
            skipButton.onClick.AddListener(SkipSection);
        }
    }

    private void Update()
    {
        if (!useSkipKeystroke) return;
        if (transitionOnSkip && Transitions.Transitioning) return;

        keystrokeKeysDown[0] = Input.GetKey(KeyCode.S);
        keystrokeKeysDown[1] = Input.GetKey(KeyCode.K);
        keystrokeKeysDown[2] = Input.GetKey(KeyCode.I);
        keystrokeKeysDown[3] = Input.GetKey(KeyCode.P);

        //I is ignored because SKI (and thus SKIP) in particular doesn't work on the keyboard of
        //the computer Patrick uses in the labs, for hardware reasons (look up keyboard rollover)
        if (keystrokeKeysDown[0]
            && keystrokeKeysDown[1]
            //&& keystrokeKeysDown[2]
            && keystrokeKeysDown[3])
        {
            SkipSection();
        }
    }

    void SkipSection()
    {
        if (transitionOnSkip && Transitions.Transitioning) return;

        int nextScIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScIndex < SceneManager.sceneCountInBuildSettings && nextScIndex >= 0)
        {
            Debug.Log($"<color=#FFF200>Skipping current scene of index {nextScIndex - 1}; loading scene at index {nextScIndex}.</color>");
            if (transitionOnSkip)
            {
                Transitions.LoadWithTransition?.Invoke(nextScIndex, transitionSpeed);
                return;
            }
            SceneManager.LoadScene(nextScIndex);
        }
        else
        {
            Debug.Log($"<color=#FFF200>Section not skipped; Scene index {nextScIndex} is out of range. (If " +
                $"scene {nextScIndex} exists, you might've forgotten to add it to the build settings.)</color>");
        }
    }
#endif
}