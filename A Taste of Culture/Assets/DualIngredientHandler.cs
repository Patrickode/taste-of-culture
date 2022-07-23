using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualIngredientHandler : MonoBehaviour
{
    private GameObject[] ingredients = new GameObject[2]; 
    public GameObject[] Ingredients { get { return ingredients; } }

    private bool dualChoppingComplete = false;
    public bool DualChoppingComplete { get { return dualChoppingComplete; } }

    public bool maskInstantiated = false;

    public List<GameObject> masks = new List<GameObject>();

    int completionCount = 0;

    void Awake() 
    {
        ingredients[0] = gameObject.transform.GetChild(0).gameObject;
        ingredients[1] = gameObject.transform.GetChild(1).gameObject;

        ingredients[0].GetComponent<IngredientMover>().IsDoubleIngredient = true;
        ingredients[1].GetComponent<IngredientMover>().IsDoubleIngredient = true;
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

    // Ensure that both ingredients were cut
    public void EnsureDualCut(GameObject cutIngredient, Vector2 cutLocation)
    {
        GameObject otherIngredient;

        if(cutIngredient == ingredients[0]) { otherIngredient = ingredients[1]; }
        else { otherIngredient = ingredients[0]; }

        otherIngredient.GetComponent<IngredientCutter>().CutIngredient(cutLocation, otherIngredient);
    }

}
