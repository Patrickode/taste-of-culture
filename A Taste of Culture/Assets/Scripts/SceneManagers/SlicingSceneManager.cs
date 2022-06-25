using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicingSceneManager : BaseIngredientSceneManager
{
    public StringVariable protein;
    public GameEvent choseChicken;
    public GameEvent choseTofu;

    GameObject currentProtein;

    void Awake() 
    {
        string selectedProtein = "";
        string otherProtein = "";   // Used to deactivate alternative protein in cases where player is given a choice in protein

        if (protein.value == "chicken")
        {
            choseChicken.Raise();

            selectedProtein = "Raw Chicken";
            otherProtein = "Tofu Block";
        }
        else if (protein.value == "tofu")
        {
            choseTofu.Raise();

            selectedProtein = "Tofu Block";
            otherProtein = "Raw Chicken";
        }
        else { selectedProtein = "Conch"; }

        ActivateProtein(selectedProtein, otherProtein);
    }
    
    void ActivateProtein(string selectedProtein, string otherProtein)
    {
        currentProtein = GameObject.Find(selectedProtein);
        // if(currentProtein != null) { currentProtein.SetActive(true); } 

        if(otherProtein != "")
        {
            GameObject proteinToDeactivate = GameObject.Find(otherProtein);
            if (proteinToDeactivate != null) { proteinToDeactivate.SetActive(false); }
        }

        CuttingKnife knife = FindObjectOfType<CuttingKnife>();
        if (knife != null && currentProtein != null)
        {
            knife.collider1 = currentProtein.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<IngredientCollider>();
            knife.collider2 = currentProtein.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<IngredientCollider>();
        }
    }

    protected override IEnumerator CompleteTask()
    {
        base.CompleteTask();
        base.HandleSceneCompletion();

        yield break;
    }
}
