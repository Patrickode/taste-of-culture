using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class IngredientMover : MonoBehaviour
{
    [SerializeField] float movementDistance = 2f;
    [SerializeField] float rotateXPosition;
    [SerializeField] float finalXPosition;
    [SerializeField] GameObject choppedPrefab;
    [SerializeField] Vector2 choppedPosition;
    [SerializeField] GameObject spriteMask;
    [SerializeField] Vector2 spriteMaskPosition;

    // Allow player to move ingredient.
    private bool allowMovement = false;
    public bool AllowMovement { set { allowMovement = value; } }

    // Allow player to rotate ingredient.
    private bool allowRotation = false;
    public bool AllowRotation { set { allowRotation = value; } }

    Rigidbody2D rigidbodyComponent;
    Vector2 originalPosition;

    bool hasBeenRotated = false;
    bool taskComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody2D>();

        originalPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(taskComplete) { return; }
        Vector2 ingredientPosition = gameObject.transform.position;

        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Only allow movement if a cut has been made. Ingredient cutter class enables allowMovement after cut is made.
            if(allowMovement)
            {
                ingredientPosition.x += movementDistance;
                gameObject.transform.position = ingredientPosition;

                allowMovement = false;
            }
            else
            { 
                // TODO: Prompt mentor to tell player to make a cut before moving ingredient.
                Debug.Log("You need to make a cut before moving the ingredient!");
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Space)) 
        { 
            if(allowRotation) { RotateIngredient(); }
        }

        if(!allowRotation && (gameObject.transform.position.x >= rotateXPosition)) { allowRotation = true; }

        if(hasBeenRotated && (gameObject.transform.position.x >= finalXPosition)) { BroadcastTaskCompletion(); }
    }

    // Rotate ingredient and reset it's position
    void RotateIngredient()
    {
        gameObject.transform.position = originalPosition;

        // Instantiate mask that will allow chunks to become visible
        GameObject mask = Instantiate(spriteMask, spriteMaskPosition, Quaternion.identity);

        // Instantiate chunks under current ingredient (will become visible when ingredient moves into mask)
        GameObject choppedIngredient = Instantiate(choppedPrefab, choppedPosition, Quaternion.identity);
        choppedIngredient.transform.parent = gameObject.transform;

        gameObject.transform.Rotate(0, 0, 90);

        allowRotation = false;
        hasBeenRotated = true;
    }

    // Disable knife interaction and inform scene controller that the task has been completed.
    void BroadcastTaskCompletion()
    {
        taskComplete = true;

        SceneController sceneController = FindObjectOfType<SceneController>();
        
        if(sceneController != null) 
        { 
            if(sceneController.CurrentIngredient == SceneController.Ingredient.Tomato)
            {
                ChoppingKnife knife = FindObjectOfType<ChoppingKnife>();
                if(knife != null) { knife.CanChop = false; }
            }

            sceneController.TaskComplete();
        }
    }
}
