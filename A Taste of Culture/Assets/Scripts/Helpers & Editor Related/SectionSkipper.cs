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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
