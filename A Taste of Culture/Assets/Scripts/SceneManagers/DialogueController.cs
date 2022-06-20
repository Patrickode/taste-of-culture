using DialogueEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private NPCConversation initConversation;
    [SerializeField]
    private string nextScene;
    [SerializeField]
    private StringVariable protein;

    void Start()
    {
        TriggerConversation(initConversation);
    }

    private void TriggerConversation(NPCConversation conversation)
    {
        ConversationManager.Instance.StartConversation(conversation);
    }

    public string getProtein()
    {
        return protein.value;
    }

    public void setProtein(string proteinString)
    {
        protein.SetValue(proteinString);
    }

    public void ConversationEndNextScene()
    {
        ConversationManager.OnConversationEnded += LoadNextScene;
    }

    public void EnableControls(bool enabled)
    {

    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
