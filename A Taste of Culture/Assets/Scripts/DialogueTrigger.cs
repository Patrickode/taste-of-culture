using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class just manages triggering the dialogue
// Attach it to whatever should trigger the dialogue
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
