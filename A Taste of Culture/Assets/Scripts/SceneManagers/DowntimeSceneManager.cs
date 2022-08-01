using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DialogueEditor;

public class DowntimeSceneManager : MonoBehaviour
{
    //[SerializeField] private StringVariable protein;
    [SerializeField] private GameObject flavorProfileObj;
    public GameObject[] backgrounds;

    public static System.Action AnimFinished;

    void Start()
    {
        foreach (var bg in backgrounds)
            bg.SetActive(false);
        backgrounds[backgrounds.Length - 1].SetActive(true);
        
        //ConversationManager.Instance.SetBool("UsedChicken", protein.name.Equals("chicken"));
        ConversationManager.Instance.SetBool("UsedChicken", DataManager.GetLevelData(LevelID.Makhani) is LevelData data 
            && (ChoiceFlag.Chicken & data.choices) == ChoiceFlag.Chicken);

        AddOnions();
        CookOnions();
    }

    private void MarkAnimFinished() => AnimFinished?.Invoke();
    private void MarkAnimFinished(float delay) => Coroutilities.DoAfterDelay(this, MarkAnimFinished, delay);

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

        Coroutilities.DoAfterYielder(this, MarkAnimFinished,
            StartCoroutine(PutOnLid()));
    }

    public void AddButter()
    {
        backgrounds[4].SetActive(false);
        backgrounds[5].SetActive(true);

        Coroutilities.DoAfterYielders(this, MarkAnimFinished,
            StartCoroutine(TakeOffLid()),
            StartCoroutine(ButterInPot()),
            StartCoroutine(BackgroundToSchool()));
    }

    IEnumerator ButterInPot()
    {
        yield return new WaitForSeconds(2.0f);
        backgrounds[6].SetActive(true);
    }

    public void CheckSauce()
    {
        Coroutilities.DoAfterYielder(this, RemoveLid, StartCoroutine(TransitionAndWait(false, 2)));

        void RemoveLid()
        {
            backgrounds[11].SetActive(false);
            backgrounds[7].SetActive(true);

            Coroutilities.DoAfterYielder(this, () => MarkAnimFinished(0.5f),
                StartCoroutine(TakeOffLid()));
        }
    }

    public void AddProtein()
    {
        Coroutilities.DoAfterYielders(this, MarkAnimFinished,
            StartCoroutine(ProteinInPot()),
            StartCoroutine(BackgroundToSchool(2)));
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

        Coroutilities.DoAfterSequence(this, MarkAnimFinished,
            () => StartCoroutine(TransitionAndWait(false, 2)),
            () => Coroutilities.DoAfterDelayFrames(this, bgSwitch, 0),
            () => StartCoroutine(TakeOffLid()),
            () => new WaitForSeconds(1));
    }

    public void PlateCurry()
    {
        Coroutilities.DoAfterYielder(this, MarkAnimFinished, StartCoroutine(ShowPlate()));
    }

    public void ReturnAfterPlating()
    {
        Coroutilities.DoAfterYielder(this,
            () =>
            {
                flavorProfileObj.SafeSetActive(false);
                backgrounds[10].SetActive(false);
                backgrounds[11].SetActive(true);
                MarkAnimFinished();
            },
            StartCoroutine(TransitionAndWait(false, 2)));
    }

    IEnumerator ShowPlate()
    {
        yield return StartCoroutine(TransitionAndWait(false, 2));

        backgrounds[10].SetActive(true);
        flavorProfileObj.SafeSetActive(true);

        yield return new WaitForSeconds(1.5f);

        /*yield return new WaitForSeconds(4.5f);

        yield return StartCoroutine(TransitionAndWait(false, 2));
        backgrounds[10].SetActive(false);
        backgrounds[11].SetActive(true);*/
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