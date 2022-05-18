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

    GameObject mask;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody2D>();

        originalPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (taskComplete) { return; }
        Vector2 ingredientPosition = gameObject.transform.position;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Only allow movement if a cut has been made. Ingredient cutter class enables allowMovement after cut is made.
            if (allowMovement)
            {
                ingredientPosition.x += movementDistance;
                gameObject.transform.position = ingredientPosition;

                allowMovement = false;
            }
            else
            {
                // TODO: Prompt mentor to tell player to make a cut before moving ingredient.
                Debug.Log("IngredientMover: Attempted to make a cut before moving the ingredient" +
                    "\n<color=#FDCE2A>TODO:</color> Prompt mentor to tell player about making cuts before moving ingredients?");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (allowRotation) { RotateIngredient(); }
        }

        if (!allowRotation && (gameObject.transform.position.x >= rotateXPosition))
        {
            allowRotation = true;

            // Switch to showing rotation instructions
            InstructionTooltips tooltips = FindObjectOfType<InstructionTooltips>();
            if (tooltips != null) { tooltips.ToggleRotationInstructions(); }
        }

        if (hasBeenRotated && (gameObject.transform.position.x >= finalXPosition)) { BroadcastTaskCompletion(); }
    }

    // Rotate ingredient and reset it's position
    void RotateIngredient()
    {
        gameObject.transform.position = originalPosition;

        // Instantiate mask that will allow chunks to become visible
        mask = Instantiate(spriteMask, spriteMaskPosition, Quaternion.identity);

        // Instantiate chunks under current ingredient (will become visible when ingredient moves into mask)
        GameObject choppedIngredient = Instantiate(choppedPrefab, choppedPosition, Quaternion.identity);
        choppedIngredient.transform.parent = gameObject.transform;

        gameObject.transform.Rotate(0, 0, 90);

        allowRotation = false;
        hasBeenRotated = true;

        InstructionTooltips tooltips = FindObjectOfType<InstructionTooltips>();
        if (tooltips != null) { tooltips.ResetInstructions(); }
    }

    // Disable knife interaction and inform scene controller that the task has been completed.
    void BroadcastTaskCompletion()
    {
        taskComplete = true;

        Vector3 scale = new Vector3(1, 1, 0);

        mask.transform.localScale += scale;

        SceneController sceneController = FindObjectOfType<SceneController>();

        if (sceneController != null)
        {
            if (sceneController.CurrentIngredient == SceneController.Ingredient.Tomato)
            {
                ChoppingKnife knife = FindObjectOfType<ChoppingKnife>();
                if (knife != null) { knife.CanChop = false; }
            }

            sceneController.TaskComplete();
        }
    }
}
