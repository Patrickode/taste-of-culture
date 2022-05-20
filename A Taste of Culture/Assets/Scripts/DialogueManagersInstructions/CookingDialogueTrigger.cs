using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class just manages triggering the dialogue
// Attach it to whatever should trigger the dialogue
public class CookingDialogueTrigger : MonoBehaviour
{
    public bool showContinueButton = true;
    public Dialogue dialogue;

    private static CookingDialogueManager cachedManager;
    private CookingDialogueManager DialogueManager
    {
        get
        {
            if (!cachedManager)
            {
                cachedManager = FindObjectOfType<CookingDialogueManager>();
                Debug.Assert(cachedManager, "No CookingDialogueManager found; CookingDialogTriggers need a " +
                    "manager to function properly.");
            }

            return cachedManager;
        }
    }

    public void TriggerDialogue()
    {
        DialogueManager.StartDialogue(dialogue, showContinueButton);
    }

    public void DisableDialogue()
    {
        DialogueManager.EndDialogue();
    }
}