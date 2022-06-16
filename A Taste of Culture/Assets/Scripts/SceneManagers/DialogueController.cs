using DialogueEditor;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private NPCConversation initConversation;

    void Start()
    {
        TriggerConversation(initConversation);
    }

    private void TriggerConversation(NPCConversation conversation)
    {
        ConversationManager.Instance.StartConversation(conversation);
    }
}
