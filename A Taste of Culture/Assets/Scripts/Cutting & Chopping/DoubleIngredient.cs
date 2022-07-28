using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleIngredient : MonoBehaviour
{
    [SerializeField] private GameObject ingr1Ref = null;
    [SerializeField] private GameObject ingr2Ref = null;
    [SerializeField] private IngredientMover mover = null;

    public GameObject Ingredient1 { get => ingr1Ref; private set => ingr1Ref = value; }
    public GameObject Ingredient2 { get => ingr2Ref; private set => ingr2Ref = value; }
    public IngredientMover Mover { get => mover; }

    public bool DualChoppingComplete { get; private set; }
    public bool MaskInstantiated { get; set; }

    [HideInInspector] public List<GameObject> masks = new List<GameObject>();

    int completionCount = 0;

    void Awake()
    {
        Ingredient1 = Ingredient1 ? Ingredient1 : transform.GetChild(0).gameObject;
        Ingredient2 = Ingredient2 ? Ingredient2 : transform.GetChild(1).gameObject;

        /*Ingredient1.GetComponent<IngredientMover>().doublIngrParent = this;
        Ingredient2.GetComponent<IngredientMover>().doublIngrParent = this;*/
    }

    public void FinishedChopping(GameObject ingredient)
    {
        Debug.Log(gameObject.name + " finished chopping");
        completionCount++;
        if (completionCount == 2)
        {
            DualChoppingComplete = true;
            foreach (GameObject mask in masks)
                Destroy(mask);
        }
    }
}
