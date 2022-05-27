using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class IngredientMover : MonoBehaviour
{
    [SerializeField] float movementDistance = 2f;
    [SerializeField] float budgeDistance = 0.25f;
    [SerializeField] float budgeDuration = 0.125f;
    [Space(5)]
    [SerializeField] float rotateXPosition;
    [SerializeField] float finalXPosition;
    [Space(5)]
    [SerializeField] GameObject choppedPrefab;
    [SerializeField] Vector2 choppedPosition;
    [Space(5)]
    [SerializeField] GameObject spriteMask;
    [SerializeField] Vector2 spriteMaskPosition;

    // Allow player to move ingredient.
    private bool allowMovement = false;
    public bool AllowMovement { set { allowMovement = value; } }

    // Allow player to rotate ingredient.
    private bool allowRotation = false;
    public bool AllowRotation { set { allowRotation = value; } }

    Vector2 originalPosition;
    Vector2 cachedIngrPosition;

    bool hasBeenRotated = false;
    bool taskComplete = false;

    GameObject mask;

    void Start()
    {
        originalPosition = transform.position;
        cachedIngrPosition = originalPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            DoMoveOrBudge();

        if (taskComplete)
            return;

        TryRotateIngredient();

        if (!allowRotation && (transform.position.x >= rotateXPosition))
        {
            allowRotation = true;

            // Switch to showing rotation instructions
            InstructionTooltips tooltips = FindObjectOfType<InstructionTooltips>();
            if (tooltips != null) { tooltips.ToggleRotationInstructions(); }
        }

        TryBroadcastTaskCompletion();
    }

    void DoMoveOrBudge()
    {
        // Only allow movement if a cut has been made. Ingredient cutter class enables allowMovement after cut is made.
        if (allowMovement)
        {
            transform.position += Vector3.right * movementDistance;
            cachedIngrPosition = transform.position;

            allowMovement = false;
            return;
        }

        //Stop and reset any budges currently going right now, so the player can't mash and move the ingredient when
        //they shouldn't.
        StopAllCoroutines();
        transform.position = cachedIngrPosition;

        float progress = Mathf.Epsilon;
        int direction = 1;
        Vector2 budgePos = cachedIngrPosition + Vector2.right * budgeDistance;
        Coroutilities.DoUntil(this,
            () =>
            {
                //Lerp between the initial position since the last chop and the budge distance.
                progress += Time.deltaTime / (budgeDuration / 2) * direction;
                transform.position = Vector3.Lerp(cachedIngrPosition, budgePos, progress);

                //Once we've lerped from init to budge, switch direction and lerp back again.
                if (progress >= 1)
                    direction = -1;
            },
            () => progress <= 0);
    }

    // Rotate ingredient and reset it's position
    void TryRotateIngredient()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || !allowRotation)
            return;

        transform.position = originalPosition;
        cachedIngrPosition = originalPosition;

        // Instantiate mask that will allow chunks to become visible
        mask = Instantiate(spriteMask, spriteMaskPosition, Quaternion.identity);

        // Instantiate chunks under current ingredient (will become visible when ingredient moves into mask)
        GameObject choppedIngredient = Instantiate(choppedPrefab, choppedPosition, Quaternion.identity);
        choppedIngredient.transform.parent = transform;

        transform.Rotate(0, 0, 90);

        allowRotation = false;
        hasBeenRotated = true;

        InstructionTooltips tooltips = FindObjectOfType<InstructionTooltips>();
        if (tooltips != null) { tooltips.ResetInstructions(); }
    }

    // Disable knife interaction and inform scene controller that the task has been completed.
    void TryBroadcastTaskCompletion()
    {
        if (!hasBeenRotated || transform.position.x < finalXPosition)
            return;

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
