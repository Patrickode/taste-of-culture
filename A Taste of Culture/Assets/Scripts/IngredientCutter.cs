using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCutter : MonoBehaviour
{
    [SerializeField] GameObject ingredientPrefab;
    [SerializeField] GameObject spriteMask;
    [SerializeField] float cutWidth = 0.25f;
    
    private Vector2 ingredientPosition;

    Collider2D colliderComponent;
    Vector2 colliderSize;

    CutGuideline guideline;

    void Start() 
    {
        colliderComponent = GetComponent<Collider2D>();
        colliderSize = colliderComponent.bounds.size;
        
        ingredientPosition = gameObject.transform.position;

        guideline = GetComponent<CutGuideline>();
    }

    public void CutIngredient(Vector2 cutStart, Vector2 cutEnd, GameObject ingredient)
    {
        // Find the center of the line.
        Vector2 center = cutStart + (cutEnd - cutStart) * 0.5f;

        // Get reference to collider component's bounds to help with sizing info.
        UnityEngine.Bounds colliderBounds = colliderComponent.bounds;

        // Find size & position of original (left) ingredient piece.
        float leftHalfScale = Vector2.Distance(colliderBounds.min, center) - 1f;
        Vector2 leftHalfPosition = new Vector2(colliderBounds.min.x + (center.x - colliderBounds.min.x) * 0.5f, ingredientPosition.y);

        // Find size & position of new (right) ingredient piece.
        float rightHalfScale = Vector2.Distance(colliderBounds.max, center) - 1f;
        Vector2 rightHalfPosition = new Vector2(colliderBounds.max.x + (center.x - colliderBounds.max.x) * 0.5f, ingredientPosition.y);

        // Draw sprite mask to make it look like a cut was made.
        bool cutDrawnSuccessfully = RepresentCut(new Vector2(center.x, 0), colliderBounds.size.y + 1f);

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

        guideline.DrawNewGuideline(position.x);

        return true;
    }
}
