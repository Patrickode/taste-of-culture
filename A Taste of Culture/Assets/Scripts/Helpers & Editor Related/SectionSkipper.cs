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
        keystrokeKeysDown[0] = Input.GetKey(KeyCode.S);
        keystrokeKeysDown[1] = Input.GetKey(KeyCode.K);
        keystrokeKeysDown[2] = Input.GetKey(KeyCode.I);
        keystrokeKeysDown[3] = Input.GetKey(KeyCode.P);

        //I is ignored because SKI (and thus SKIP) in particular doesn't work. Why? God only knows.
        if (useSkipKeystroke
            && keystrokeKeysDown[0]
            && keystrokeKeysDown[1]
            //&& keystrokeKeysDown[2]
            && keystrokeKeysDown[3])
        {
            SkipSection();
        }
    }

    void SkipSection()
    {
        int nextScIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScIndex < SceneManager.sceneCountInBuildSettings && nextScIndex >= 0)
        {
            Debug.Log($"<color=#FFF200>Skipping current scene of index {nextScIndex - 1}; loading scene at index {nextScIndex}.</color>");
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