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
    [SerializeField] private GameObject flavorPObj;

    public GameObject[] backgrounds;
    //private FlavorProfile flavorPfile;

    private void Start()
    {
        //flavorPfile = backgrounds[10].GetComponentInChildren<FlavorProfile>();
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

        Coroutilities.DoAfterYielders(this, () => dialogueManager.ToggleContinue(true),
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
        dialogueManager.ToggleContinue(false);
        dialogueManager.ToggleDialogue(false);
        Coroutilities.DoAfterYielder(this, ActuallyAddProtein, StartCoroutine(TransitionAndWait(false, 2)));

        void ActuallyAddProtein()
        {
            backgrounds[11].SetActive(false);
            backgrounds[7].SetActive(true);
            mentor.SetActive(false);

            Coroutilities.DoAfterYielders(this, () => { dialogueManager.ToggleDialogue(true); dialogueManager.ToggleContinue(true); },
                StartCoroutine(TakeOffLid()),
                StartCoroutine(ProteinInPot()),
                StartCoroutine(BackgroundToSchool(2)));
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
            mentor.SetActive(false);
            backgrounds[11].SetActive(false);
            backgrounds[8].SetActive(true);
            backgrounds[9].SetActive(true);
        };

        dialogueManager.ToggleContinue(false);
        dialogueManager.ToggleDialogue(false);

        Coroutilities.DoAfterSequence(this, () => { dialogueManager.ToggleDialogue(true); dialogueManager.ToggleContinue(true); },
            () => StartCoroutine(TransitionAndWait(false, 2)),
            () => Coroutilities.DoAfterDelayFrames(this, bgSwitch, 0),
            () => StartCoroutine(TakeOffLid()),
            () => new WaitForSeconds(1));
    }

    public void PlateCurry()
    {
        dialogueManager.ToggleDialogue(false);
        dialogueManager.ToggleContinue(false);

        Coroutilities.DoAfterYielder(this, () => { dialogueManager.ToggleDialogue(true); dialogueManager.ToggleContinue(true); },
            StartCoroutine(ShowPlate()));
    }

    IEnumerator ShowPlate()
    {
        mentor.SetActive(false);
        yield return StartCoroutine(TransitionAndWait(false, 2));

        backgrounds[10].SetActive(true);
        /*if (flavorPfile)
            flavorPfile.VisualizeFlavors();*/
        flavorPObj.SafeSetActive(true);

        dialogueManager.ToggleDialogue(true);

        yield return new WaitForSeconds(4.5f);
        yield return StartCoroutine(TransitionAndWait(false, 2));

        backgrounds[10].SetActive(false);
        flavorPObj.SafeSetActive(false);
        
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
        yield return StartCoroutine(TransitionAndWait(false, 2));

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
        dialogueManager.animator.gameObject.SetActive(false);
    }

    public void RecipeDismissed()
    {
        Transitions.LoadWithTransition(0, -1);
    }
}