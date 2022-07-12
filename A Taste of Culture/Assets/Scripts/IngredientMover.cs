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

    [SerializeField]
    [Tooltip("X Position at which player can rotate ingredient")]
    float rotateXPosition;
    [SerializeField]
    [Tooltip("X Position that triggers transition")]
    float finalXPosition;
    [Space(5)]

    [SerializeField]
    [Tooltip("Desired position of ingredient after it has been rotated")]
    Vector2 rotatedPosition;
    [Space(5)]

    [SerializeField] GameObject choppedPrefab;
    [SerializeField] Vector2 choppedPrefabPosition;
    [SerializeField] Vector3 choppedPrefabRotation;
    [Space(5)]

    [SerializeField] GameObject spriteMask;
    [SerializeField] Vector2 spriteMaskPosition;

    // Allow player to move ingredient.
    private bool allowMovement = false;
    public bool AllowMovement { set { allowMovement = value; } get { return allowMovement; } }

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
        Debug.Log($"<color=#777>IngMover: Allow Movement = {allowMovement}</color>");

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

        // transform.position = originalPosition;
        // cachedIngrPosition = originalPosition;

        transform.position = rotatedPosition;
        cachedIngrPosition = rotatedPosition;

        // Instantiate mask that will allow chunks to become visible
        mask = Instantiate(spriteMask, spriteMaskPosition, Quaternion.identity);

        // Instantiate chunks under current ingredient (will become visible when ingredient moves into mask)
        GameObject choppedIngredient = Instantiate(choppedPrefab, choppedPrefabPosition, Quaternion.identity);
        choppedIngredient.transform.Rotate(choppedPrefabRotation);      // Manually change to desired prefab rotation
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

        // TODO: Replace sceneControllers in level 1 scene and delete this code
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

        ChoppingSceneManager sceneManager = FindObjectOfType<ChoppingSceneManager>();

        if (sceneManager != null)
        {
            // Disable knife chop if last ingredient
            if (sceneManager.bAtLastIngredient)
            {
                ChoppingKnife knife = FindObjectOfType<ChoppingKnife>();
                if (knife != null) { knife.CanChop = false; }
            }

            sceneManager.TaskComplete();
        }
    }
}
