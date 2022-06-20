using DialogueEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private NPCConversation initConversation;
    [SerializeField]
    private string nextScene;

    private Protein protein;

    public enum Protein { chicken, tofu };

    void Start()
    {
        TriggerConversation(initConversation);
    }

    private void TriggerConversation(NPCConversation conversation)
    {
        ConversationManager.Instance.StartConversation(conversation);
        ConversationManager.OnConversationEnded += ConversationEnd;
    }

    public Protein getProtein()
    {
        return protein;
    }

    public void setProtein(int numCode)
    {
        switch (numCode)
        {
            case 0:
                protein = Protein.chicken;
                break;
            case 1:
                protein = Protein.tofu;
                break;
        }
    }

    private void ConversationEnd()
    {
        SceneManager.LoadScene(nextScene);
    }
}
