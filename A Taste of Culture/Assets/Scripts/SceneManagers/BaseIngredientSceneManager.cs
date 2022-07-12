using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseIngredientSceneManager : MonoBehaviour
{
    [SerializeField] int nextSceneIndex = -1;
    [SerializeField] CookingSceneManager sceneManager;
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

        StartCoroutine(TransitionToNewScene());
    }

    protected IEnumerator TransitionToNewScene()
    {
        sceneManager.FinishedSliceOrSpice();

        yield return new WaitForSeconds(5f);

        Transitions.LoadWithTransition?.Invoke(nextSceneIndex, -1);
    }
}
