using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DialogueEditor;

public class DowntimeSceneManager : MonoBehaviour
{
    public GameObject[] backgrounds;
    private FlavorProfile flavorPfile;
    [SerializeField] private StringVariable protein;

    void Start()
    {
        flavorPfile = backgrounds[10].GetComponentInChildren<FlavorProfile>();

        foreach (var bg in backgrounds)
            bg.SetActive(false);
        backgrounds[backgrounds.Length - 1].SetActive(true);
        ConversationManager.Instance.SetBool("UsedChicken", protein.name.Equals("chicken"));
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

//<<<<<<< dialogue-overhaul
        //Coroutilities.DoAfterYielder(this, () => StartCoroutine(TakeOffLid()),
          // StartCoroutine(ButterInPot()),
           //StartCoroutine(BackgroundToSchool()));
//=======
        Coroutilities.DoAfterYielders(this, () => dialogueManager.ToggleContinue(true),
            StartCoroutine(TakeOffLid()),
            StartCoroutine(ButterInPot()),
            StartCoroutine(BackgroundToSchool()));
//>>>>>>> dev
    }

    IEnumerator ButterInPot()
    {
        yield return new WaitForSeconds(2.0f);
        backgrounds[6].SetActive(true);
    }

    public void AddProtein()
    {
        Coroutilities.DoAfterYielder(this, ActuallyAddProtein, StartCoroutine(TransitionAndWait(false, 2)));

        void ActuallyAddProtein()
        {
            backgrounds[11].SetActive(false);
            backgrounds[7].SetActive(true);

//<<<<<<< dialogue-overhaul
           // Coroutilities.DoAfterYielder(this, () => StartCoroutine(TakeOffLid()),
//=======
            Coroutilities.DoAfterYielders(this, () => { dialogueManager.ToggleDialogue(true); dialogueManager.ToggleContinue(true); },
                StartCoroutine(TakeOffLid()),
//>>>>>>> dev
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
            backgrounds[11].SetActive(false);
            backgrounds[8].SetActive(true);
            backgrounds[9].SetActive(true);
        };

        Coroutilities.DoAfterSequence(this, () => StartCoroutine(TransitionAndWait(false, 2)),
            () => Coroutilities.DoAfterDelayFrames(this, bgSwitch, 0),
            () => StartCoroutine(TakeOffLid()),
            () => new WaitForSeconds(1));
    }

    public void PlateCurry()
    {
    StartCoroutine(ShowPlate());
    }

    IEnumerator ShowPlate()
    {
        yield return StartCoroutine(TransitionAndWait(false, 2));
        backgrounds[10].SetActive(true);
        if (flavorPfile)
            flavorPfile.VisualizeFlavors();


        yield return new WaitForSeconds(4.5f);

        yield return StartCoroutine(TransitionAndWait(false, 2));
        backgrounds[10].SetActive(false);
        backgrounds[11].SetActive(true);
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
    }

    IEnumerator TransitionAndWait(bool pauseOnMid, float speed)
    {
        Transitions.StartTransition(pauseOnMid, speed);
        bool transitionDone = false;
        Transitions.MidTransition += OnMid;
        void OnMid(bool _) { Transitions.MidTransition -= OnMid; transitionDone = true; }
        yield return new WaitUntil(() => transitionDone);
    }
}