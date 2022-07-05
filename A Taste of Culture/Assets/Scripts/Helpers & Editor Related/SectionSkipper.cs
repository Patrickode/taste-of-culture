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
        if (useSkipKeystroke
            && Input.GetKey(KeyCode.S)
            && Input.GetKey(KeyCode.K)
            && Input.GetKey(KeyCode.I)
            && Input.GetKeyDown(KeyCode.P))
        {
            SkipSection();
        }
    }

    void SkipSection()
    {
        int nextScIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScIndex < SceneManager.sceneCountInBuildSettings || nextScIndex < 0)
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