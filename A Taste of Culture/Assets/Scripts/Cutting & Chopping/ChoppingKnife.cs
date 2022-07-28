using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChoppingKnife : MonoBehaviour
{
    [SerializeField] private Transform knifeTip;
    [SerializeField] private Transform knifeBase;
    [Space(5)]
    [UnityEngine.Serialization.FormerlySerializedAs("layerMask")]
    [SerializeField] private LayerMask layersToChop;
    [SerializeField] private float knifeDownDuration = 0.1f;

    public Animator animator;

    private AudioSource choppingAudio;

    private bool canChop = true;
    public bool CanChop { set { canChop = value; } }

    private bool madeFirstCut = false;

    private void Start()
    {
        choppingAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canChop)
        {
            Chop();

            choppingAudio.Play();

            //true = knife is down, false = knife is up
            SetChopAnim(true);
            Coroutilities.DoAfterDelay(this, () => SetChopAnim(false), knifeDownDuration);
        }
    }

    private void Chop()
    {
        List<GameObject> objectsToCut = new List<GameObject>();
        List<GameObject> Ingredients = new List<GameObject>();

        // Get an array of all ingredients that were hit. 
        RaycastHit2D[] hitObjects = Physics2D.LinecastAll(knifeTip.position, knifeBase.position, layersToChop);

        foreach (RaycastHit2D hitObject in hitObjects)
        {
            if (hitObject.transform.CompareTag("Double Ingredient"))
            {
                Ingredients.Add(hitObject.transform.GetComponent<DoubleIngredient>().Ingredient1);
                Ingredients.Add(hitObject.transform.GetComponent<DoubleIngredient>().Ingredient2);
            }

            // Ensure that hitObject isn't the child of a double ingredient object 
            // since it would have been added to the ingredient list when its parent was hit
            else if (!UtilFunctions.CompareTagInParents(hitObject.transform, "Double Ingredient", 2, false))
            {
                Ingredients.Add(hitObject.transform.parent.gameObject);
            }
        }

        foreach (GameObject ingredient in Ingredients)
        {
            if (!madeFirstCut) { madeFirstCut = true; }
            else
            {
                IngredientMover ingredientMover = ingredient.GetComponent<IngredientMover>();
                if (ingredientMover.AllowMovement) { continue; }
            }

            objectsToCut.Add(ingredient.transform.GetChild(0).gameObject);
        }

        foreach (GameObject objectToCut in objectsToCut)
        {
            objectToCut.GetComponent<IngredientCutter>().CutIngredient(knifeTip.position, objectToCut);
        }
    }

    private void SetChopAnim(bool value)
    {
        animator.SetBool("Click", value);
    }
}
