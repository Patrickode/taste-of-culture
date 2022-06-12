using UnityEngine;
using DialogueEditor;

public class testcontroller : MonoBehaviour
{
    public NPCConversation test;    

    // Start is called before the first frame update
    void Start()
    {
        ConversationManager.Instance.StartConversation(test);
    }
}
