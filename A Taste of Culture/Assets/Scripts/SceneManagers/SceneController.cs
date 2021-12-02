using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] string nextScene;
    public CookingSceneManager sceneManager;
    // [SerializeField] float applauseDelay = 0.5f;
    // [SerializeField] float sceneTransitionDelay = 3f;
    public StringVariable protein;
    public GameEvent choseChicken;
    public GameEvent choseTofu;
    public enum Veggie { Onion, Tomato };

    [SerializeField] private Veggie currentVeggie;
    public Veggie CurrentVeggie { get { return currentVeggie; } }

    // Will only be referenced if in chopping scene.
    GameObject onion;
    GameObject tomato;
    GameObject onionInstruction;
    GameObject tomatoInstruction;

    void Awake() 
    {
        // If ingredient is onion, then grab onion and tomato gameobjects to be referenced on task completion
        if(currentVeggie == Veggie.Onion)
        {
            onion = GameObject.Find("Onion");
            tomato = GameObject.Find("Tomato");

            onionInstruction = GameObject.Find("Onion Instruction");
            tomatoInstruction = GameObject.Find("Tomato Instruction");
        }

        if (protein.value == "chicken")
        {
            choseChicken.Raise();
        }
        else if (protein.value == "tofu")
        {
            choseTofu.Raise();
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        // If ingredient is onion, then disable the tomato gameobject
        if(currentVeggie == Veggie.Onion)
        {
            if(tomato != null) { tomato.SetActive(false); }
            if(tomatoInstruction != null) { tomatoInstruction.SetActive(false); }
        }
    }

    public void TaskComplete()
    {
        StartCoroutine(CompleteTask());
    }

    IEnumerator CompleteTask()
    {
        yield return new WaitForSeconds(0.5f);

        // If current ingredient is Onion, disable it and enable tomato
        if(currentVeggie == Veggie.Onion)
        {
            yield return new WaitForSeconds(1f);
            if(onion != null) { onion.SetActive(false); }
            if(onionInstruction != null) { onionInstruction.SetActive(false); }

            GameObject spriteMask = GameObject.Find("Chunks Sprite Mask(Clone)");
            if(spriteMask != null) { GameObject.Destroy(spriteMask); }

            if(tomato != null) 
            { 
                // Debug.Log("Found Tomato!");
                tomato.SetActive(true);
                currentVeggie = Veggie.Tomato;
            }

            if(tomatoInstruction != null) { tomatoInstruction.SetActive(true); }

            // Reset instruction tooltip so that movement instructions will be toggled again
            InstructionTooltips tooltips = FindObjectOfType<InstructionTooltips>();
            if(tooltips != null) { tooltips.ResetInstructions(); }

            yield break;
        }

        else
        {
            sceneManager.FinishedCutting();
            yield return new WaitForSeconds(5f);
            
            // Load next scene
            if(nextScene != null) 
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}
