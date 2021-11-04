using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingSceneManager : MonoBehaviour
{
    public GameObject knife;
    public GameObject dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager.SetActive(true);
    }

    public void IntroEnded()
    {
        dialogueManager.SetActive(false);
        knife.SetActive(true);
    }
}
