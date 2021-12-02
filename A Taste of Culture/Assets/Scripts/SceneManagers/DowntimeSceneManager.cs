using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DowntimeSceneManager : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject dialogue;
    public DialogueTrigger trigger;

    public GameObject[] backgrounds;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        dialogue.SetActive(true);
        backgrounds[11].SetActive(true);
        trigger.StartDialogue();
    }

    public void AddOnions()
    {
        backgrounds[11].SetActive(false);
        backgrounds[0].SetActive(true);
        backgrounds[1].SetActive(true);
        backgrounds[2].SetActive(true);
    }

    public void CookOnions()
    {
        backgrounds[2].SetActive(false);
        backgrounds[3].SetActive(true);
    }

    public void AddTomato()
    {
        backgrounds[3].SetActive(false);
        backgrounds[4].SetActive(true);
        StartCoroutine(PutOnLid());
    }

    public void AddButter()
    {
        backgrounds[4].SetActive(false);
        backgrounds[5].SetActive(true);
        StartCoroutine(TakeOffLid());
        StartCoroutine(ButterInPot());
        StartCoroutine(BackgroundToSchool());
    }

    IEnumerator ButterInPot()
    {
        yield return new WaitForSeconds(1.0f);
        backgrounds[6].SetActive(true);
    }

    public void AddProtein()
    {
        backgrounds[11].SetActive(false);
        backgrounds[7].SetActive(true);
        StartCoroutine(TakeOffLid());
        StartCoroutine(ProteinInPot());
    }

    IEnumerator ProteinInPot()
    {
        yield return new WaitForSeconds(1.0f);
        backgrounds[8].SetActive(true);
    }

    IEnumerator PutOnLid()
    {
        yield return new WaitForSeconds(0.5f);
        backgrounds[9].SetActive(true);
    }

    IEnumerator TakeOffLid()
    {
        yield return new WaitForSeconds(0.5f);
        backgrounds[9].SetActive(false);
    }

    IEnumerator BackgroundToSchool()
    {
        yield return new WaitForSeconds(2.0f);
        backgrounds[11].SetActive(true);
    }
    IEnumerator BackgroundToDish()
    {
        yield return new WaitForSeconds(1.0f);
        backgrounds[10].SetActive(true);
    }

    public void DialogueEnded()
    {
        Cursor.visible = false;
        dialogue.SetActive(false);
        //switch (time)
        //{
        //    case DialogueTime.Closing:
        //        SceneManager.LoadScene("IntroDialogue");
        //        break;

        //    case DialogueTime.Opening:
        //        SceneManager.LoadScene("Slicing");
        //        break;
        //}
    }
}