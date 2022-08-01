using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RemovalManager : MonoBehaviour
{
    public RemovalTool current;
    [SerializeField] private RemovalTool hand;
    [SerializeField] private RemovalTool hammer;
    [SerializeField] private RemovalTool knife;
    public static RemovalManager Instance;
    [Space(5)]
    [SerializeField] private GameObject[] knifeStartObjects;
    [SerializeField] private GameObject[] knifeStopObjects;
    [SerializeField] private GameObject[] hammerStopObjects;
    [Space(5)]
    [SerializeField] private GameObject startingShellObject;
    [SerializeField] private GameObject flippedShellObject;
    [Space(5)]
    [Tooltip("Uses `Transitions.LoadWithTransition`, so negative indices = relative; -2 = 2 ahead.")]
    [SerializeField] private int NextSceneIndex;

    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject conchAnimal;
    [SerializeField] private GameObject conchEndingHitbox;
    private int toolSequenceIndex;
    private float conchStartDistance;
    [SerializeField] private GameObject handText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        current = hand;
        toolSequenceIndex = 0;
        conchStartDistance = (conchAnimal.transform.position - conchEndingHitbox.transform.position).sqrMagnitude;
    }

    public void Update()
    {
        UpdateProgress();
    }

    public void WarnPlayerHammer()
    {
        Debug.Log("Being used Wrong");
    }

    public void ResetCurrentTool()
    {
        if (current.name == "Hammer")
        {
            // place in hamer position
            hammer.Active = false;
            ((RemovalHammer)hammer).ChangeSprite(false);
            hammer.ResetPosition();
        }
        else if (current.name == "Knife")
        {
            // place in knife position
            knife.ResetPosition();
            knife.Active = false;
        }

        current = hand;
        hand.Active = true;
    }

    public void ResetHand()
    {
        hand.Active = false;
        hand.ResetPosition();
    }

    public void SetHammer()
    {
        hammer.Active = true;
        current = hammer;
        ((RemovalHammer)hammer).ChangeSprite();

        ResetHand();
    }

    public void SetKnife()
    {
        knife.Active = true;
        current = knife;

        ResetHand();
    }

    public void StartKnifePlay()
    {
        toolSequenceIndex++;

        // turns on the stuff for knife stuff
        foreach (GameObject go in knifeStartObjects)
        {
            go.SetActive(true);
        }

        // turns off stuff for the hammer stuff
        foreach (GameObject go in hammerStopObjects)
        {
            go.SetActive(false);
        }
    }

    public void StartHandPlay()
    {
        toolSequenceIndex++;

        // turns off the stuff for knife stuff
        foreach (GameObject go in knifeStopObjects)
        {
            go.SetActive(false);
        }

        // turn on hand text
        handText.SetActive(true);

        // turn off entire unflipped shell
        startingShellObject.SetActive(false);

        // turn on flipped shell
        flippedShellObject.SetActive(true);
    }

    public void MoveToNextScene()
    {
        Transitions.LoadWithTransition?.Invoke(NextSceneIndex, -1);
    }

    public void UpdateProgress()
    {
        if (toolSequenceIndex == 0)
        {
            progressSlider.value = ((RemovalHammer)hammer).GetProgress();
        }
        else if (toolSequenceIndex == 1)
        {
            progressSlider.value = ((RemovalKnife)knife).GetProgress();
        }
        else if (toolSequenceIndex == 2)
        {
            progressSlider.value = (conchAnimal.transform.position - conchEndingHitbox.transform.position).sqrMagnitude / conchStartDistance;
        }
    }
}
