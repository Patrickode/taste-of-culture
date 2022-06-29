using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SectionSkipper : Singleton<SectionSkipper>
{
    [SerializeField] Button skipButton;

#if UNITY_EDITOR
    void Start()
    {
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true);
            skipButton.onClick.AddListener(SkipSection);
        }
    }
#endif

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
}
