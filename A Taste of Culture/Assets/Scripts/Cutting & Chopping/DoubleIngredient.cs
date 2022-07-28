using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleIngredient : MonoBehaviour
{
    public GameObject Ingredient1 { get; private set; }
    public GameObject Ingredient2 { get; private set; }

    public bool DualChoppingComplete { get; private set; }
    public bool MaskInstantiated { get; set; }

    [HideInInspector] public List<GameObject> masks = new List<GameObject>();

    int completionCount = 0;

    void Awake()
    {
        Ingredient1 = gameObject.transform.GetChild(0).gameObject;
        Ingredient2 = gameObject.transform.GetChild(1).gameObject;

        Ingredient1.GetComponent<IngredientMover>().doublIngrParent = this;
        Ingredient2.GetComponent<IngredientMover>().doublIngrParent = this;
    }

    public void FinishedChopping(GameObject ingredient)
    {
        Debug.Log(gameObject.name + " finished chopping");
        completionCount++;
        if (completionCount == 2)
        {
            DualChoppingComplete = true;
            foreach (GameObject mask in masks) { GameObject.Destroy(mask); }
        }
    }
}
