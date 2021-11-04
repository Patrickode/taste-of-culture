using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuttingSceneManager : MonoBehaviour
{
    public GameObject knife;
    public DialogueManager dialogueManager;
    public DialogueTrigger dialogueTrigger;
    public GameObject dialogue;
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
        knife.SetActive(true);
        Cursor.visible = false;
    }

    public void CutOutsideMargins()
    {
        knife.SetActive(false);
        Cursor.visible = true;
        dialogue.SetActive(true);
        dialogueString = "margins";
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

    }
}
