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
    public GameObject dialogue;
    public DialogueTrigger trigger;
    public int nextSceneIndex;

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
            #region Deprecated
            case DialogueTime.Closing:
                SceneManager.LoadScene("IntroDialogue");
                break;
            #endregion

            case DialogueTime.Opening:
                Transitions.LoadWithTransition(nextSceneIndex, -1);
                break;
        }
    }
}