using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingSceneManager : MonoBehaviour
{
    public GameObject knife;
    public GameObject introDialogue;

    // Start is called before the first frame update
    void Start()
    {
        introDialogue.SetActive(true);
    }

    public void IntroEnded()
    {
        introDialogue.SetActive(false);
        knife.SetActive(true);
    }
}
