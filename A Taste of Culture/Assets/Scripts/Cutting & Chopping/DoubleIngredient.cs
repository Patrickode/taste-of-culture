using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleIngredient : MonoBehaviour
{
    private GameObject ingredient1;
    public GameObject Ingredient1 { get { return ingredient1; } }

    private GameObject ingredient2;
    public GameObject Ingredient2 { get { return ingredient2; } }

    private bool dualChoppingComplete = false;
    public bool DualChoppingComplete { get { return dualChoppingComplete; } }

    private bool maskInstantiated = false;
    public bool MaskInstantiated { set { maskInstantiated = value; } get { return maskInstantiated; } }
    
    public List<GameObject> masks = new List<GameObject>();

    int completionCount = 0;

    void Awake() 
    {
        ingredient1 = gameObject.transform.GetChild(0).gameObject;
        ingredient2 = gameObject.transform.GetChild(1).gameObject;

        ingredient1.GetComponent<IngredientMover>().IsDoubleIngredient = true;
        ingredient2.GetComponent<IngredientMover>().IsDoubleIngredient = true;
    }

    public void FinishedChopping(GameObject ingredient)
    {
        Debug.Log(gameObject.name + " finished chopping");
        completionCount++;
        if(completionCount == 2) 
        { 
            dualChoppingComplete = true; 
            foreach(GameObject mask in masks) { GameObject.Destroy(mask); }
        }
    }
}
