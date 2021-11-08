using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class just manages triggering the dialogue
// Attach it to whatever should trigger the dialogue
public class FullscreenDialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<FullscreenDialogueManager>().StartDialogue(dialogue);
    }
}
