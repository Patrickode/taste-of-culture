using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CuttingKnife : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;                           // Layer to detect colliders on.
    
    public Animator animator;
    public CookingSceneManager sceneManager;
    public CookingDialogueTrigger dialogueTrigger;

    Rigidbody2D rigidbodyComponent;
    IngredientCutter ingredientCutter;
    LineRenderer lineRenderer;

    Vector2 cutStartPosition;
    Vector2 cutEndPosition;

    private bool canChop = true;
    public bool CanChop { set { canChop = value; } }

    // Start is called before the first frame update
    void Start()
    {
        // Cursor.visible = false;

        rigidbodyComponent = GetComponent<Rigidbody2D>();
        ingredientCutter = GetComponent<IngredientCutter>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() 
    {
        // Only allow for knife movement and cutting if knife can chop.
        if(!canChop){ return; }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rigidbodyComponent.position =  mousePosition;               // Make knife move according to mouse location.

        if(Input.GetMouseButtonDown(0))
        {
            cutStartPosition = mousePosition;

            // Check if player is cutting within margin of error
            if(CutWithinMargin(cutStartPosition))
            {
                lineRenderer.enabled = true;                        // Start drawing line at initial "cut location".
                lineRenderer.SetPosition(0, cutStartPosition);
            }

            // this will turn the knife "down" on click
            animator.SetBool("Click", true);
        }

        if(Input.GetMouseButton(0) && lineRenderer.enabled == true)
        {
            lineRenderer.SetPosition(1, mousePosition);             // Continue drawing line to current location of mouse.
        }

        if(Input.GetMouseButtonUp(0))
        {
            cutEndPosition = mousePosition;

            // Check if player has made cut within margin of error
            if(CutWithinMargin(cutEndPosition) && CutWithinMargin(cutStartPosition))
            {
                lineRenderer.enabled = false;

                CutObjects(cutStartPosition, cutEndPosition);
            }

            // this will turn the knife "up" on release"
            animator.SetBool("Click", false);
        }
    } 

    void CutObjects(Vector2 startPosition, Vector2 endPosition)
    {
        List<GameObject> objectsToCut = new List<GameObject>();

        // Get an array of all ingredients that were hit. 
        RaycastHit2D[] hitObjects = Physics2D.LinecastAll(startPosition, endPosition, layerMask);       

        foreach(RaycastHit2D hitObject in hitObjects)
        {
            objectsToCut.Add(hitObject.transform.gameObject);
        }    

        foreach(GameObject objectToCut in objectsToCut)
        {
            Vector2 cutCenter = startPosition + (endPosition - startPosition) * 0.5f;
            objectToCut.GetComponent<IngredientCutter>().CutIngredient(cutCenter, objectToCut);
            // objectToCut.GetComponent<IngredientCutter>().CutIngredient(startPosition, endPosition, objectToCut);
        }
    }

    bool CutWithinMargin(Vector2 cutPostion)
    {
        GameObject guideline = GameObject.FindGameObjectWithTag("Guideline");
        if(guideline == null) { return false; }

        Vector2 guidelinePosition = guideline.transform.position;

        float margin = FindObjectOfType<CutGuideline>().MarginOfError;

        if((cutPostion.x > guidelinePosition.x + margin) || (cutPostion.x < guidelinePosition.x - margin)) 
        {
            //Debug.Log("ERROR: Attempting to cut outside the margin of error");      // TODO: Replace with comment from mentor
            sceneManager.dialogueTrigger = dialogueTrigger;
            sceneManager.CutOutsideMargins();

            lineRenderer.enabled = false;
            return false; 
        }

        return true;
    }
}
