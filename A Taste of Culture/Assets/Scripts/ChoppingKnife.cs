using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChoppingKnife : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;                           // Layer to detect colliders on.
    
    public Animator animator;

    Vector2 knifeTip;
    Vector2 knifeBase;

    private AudioSource choppingAudio;

    private bool canChop = true;
    public bool CanChop { set { canChop = value; } }

    private bool madeFirstCut = false;

    // Start is called before the first frame update
    void Start()
    {
        knifeTip = gameObject.transform.GetChild(0).gameObject.transform.position;
        knifeTip = gameObject.transform.GetChild(1).gameObject.transform.position;
        
        choppingAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && canChop)
        {
            // this will turn the knife "down" on click
            animator.SetBool("Click", true);

            StartCoroutine(Chop());
            choppingAudio.Play();
        }
    }

    IEnumerator Chop()
    {
        yield return new WaitForSeconds(0.1f);

        List<GameObject> objectsToCut = new List<GameObject>();
        List<GameObject> Ingredients = new List<GameObject>();

        // Get an array of all ingredients that were hit. 
        RaycastHit2D[] hitObjects = Physics2D.LinecastAll(knifeTip, knifeBase, layerMask);       

        foreach(RaycastHit2D hitObject in hitObjects)
        {
            if(hitObject.transform.gameObject.CompareTag("Double Ingredient"))
            {
                Ingredients.Add(hitObject.transform.gameObject.GetComponent<DualIngredientHandler>().Ingredients[0]);
                Ingredients.Add(hitObject.transform.gameObject.GetComponent<DualIngredientHandler>().Ingredients[1]);
            }

            // Ensure that hitObject isn't the child of a double ingredient object 
            // since it would have been added to the ingredient list when its parent was hit
            else if(!(hitObject.transform.parent.transform.parent.transform.gameObject.CompareTag("Double Ingredient"))) 
            {
                Ingredients.Add(hitObject.transform.parent.gameObject); 
            }
        }

        foreach(GameObject ingredient in Ingredients)
        {
            if(!madeFirstCut) { madeFirstCut = true; }
            else 
            {
                IngredientMover ingredientMover = ingredient.GetComponent<IngredientMover>();
                if(ingredientMover.AllowMovement) { continue; }
            }

            objectsToCut.Add(ingredient.transform.GetChild(0).gameObject);
        }

        // foreach(RaycastHit2D hitObject in hitObjects)
        // {
        //     if(!madeFirstCut) { madeFirstCut = true; }
        //     else 
        //     {
        //         IngredientMover ingredientMover = hitObject.transform.parent.gameObject.GetComponent<IngredientMover>();
        //         if(ingredientMover.AllowMovement) { continue; }
        //     }
            
        //     objectsToCut.Add(hitObject.transform.gameObject);
        // }    

        foreach(GameObject objectToCut in objectsToCut)
        {
            objectToCut.GetComponent<IngredientCutter>().CutIngredient(knifeTip, objectToCut);
        }  

        animator.SetBool("Click", false);
    }
}
