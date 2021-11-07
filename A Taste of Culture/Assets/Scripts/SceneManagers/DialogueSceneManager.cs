using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSceneManager : MonoBehaviour
{
    public FullscreenDialogueManager dialogueManager;
    public FullscreenDialogueTrigger dialogueTrigger;
    public GameObject dialogue;

    // Start is called before the first frame update
    void Start()
    {
        dialogue.SetActive(true);
        StartCoroutine(TriggerDialogue());
    }

    IEnumerator TriggerDialogue()
    {
        yield return new WaitForSeconds(0.5f);
        dialogueTrigger.TriggerDialogue();
    }

    public void DialogueEnded()
    {
        dialogue.SetActive(false);
        Cursor.visible = false;
    }
}
