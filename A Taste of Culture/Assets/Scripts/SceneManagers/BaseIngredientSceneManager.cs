using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseIngredientSceneManager : MonoBehaviour
{
    [Header("Base Ingr Scene Manager Fields")]
    [SerializeField] DialogueEditor.NPCConversation endConvo = null;
    [SerializeField] bool endConvoTriggersTransition = false;
    [SerializeField] int nextSceneIndex = -1;
    //[SerializeField] CookingSceneManager sceneManager;
    // [SerializeField] float sceneTransitionDelay = 3f;
    // [SerializeField] float applauseDelay = 0.5f;

    public void TaskComplete()
    {
        Debug.Log($"<color=#777>BaseIngSceneManager \"{name}\": Task Complete!</color>");

        StartCoroutine(CompleteTask());
    }

    protected virtual IEnumerator CompleteTask()
    {
        yield return new WaitForSeconds(0.5f);
    }

    protected void HandleSceneCompletion()
    {
        Debug.Log("<color=#777>BaseIngSceneManager \"{name}\": Scene Complete!</color>");

        if (endConvo)
        {
            DialogueEditor.ConversationManager.Instance.StartConversation(endConvo);
            if (endConvoTriggersTransition) return;
        }

        StartCoroutine(QueueTransition());
    }

    protected IEnumerator QueueTransition(float delayTime = 5f)
    {
        //sceneManager.FinishedSliceOrSpice();

        yield return new WaitForSeconds(delayTime);

        TransitionToNextScene();
    }

    public void TransitionToNextScene(float speed) => Transitions.LoadWithTransition?.Invoke(nextSceneIndex, speed);
    public void TransitionToNextScene() => TransitionToNextScene(-1);
}
