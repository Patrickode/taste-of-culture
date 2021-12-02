using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueContainer dialogue;
    public DialogueManager dialogueManager;

    public void StartDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }
}