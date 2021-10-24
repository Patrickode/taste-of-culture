/** This was me kind of throwing ideas around of trying to simply 
    create two objects using the original ingredient's prefab but 
    I think I'm just gonna try and implement the sprite cutting.

    We could use this if we wanted and just limit the player to only 
    being able to make vertical cuts and ask Ajay to make one or two 
    more assets for what ingredients would look like cut.
**/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableIngredient : MonoBehaviour
{
    [SerializeField] GameObject tofuPrefab;

    float colliderSize;

    // Start is called before the first frame update
    void Start()
    {
        colliderSize = GetComponent<BoxCollider2D>().size.x;
    }

    public void CutIngredient(Vector2 cutStartPosition, Vector2 cutEndPosition)
    {
        // Hide original ingredient sprite and turn off collision.
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<BoxCollider2D>());  

        // Empty gameobjects present in prefab to help with finding the edge of the tofu block.
        Vector2 leftEdge = transform.Find("Left Edge").position;            
        Vector2 rightEdge = transform.Find("Right Edge").position;

        // Find the center of the line.
        Vector2 center = cutStartPosition + (cutEndPosition - cutStartPosition) * 0.5f;
        // Debug.Log("Center point: " + center);

        // Find "scale" of new pieces. Basically the size of one cut.
        float firstHalfScaleX = Vector2.Distance(leftEdge, center) / colliderSize;
        float firstHalfPosition = leftEdge.x + (center.x - leftEdge.x) * 0.5f;
        CreateHalf(firstHalfPosition, firstHalfScaleX, "Left");

        float secondHalfScaleX = Vector2.Distance(rightEdge, center) / colliderSize;
        float secondHalfPosition = rightEdge.x - (rightEdge.x - center.x) * 0.5f;
        CreateHalf(secondHalfPosition, secondHalfScaleX, "Right");
    }

    void CreateHalf(float position, float scale, string name)
    {
        GameObject halfPiece = Instantiate(tofuPrefab, new Vector2(position, 0), Quaternion.identity);
        halfPiece.transform.localScale = new Vector2(scale, 1);
        // halfPiece.transform.parent = gameObject.transform;              // Parenting to OG gameobject creates bug where second piece is also cut.
        halfPiece.GetComponent<BoxCollider2D>().enabled = true;         // Manually enable boxCollider since parent collider was destroyed.
        halfPiece.name = name;
    }
}
