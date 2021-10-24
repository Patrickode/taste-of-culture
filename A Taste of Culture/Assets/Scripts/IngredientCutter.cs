using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCutter : MonoBehaviour
{
    [SerializeField] GameObject ingredientPrefab;
    [SerializeField] GameObject spriteMask;
    [SerializeField] float cutWidth = 0.25f;

    [Tooltip("Should be set to true for cuttable ingredients and false for choppable ingredients.")]
    [SerializeField] bool isCuttable;
    
    private Vector2 ingredientPosition;

    BoxCollider2D colliderComponent;

    CutGuideline guideline;

    void Start() 
    {
        colliderComponent = GetComponent<BoxCollider2D>();
        
        if(isCuttable) { guideline = GetComponent<CutGuideline>(); }
    }

    // public void CutIngredient(Vector2 cutStart, Vector2 cutEnd, GameObject ingredient)
    public void CutIngredient(Vector2 cutCenter, GameObject ingredient)
    {
        ingredientPosition = gameObject.transform.position;

        // Find the center of the line.
        Vector2 center = cutCenter;
        Debug.Log("Cut Center: " + center);

        // Get reference to collider component's bounds to help with sizing info.
        UnityEngine.Bounds colliderBounds = colliderComponent.bounds;

        // Draw sprite mask to make it look like a cut was made.
        bool cutDrawnSuccessfully = RepresentCut(new Vector2(center.x, 0), colliderBounds.size.y + 1f);

        if(isCuttable)
        {
            // Find size & position of original (left) ingredient piece.
            float leftHalfScale = Vector2.Distance(colliderBounds.min, center) - 1f;
            Debug.Log("LeftHalfScale: " + leftHalfScale);
            Vector2 leftHalfPosition = new Vector2(colliderBounds.min.x + (center.x - colliderBounds.min.x) * 0.5f, ingredientPosition.y);
            Debug.Log("LeftHalfPosition: " + leftHalfPosition);

            // Find size & position of new (right) ingredient piece.
            float rightHalfScale = Vector2.Distance(colliderBounds.max, center) - 1f;
            Debug.Log("RightHalfScale: " + rightHalfScale);
            Vector2 rightHalfPosition = new Vector2(colliderBounds.max.x + (center.x - colliderBounds.max.x) * 0.5f, ingredientPosition.y);
            Debug.Log("RightHalfPosition: " + rightHalfPosition);
        
            if(cutDrawnSuccessfully)
            {
                // Resize the collider of original (left) piece.
                ResizeCollider(this.gameObject, new Vector2(leftHalfScale, colliderBounds.size.y), leftHalfPosition.x);

                // Create new (right) ingredient piece.
                GameObject newPiece = Instantiate(ingredientPrefab, ingredientPosition, Quaternion.identity);
                newPiece.transform.parent = gameObject.transform.parent; 
                newPiece.GetComponent<CutGuideline>().drawInitialGuideline = false;

                // Resize the collider of new (right) piece.
                ResizeCollider(newPiece, new Vector2(rightHalfScale, colliderBounds.size.y), rightHalfPosition.x);
            }
        }

    }

    void ResizeCollider(GameObject colliderOwner, Vector2 colliderSize, float colliderXOffset)
    {
        colliderOwner.GetComponent<BoxCollider2D>().size = colliderSize;
        colliderOwner.GetComponent<BoxCollider2D>().offset = new Vector2(colliderXOffset, colliderOwner.GetComponent<BoxCollider2D>().offset.y);
    }

    bool RepresentCut(Vector2 position, float verticalScale)
    { 
        if(spriteMask == null) 
        { 
            Debug.Log("ERROR: No Sprite Mask Found.");

            return false; 
        }

        // Find old guideline and destroy it when new cut is made.
        GameObject oldGuideline = GameObject.FindGameObjectWithTag("Guideline");
        if(oldGuideline != null) { Destroy(oldGuideline); }

        GameObject mask = Instantiate(spriteMask, position, Quaternion.identity);
        mask.transform.localScale = new Vector2(cutWidth, verticalScale);
        mask.transform.parent = gameObject.transform.parent;
        mask.SetActive(true);

        if(isCuttable) { guideline.DrawNewGuideline(position.x); }
        
        return true;
    }
}
