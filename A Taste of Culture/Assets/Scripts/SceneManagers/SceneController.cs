using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //[SerializeField] int nextSceneIndex = -1;
    // [SerializeField] float applauseDelay = 0.5f;
    // [SerializeField] float sceneTransitionDelay = 3f;
    //public StringVariable protein;
    //public GameEvent choseChicken;
    //public GameEvent choseTofu;
    public NPCConversation finishedConversation;

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

        //if (protein.value == "chicken")
        if (DataManager.GetLevelData(LevelID.Makhani) is LevelData data
            && (ChoiceFlag.Chicken & data.choices) == ChoiceFlag.Chicken)
        {
            //choseChicken.Raise();

            ActivateProtein("Raw Chicken", "Tofu Block");
        }
        else /*if (protein.value == "tofu")*/
        {
            //choseTofu.Raise();

            ActivateProtein("Tofu Block", "Raw Chicken");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // If ingredient is onion, then disable the tomato gameobject
        if (currentIngredient == Ingredient.Onion)
        {
            if (tomato) { tomato.SetActive(false); }
            if (tomatoInstruction) { tomatoInstruction.SetActive(false); }
        }
    }

    void ActivateProtein(string selectedProtein, string otherProtein)
    {
        currentProtein = GameObject.Find(selectedProtein);
        // if(currentProtein != null) { currentProtein.SetActive(true); } 

        GameObject proteinToDeactivate = GameObject.Find(otherProtein);
        if (proteinToDeactivate) { proteinToDeactivate.SetActive(false); }

        CuttingKnife knife = FindObjectOfType<CuttingKnife>();
        if (knife && currentProtein)
        {
            knife.collider1 = currentProtein.transform.GetChild(0).GetChild(0).GetComponent<IngredientCollider>();
            knife.collider2 = currentProtein.transform.GetChild(0).GetChild(1).GetComponent<IngredientCollider>();
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
            if (onion) { onion.SetActive(false); }
            if (onionInstruction) { onionInstruction.SetActive(false); }

            GameObject spriteMask = GameObject.Find("Chunks Sprite Mask(Clone)");
            if (spriteMask) { Destroy(spriteMask); }

            if (tomato)
            {
                // Debug.Log("Found Tomato!");
                tomato.SetActive(true);
                currentIngredient = Ingredient.Tomato;
            }

            if (tomatoInstruction) { tomatoInstruction.SetActive(true); }

            yield break;
        }

        else
        {
            ConversationManager.Instance.StartConversation(finishedConversation);
        }
    }
}
