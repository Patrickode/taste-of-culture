using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CuttingKnife : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;                           // Layer to detect colliders on.
    public IngredientCollider collider1;
    public IngredientCollider collider2;

    public Animator animator;

    private AudioSource sliceAudio;

    Rigidbody2D rigidbodyComponent;
    //IngredientCutter ingredientCutter;
    LineRenderer lineRenderer;

    Vector2 cutStartPosition;
    Vector2 cutEndPosition;

    private bool canChop = true;
    public bool CanChop { set { canChop = value; } }

    [SerializeField] NPCConversation outsideLinesDialogue;

    // Start is called before the first frame update
    void Start()
    {
        // Cursor.visible = false;

        rigidbodyComponent = GetComponent<Rigidbody2D>();
        //ingredientCutter = GetComponent<IngredientCutter>();
        lineRenderer = GetComponent<LineRenderer>();

        sliceAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Only allow for knife movement and cutting if knife can chop.
        if (!canChop) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rigidbodyComponent.position = mousePosition;               // Make knife move according to mouse location.

        //Don't allow cutting while the player's being warned about uneven cuts (mostly to prevent minor visual jank).
        if (outsideLinesDialogue.ConversationActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            cutStartPosition = mousePosition;

            // Check if player is cutting within margin of error
            if (CutWithinMargin(cutStartPosition))
            {
                lineRenderer.enabled = true;                        // Start drawing line at initial "cut location".
                lineRenderer.SetPosition(0, cutStartPosition);

                // this will turn the knife "down" on click
                animator.SetBool("Click", true);
            }
        }
        //If the line renderer isn't enabled, the above must've failed, so we don't need to do the following
        else if (Input.GetMouseButtonUp(0) && lineRenderer.enabled)
        {
            cutEndPosition = mousePosition;

            // Check if player has made cut within margin of error
            if (CutWithinMargin(cutEndPosition) && CutWithinMargin(cutStartPosition))
            {
                lineRenderer.enabled = false;

                Vector2 cutdirection = cutEndPosition - cutStartPosition;

                // if cut is started from the bottom, flip the x difference
                if (cutStartPosition.y < cutEndPosition.y) { cutdirection.x *= -1; }

                if (collider1.HasCollided && collider2.HasCollided)
                    CutObjects(cutStartPosition, cutEndPosition, Quaternion.Euler(0, 0, cutdirection.x * 10));

                collider1.ResetCollider();
                collider2.ResetCollider();

                sliceAudio.Play();
            }

            // this will turn the knife "up" on release"
            animator.SetBool("Click", false);
        }

        if (Input.GetMouseButton(0) && lineRenderer.enabled)
        {
            lineRenderer.SetPosition(1, mousePosition);             // Continue drawing line to current location of mouse.
        }
    }

    void CutObjects(Vector2 startPosition, Vector2 endPosition, Quaternion cutRotation)
    {
        List<GameObject> objectsToCut = new List<GameObject>();

        // Get an array of all ingredients that were hit. 
        RaycastHit2D[] hitObjects = Physics2D.LinecastAll(startPosition, endPosition, layerMask);

        foreach (RaycastHit2D hitObject in hitObjects)
        {
            objectsToCut.Add(hitObject.transform.gameObject);
        }

        foreach (GameObject objectToCut in objectsToCut)
        {
            Vector2 cutCenter = startPosition + (endPosition - startPosition) * 0.5f;
            objectToCut.GetComponent<IngredientCutter>().CutIngredient(cutCenter, cutRotation);
        }
    }

    bool CutWithinMargin(Vector2 cutPostion)
    {
        GameObject guideline = GameObject.FindGameObjectWithTag("Guideline");
        if (guideline == null) { return false; }

        Vector2 guidelinePosition = guideline.transform.position;

        float margin = FindObjectOfType<CutGuideline>().MarginOfError;

        if ((cutPostion.x > guidelinePosition.x + margin) || (cutPostion.x < guidelinePosition.x - margin))
        {
            ConversationManager.Instance.StartConversation(outsideLinesDialogue);

            lineRenderer.enabled = false;
            return false;
        }

        return true;
    }
}