using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingSceneManager : MonoBehaviour
{
    [UnityEngine.Serialization.FormerlySerializedAs("knife")] public GameObject cursorObj;
    public CookingDialogueManager dialogueManager;
    public CookingDialogueTrigger dialogueTrigger;
    public GameObject dialogue;
    public GIFManager gifManager;
    public string dialogueString;

    bool gifHasPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        dialogue.SetActive(true);
        cursorObj.SetActive(false);
        dialogueString = "intro";
        StartCoroutine(TriggerIntro());
        Cursor.visible = true;
    }

    IEnumerator TriggerIntro()
    {
        yield return new WaitForSeconds(0.5f);
        dialogueTrigger.TriggerDialogue();
    }

    public void IntroEnded()
    {
        if (gifHasPlayed) { return; }

        dialogue.SetActive(false);
        // knife.SetActive(true);
        gifManager.StartVideo();

        gifHasPlayed = true;
    }

    public void GifEnded()
    {
        cursorObj.SetActive(true);
        Cursor.visible = false;

        SceneController sceneController = FindObjectOfType<SceneController>();
        if (sceneController != null && sceneController.CurrentIngredient == SceneController.Ingredient.Spices)
        {
            dialogue.SetActive(true);
        }
    }

    public void CutOutsideMargins()
    {
        cursorObj.SetActive(false);
        Cursor.visible = true;
        dialogue.SetActive(true);
        dialogueString = "error";
        dialogueTrigger.TriggerDialogue();
    }

    public void MarginsEnded()
    {
        dialogue.SetActive(false);
        cursorObj.SetActive(true);
        Cursor.visible = false;
    }

    public void FinishedSliceOrSpice()
    {
        Debug.Log("CookingSceneManager's FinishedSliceOrSpice called");
        dialogue.SetActive(true);
        cursorObj.SetActive(false);
        Cursor.visible = true;
        
        gameObject.GetComponents<CookingDialogueTrigger>()[1].TriggerDialogue();
        GameObject.Find("ContinueButton").SetActive(false);
    }
}