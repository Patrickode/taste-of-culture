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
    private FlavorProfile flavorPfile;

    // Start is called before the first frame update
    void Start()
    {
        flavorPfile = backgrounds[10].GetComponentInChildren<FlavorProfile>();
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
        dialogueManager.ToggleContinue(false);

        Coroutilities.DoAfterYielder(this, () => dialogueManager.ToggleContinue(true),
            StartCoroutine(PutOnLid()));
    }

    public void AddButter()
    {
        backgrounds[4].SetActive(false);
        backgrounds[5].SetActive(true);
        mentor.SetActive(false);
        dialogueManager.ToggleContinue(false);

        Coroutilities.DoAfterYielder(this, () => dialogueManager.ToggleContinue(true),
            StartCoroutine(TakeOffLid()),
            StartCoroutine(ButterInPot()),
            StartCoroutine(BackgroundToSchool()));
    }

    IEnumerator ButterInPot()
    {
        yield return new WaitForSeconds(2.0f);
        backgrounds[6].SetActive(true);
    }

    public void AddProtein()
    {
        Coroutilities.DoAfterYielder(this, ActuallyAddProtein, StartCoroutine(TransitionAndWait(false, 2.5f)));

        void ActuallyAddProtein()
        {
            backgrounds[11].SetActive(false);
            backgrounds[7].SetActive(true);
            mentor.SetActive(false);
            Coroutilities.DoAfterDelayFrames(this, () => dialogueManager.ToggleDialogue(false), 1);

            Coroutilities.DoAfterYielder(this, () => dialogueManager.ToggleDialogue(true),
                StartCoroutine(TakeOffLid()),
                StartCoroutine(ProteinInPot()),
                StartCoroutine(BackgroundToSchool(2.5f)));
        }
    }

    IEnumerator ProteinInPot()
    {
        yield return new WaitForSeconds(1.0f);
        backgrounds[8].SetActive(true);
    }

    public void CheckOnCurry()
    {
        System.Action bgSwitch = () =>
        {
            backgrounds[11].SetActive(false);
            backgrounds[8].SetActive(true);
            backgrounds[9].SetActive(true);
            Coroutilities.DoAfterDelayFrames(this, () => dialogueManager.ToggleDialogue(false), 1);
        };

        Coroutilities.DoAfterSequence(this, () => dialogueManager.ToggleDialogue(true),
            () => Coroutilities.DoAfterYielder(this, bgSwitch, StartCoroutine(TransitionAndWait(false, 2.5f))),
            () => StartCoroutine(TakeOffLid()),
            () => new WaitForSeconds(1));
    }

    public void PlateCurry()
    {
        dialogueManager.DialogueUI.SetActive(false);

        Coroutilities.DoAfterYielder(this, () => dialogueManager.ToggleContinue(true),
            StartCoroutine(ShowPlate()));
    }

    IEnumerator ShowPlate()
    {
        mentor.SetActive(false);
        yield return StartCoroutine(TransitionAndWait(false, 2.5f));
        backgrounds[10].SetActive(true);
        if (flavorPfile)
            flavorPfile.VisualizeFlavors();

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

    IEnumerator BackgroundToSchool(float waitLength = 3.5f)
    {
        yield return new WaitForSeconds(waitLength);
        yield return StartCoroutine(TransitionAndWait(false, 2.5f));

        backgrounds[11].SetActive(true);
        mentor.SetActive(true);
    }

    IEnumerator TransitionAndWait(bool pauseOnMid, float speed)
    {
        Transitions.StartTransition(pauseOnMid, speed);
        bool transitionDone = false;
        Transitions.MidTransition += OnMid;
        void OnMid(bool _) { Transitions.MidTransition -= OnMid; transitionDone = true; }
        yield return new WaitUntil(() => transitionDone);
    }

    public void DialogueEnded()
    {
        dialogue.SetActive(false);
    }

    public void RecipeDismissed()
    {
        Transitions.LoadWithTransition(0, -1);
    }
}