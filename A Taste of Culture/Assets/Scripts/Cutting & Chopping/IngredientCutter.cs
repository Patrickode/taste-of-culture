using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCutter : MonoBehaviour
{
    [SerializeField] GameObject spriteMask;
    [SerializeField] float cutWidth = 0.25f;

    [Tooltip("Should be set to true for cuttable ingredients and false for choppable ingredients.")]
    [SerializeField] bool isCuttable;

    private Vector2 ingredientPosition;

    BoxCollider2D collRef;
    SpriteRenderer rendRef;

    CutGuideline guideline;

    IngredientMover ingredientMover;

    Quaternion cutRotation = Quaternion.identity;
    public Quaternion CutRotation { set { cutRotation = value; } }

    void Awake()
    {
        if (!isCuttable)
            //The singular version of this throws a compiler error that there is no overload with one argument.
            //...Even though there is in both the documentation and GitHub source code? yeah ok sure whatever
            ingredientMover = GetComponentsInParent<IngredientMover>(true)[0];
    }

    void Start()
    {
        collRef = GetComponent<BoxCollider2D>();
        rendRef = GetComponent<SpriteRenderer>();

        if (isCuttable) { guideline = GetComponent<CutGuideline>(); }
    }

    // public void CutIngredient(Vector2 cutStart, Vector2 cutEnd, GameObject ingredient)
    public void CutIngredient(Vector2 cutCenter, Quaternion cutRotation = default)
    {
        ingredientPosition = gameObject.transform.position;

        // Get reference to collider component's bounds to help with sizing info.
        Bounds sizeRefBounds = collRef ? collRef.bounds : UtilFunctions.GetBoundsSansPadding(rendRef);

        // Draw sprite mask to make it look like a cut was made.
        CutRotation = cutRotation != default ? cutRotation : Quaternion.identity;
        RepresentCut(new Vector2(cutCenter.x, sizeRefBounds.center.y), sizeRefBounds.size.y + 1f);

        // If the ingredient can be moved, allow movement after cut is made.
        if (ingredientMover)
            ingredientMover.AllowMovement = true;
    }

    void ResizeCollider(GameObject colliderOwner, Vector2 colliderSize, float colliderXOffset)
    {
        var coll = colliderOwner.GetComponent<BoxCollider2D>();
        coll.size = colliderSize;
        coll.offset = new Vector2(colliderXOffset, coll.offset.y);
    }

    bool RepresentCut(Vector2 position, float verticalScale)
    {
        if (!spriteMask)
        {
            Debug.Log("ERROR: No Sprite Mask Found.");

            return false;
        }

        // Find old guideline and destroy it when new cut is made.
        GameObject oldGuideline = GameObject.FindGameObjectWithTag("Guideline");
        if (oldGuideline != null) { Destroy(oldGuideline); }

        GameObject mask = Instantiate(spriteMask, position, cutRotation);
        mask.transform.localScale = new Vector2(cutWidth, verticalScale);
        mask.transform.parent = gameObject.transform.parent;
        mask.SetActive(true);

        if (isCuttable) { guideline.DrawNewGuideline(position.x); }

        return true;
    }
}
