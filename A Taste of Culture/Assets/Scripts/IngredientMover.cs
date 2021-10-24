using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class IngredientMover : MonoBehaviour
{
    [SerializeField] float movementDistance = 2f;
    [SerializeField] GameObject choppedPrefab;
    [SerializeField] Vector2 choppedPosition;
    [SerializeField] GameObject spriteMask;
    [SerializeField] Vector2 spriteMaskPosition;
    
    Rigidbody2D rigidbodyComponent;

    Vector2 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody2D>();

        originalPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 ingredientPosition = gameObject.transform.position;

        // TODO: Restrict player control so that ingredient can only move once cut is made
        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ingredientPosition.x += movementDistance;
            gameObject.transform.position = ingredientPosition;
        }

        // TODO: Restrict player control & remove ability to move ingredient backwards
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ingredientPosition.x -= movementDistance;
            gameObject.transform.position = ingredientPosition;
        }

        // TODO: 
        if(Input.GetKeyDown(KeyCode.Space)) { RotateIngredient(); }
    }

    // Rotate ingredient and reset it's position
    void RotateIngredient()
    {
        gameObject.transform.position = originalPosition;

        // Instantiate that will allow chunks to become visible
        GameObject mask = Instantiate(spriteMask, spriteMaskPosition, Quaternion.identity);

        // Instantiate chunks under current ingredient (will become visible when ingredient moves into mask)
        GameObject choppedIngredient = Instantiate(choppedPrefab, choppedPosition, Quaternion.identity);
        choppedIngredient.transform.parent = gameObject.transform;

        gameObject.transform.Rotate(0, 0, 90);
    }
}
