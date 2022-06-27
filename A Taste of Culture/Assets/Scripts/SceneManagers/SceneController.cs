using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] int nextSceneIndex = -1;
    public CookingSceneManager sceneManager;
    // [SerializeField] float applauseDelay = 0.5f;
    // [SerializeField] float sceneTransitionDelay = 3f;
    public StringVariable protein;
    public GameEvent choseChicken;
    public GameEvent choseTofu;

    public enum Ingredient { Protein, Onion, Tomato, Spices };

    [SerializeField] private Ingredient currentIngredient;
    public Ingredient CurrentIngredient { get { return currentIngredient; } }

    // public DataHolder dataHolder;

    // Will only be referenced if in chopping scene.
    GameObject onion;
    GameObject tomato;
    GameObject onionInstruction;
    GameObject tomatoInstruction;

    GameObject currentProtein;

    void Awake()
    {
        // If ingredient is onion, then grab onion and tomato gameobjects to be referenced on task completion
        if (currentIngredient == Ingredient.Onion)
        {
            onion = GameObject.Find("Onion");
            tomato = GameObject.Find("Tomato");

            onionInstruction = GameObject.Find("Onion Instruction");
            tomatoInstruction = GameObject.Find("Tomato Instruction");
        }

        if (protein.value == "chicken")
        {
            choseChicken.Raise();

            ActivateProtein("Raw Chicken", "Tofu Block");
        }
        else if (protein.value == "tofu")
        {
            choseTofu.Raise();

            ActivateProtein("Tofu Block", "Raw Chicken");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // If ingredient is onion, then disable the tomato gameobject
        if (currentIngredient == Ingredient.Onion)
        {
            if (tomato != null) { tomato.SetActive(false); }
            if (tomatoInstruction != null) { tomatoInstruction.SetActive(false); }
        }
    }

    void ActivateProtein(string selectedProtein, string otherProtein)
    {
        currentProtein = GameObject.Find(selectedProtein);
        // if(currentProtein != null) { currentProtein.SetActive(true); } 

        GameObject proteinToDeactivate = GameObject.Find(otherProtein);
        if (proteinToDeactivate != null) { proteinToDeactivate.SetActive(false); }

        CuttingKnife knife = FindObjectOfType<CuttingKnife>();
        if (knife != null && currentProtein != null)
        {
            knife.collider1 = currentProtein.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<IngredientCollider>();
            knife.collider2 = currentProtein.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<IngredientCollider>();
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
        if (currentIngredient == Ingredient.Onion)
        {
            yield return new WaitForSeconds(1f);
            if (onion != null) { onion.SetActive(false); }
            if (onionInstruction != null) { onionInstruction.SetActive(false); }

            GameObject spriteMask = GameObject.Find("Chunks Sprite Mask(Clone)");
            if (spriteMask != null) { GameObject.Destroy(spriteMask); }

            if (tomato != null)
            {
                // Debug.Log("Found Tomato!");
                tomato.SetActive(true);
                currentIngredient = Ingredient.Tomato;
            }

            if (tomatoInstruction != null) { tomatoInstruction.SetActive(true); }

            yield break;
        }

        else
        {
            sceneManager.FinishedSliceOrSpice();
            // if(currentIngredient == Ingredient.Chicken || currentIngredient == Ingredient.Tofu)
            // {
            //     sceneManager.GetComponent<CookingSceneManager>().FinishedCutting();
            // }
            // else 
            // { 
            //     /// TODO: Disable hand in spice selection...
            //     Debug.Log("Done selecting spices");
            // }
            yield return new WaitForSeconds(5f);

            Transitions.LoadWithTransition?.Invoke(nextSceneIndex, -1);
        }
    }
}
