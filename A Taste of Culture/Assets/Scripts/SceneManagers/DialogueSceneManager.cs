using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum DialogueTime
{
    Opening,
    Closing
}

public class DialogueSceneManager : MonoBehaviour
{
    public DialogueTime time;
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
        switch (time)
        {
            case DialogueTime.Closing:
                SceneManager.LoadScene("IntroDialogue");
                break;

            case DialogueTime.Opening:
                SceneManager.LoadScene("Slicing");
                break;
        }
    }
}
