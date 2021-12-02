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
    public DialogueManager dialogueManager;
    public DialogueTrigger dialogueTrigger;
    public GameObject dialogue;
    public DialogueTrigger trigger;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        dialogue.SetActive(true);
        trigger.StartDialogue();
    }

    public void DialogueEnded()
    {
        Cursor.visible = false;
        dialogue.SetActive(false);
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