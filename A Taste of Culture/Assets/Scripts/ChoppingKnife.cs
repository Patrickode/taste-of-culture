using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChoppingKnife : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;                           // Layer to detect colliders on.
    
    public Animator animator;

    Vector2 knifeEdge;

    private AudioSource choppingAudio;

    private bool canChop = true;
    public bool CanChop { set { canChop = value; } }

    private bool madeFirstCut = false;

    // Start is called before the first frame update
    void Start()
    {
        knifeEdge = gameObject.transform.GetChild(0).gameObject.transform.position;
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

        // Get an array of all ingredients that were hit. 
        RaycastHit2D[] hitObjects = Physics2D.LinecastAll(knifeEdge, knifeEdge, layerMask);       

        foreach(RaycastHit2D hitObject in hitObjects)
        {
            // Check if a cut mask should be instantiated (avoids having serval cut masks on the same "cut")
            if(!madeFirstCut) { madeFirstCut = true; }
            else 
            {
                IngredientMover ingredientMover = hitObject.transform.parent.gameObject.GetComponent<IngredientMover>();
                if(ingredientMover.AllowMovement) { continue; }
            }
            
            objectsToCut.Add(hitObject.transform.gameObject);
        }    

        foreach(GameObject objectToCut in objectsToCut)
        {
            objectToCut.GetComponent<IngredientCutter>().CutIngredient(knifeEdge, objectToCut);
        }  

        animator.SetBool("Click", false);
    }
}
