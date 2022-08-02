using DialogueEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private NPCConversation[] convos;
    private int currentConvoIndex = 0;
    [Space(5)]
    [SerializeField] private bool useIndex;
    [SerializeField] private string nextScene;
    [SerializeField] private int nextSceneIndex;
    [Space(5)]
    //[SerializeField] private StringVariable protein;
    [SerializeField] private GameObject tool;

    public static System.Action<bool> ToggleControls;

    void Start()
    {
        TriggerConversation(convos[0]);
        Cursor.visible = true;
        tool.SafeSetActive(false);

        ConversationManager.OnConversationEnded += OnAnyConvoEnd;
        DowntimeSceneManager.AnimFinished += TriggerNextConvo;

        ToggleControls += OnToggleControls;
    }
    private void OnDestroy()
    {
        ConversationManager.OnConversationEnded -= OnAnyConvoEnd;
        DowntimeSceneManager.AnimFinished -= TriggerNextConvo;

        ConversationManager.OnConversationEnded -= LoadNextScene;
        ConversationManager.OnConversationEnded -= EnableControls;

        ToggleControls -= OnToggleControls;
    }

    private void OnAnyConvoEnd()
    {
        currentConvoIndex++;
    }

    private void OnToggleControls(bool enable)
    {
        if (enable)
            EnableControls();
        else
            DisableControls();
    }

    public void TriggerConversation(NPCConversation conversation)
    {
        ConversationManager.Instance.StartConversation(conversation);
    }

    public void TriggerNextConvo()
    {
        if (currentConvoIndex < convos.Length)
            TriggerConversation(convos[currentConvoIndex]);

        else Debug.LogWarning($"Tried to start next convo @ index {currentConvoIndex}, " +
                $"but that index is out of range! (convos.Length = {convos.Length})");
    }

    /*public string getProtein()
    {
        return protein.value;
    }

    public void setProtein(string proteinString)
    {
        protein.SetValue(proteinString);
    }*/

    public void ConversationEndNextScene()
    {
        ConversationManager.OnConversationEnded += LoadNextScene;
    }

    public void ConversationEndEnableControls()
    {
        ConversationManager.OnConversationEnded += EnableControls;
    }

    public void SaveChoice_Tofu() => DataManager.SaveChoice(DataManager.ScnIndToLvlID(SceneManager.GetActiveScene()), ChoiceFlag.Tofu);
    public void SaveChoice_Chicken() => DataManager.SaveChoice(DataManager.ScnIndToLvlID(SceneManager.GetActiveScene()), ChoiceFlag.Chicken);

    public void EvtOnConversationEnd(UnityEngine.Events.UnityEvent evt)
    {
        ConversationManager.OnConversationEnded += unityToSystemEvt;

        void unityToSystemEvt()
        {
            ConversationManager.OnConversationEnded -= unityToSystemEvt;
            evt?.Invoke();
        }
    }

    public void EnableControls()
    {
        //Only enable controls on the first conversation/when explicitly resubscribed
        ConversationManager.OnConversationEnded -= EnableControls;

        Cursor.visible = false;
        tool.SafeSetActive(true);
    }

    public void DisableControls()
    {
        Cursor.visible = true;
        tool.SafeSetActive(false);
    }

    public void LoadNextScene()
    {
        if (useIndex)
        {
            Transitions.LoadWithTransition?.Invoke(nextSceneIndex, -1);
            return;
        }

        Transitions.LoadWithTransition?.Invoke(SceneManager.GetSceneByName(nextScene).buildIndex, -1);
    }
}
