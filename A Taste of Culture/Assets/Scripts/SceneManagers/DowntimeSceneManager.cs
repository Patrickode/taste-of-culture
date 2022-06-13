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
    public GameObject mentor;

    public GameObject[] backgrounds;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        dialogue.SetActive(true);

        foreach (var bg in backgrounds)
            bg.SetActive(false);
        backgrounds[backgrounds.Length - 1].SetActive(true);

        AddOnions();
        CookOnions();
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
        mentor.SetActive(false);
        StartCoroutine(TakeOffLid());
        StartCoroutine(ButterInPot());
        StartCoroutine(BackgroundToSchool());
    }

    IEnumerator ButterInPot()
    {
        yield return new WaitForSeconds(2.0f);
        backgrounds[6].SetActive(true);
    }

    public void AddProtein()
    {
        backgrounds[11].SetActive(false);
        backgrounds[7].SetActive(true);
        mentor.SetActive(false);
        StartCoroutine(TakeOffLid());
        StartCoroutine(ProteinInPot());
        StartCoroutine(BackgroundToSchool());
    }

    IEnumerator ProteinInPot()
    {
        yield return new WaitForSeconds(1.0f);
        backgrounds[8].SetActive(true);
    }

    public void CheckOnCurry()
    {
        backgrounds[11].SetActive(false);
        backgrounds[8].SetActive(true);
        backgrounds[9].SetActive(true);
        StartCoroutine(TakeOffLid());
    }

    public void PlateCurry()
    {
        StartCoroutine(ShowPlate());
    }

    IEnumerator ShowPlate()
    {
        mentor.SetActive(false);
        backgrounds[10].SetActive(true);
        yield return new WaitForSeconds(4.5f);
        backgrounds[10].SetActive(false);
        backgrounds[11].SetActive(true);
        mentor.SetActive(true);
    }

    IEnumerator PutOnLid()
    {
        yield return new WaitForSeconds(1.0f);
        backgrounds[9].SetActive(true);
    }

    IEnumerator TakeOffLid()
    {
        yield return new WaitForSeconds(1.0f);
        backgrounds[9].SetActive(false);
    }

    IEnumerator BackgroundToSchool()
    {
        yield return new WaitForSeconds(3.5f);
        backgrounds[11].SetActive(true);
        mentor.SetActive(true);
    }
    IEnumerator BackgroundToDish()
    {
        yield return new WaitForSeconds(1.0f);
        backgrounds[10].SetActive(true);
        mentor.SetActive(false);
    }

    public void DialogueEnded()
    {
        Cursor.visible = false;
        dialogue.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}