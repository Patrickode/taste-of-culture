using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingSceneManager : MonoBehaviour
{
    public GameObject knife;
    public CookingDialogueManager dialogueManager;
    public CookingDialogueTrigger dialogueTrigger;
    public GameObject dialogue;
    public GifManager gifManager;
    public string dialogueString;

    // Start is called before the first frame update
    void Start()
    {
        dialogue.SetActive(true);
        knife.SetActive(false);
        dialogueString = "intro";
        StartCoroutine(TriggerIntro());
    }

    IEnumerator TriggerIntro()
    {
        yield return new WaitForSeconds(0.5f);
        dialogueTrigger.TriggerDialogue();
    }

    public void IntroEnded()
    {
        dialogue.SetActive(false);
        // knife.SetActive(true);
        gifManager.StartVideo();
    }

    public void GifEnded()
    {
        knife.SetActive(true);
        Cursor.visible = false;
    }

    public void CutOutsideMargins()
    {
        knife.SetActive(false);
        Cursor.visible = true;
        dialogue.SetActive(true);
        dialogueString = "error";
        dialogueTrigger.TriggerDialogue();
    }

    public void MarginsEnded()
    {
        dialogue.SetActive(false);
        knife.SetActive(true);
        Cursor.visible = false;
    }

    public void FinishedCutting()
    {
        dialogue.SetActive(true);
        knife.SetActive(false);
        Cursor.visible = true;
        dialogueTrigger = gameObject.GetComponents<CookingDialogueTrigger>()[1];
        dialogueTrigger.TriggerDialogue();
        GameObject.Find("ContinueButton").SetActive(false);
    }
}
