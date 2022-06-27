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
    [SerializeField]
    private GameObject tool;

    void Start()
    {
        TriggerConversation(initConversation);
        Cursor.visible = true;
        if (tool != null)
        {
            tool.SetActive(false);
        }
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

    public void ConversationEndEnableControls()
    {
        ConversationManager.OnConversationEnded += EnableControls;
    }

    public void EnableControls()
    {
        Cursor.visible = false;
        if (tool != null)
        {
            tool.SetActive(true);
        }
    }

    public void DisableControls()
    {
        Cursor.visible = true;
        if (tool != null)
        {
            tool.SetActive(false);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
