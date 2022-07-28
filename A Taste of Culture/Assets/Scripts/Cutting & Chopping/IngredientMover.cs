using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientMover : MonoBehaviour
{
    [SerializeField] float movementDistance = 2f;
    [SerializeField] float budgeDistance = 0.25f;
    [SerializeField] float budgeDuration = 0.125f;
    [Space(5)]

    [Tooltip("X Position at which player can rotate ingredient")]
    [SerializeField] float rotateXPosition;
    [Tooltip("X Position that triggers transition")]
    [SerializeField] float finalXPosition;
    [Space(5)]

    [Tooltip("Desired position of ingredient after it has been rotated")]
    [SerializeField] float rotateAmount = 90;
    [SerializeField] Vector2 rotatedPosition;
    [Space(5)]

    [SerializeField] GameObject choppedPrefab;
    [SerializeField] Vector2 choppedPrefabPosition;
    [SerializeField] Vector3 choppedPrefabRotation;
    [Space(5)]

    [SerializeField] GameObject spriteMask;
    [SerializeField] Vector2 spriteMaskPosition;

    // Allow player to move ingredient.
    public bool AllowMovement { get; set; }

    // Allow player to rotate ingredient.
    public bool AllowRotation { private get; set; }

    [HideInInspector] public DoubleIngredient doublIngrParent;

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
        //On move input, if this mover actually moves (has non-negative, non-zero move distance), try to move.
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            && movementDistance > Mathf.Epsilon)
            DoMoveOrBudge();

        if (taskComplete)
            return;

        TryRotateIngredient();

        if (!AllowRotation && (transform.position.x >= rotateXPosition))
        {
            AllowRotation = true;

            // Switch to showing rotation instructions
            InstructionTooltips tooltips = FindObjectOfType<InstructionTooltips>();
            if (tooltips != null) { tooltips.ToggleRotationInstructions(); }
        }

        TryBroadcastTaskCompletion();
    }

    void DoMoveOrBudge()
    {
        Debug.Log($"<color=#777>IngMover: Allow Movement = {AllowMovement}</color>");

        // Only allow movement if a cut has been made. Ingredient cutter class enables allowMovement after cut is made.
        if (AllowMovement)
        {
            transform.position += Vector3.right * movementDistance;
            cachedIngrPosition = transform.position;

            AllowMovement = false;
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
        if (!Input.GetKeyDown(KeyCode.Space) || !AllowRotation)
            return;

        // transform.position = originalPosition;
        // cachedIngrPosition = originalPosition;

        Debug.Log("Rotating " + gameObject.name);

        transform.position = rotatedPosition;
        cachedIngrPosition = rotatedPosition;

        if (spriteMask)
            mask = Instantiate(spriteMask, spriteMaskPosition, Quaternion.identity);

        // Instantiate chunks under current ingredient (will become visible when ingredient moves into mask)
        if (choppedPrefab)
        {
            Transform choppedIngredient = Instantiate(choppedPrefab, choppedPrefabPosition, Quaternion.identity).transform;
            choppedIngredient.Rotate(choppedPrefabRotation);      // Manually change to desired prefab rotation
            choppedIngredient.parent = transform;
        }

        transform.Rotate(0, 0, rotateAmount);

        AllowRotation = false;
        hasBeenRotated = true;

        InstructionTooltips tooltips = FindObjectOfType<InstructionTooltips>();
        if (tooltips != null) { tooltips.ResetInstructions(); }
    }

    // Disable knife interaction and inform scene controller that the task has been completed.
    void TryBroadcastTaskCompletion()
    {
        if (!hasBeenRotated || transform.position.x < finalXPosition) return;

        taskComplete = true;

        // If ingredient is a double ingredient, make sure other ingredient is finished chopping before task completion
        if (doublIngrParent)
        {
            doublIngrParent.FinishedChopping(gameObject);
            doublIngrParent.masks.Add(mask);

            if (!doublIngrParent.DualChoppingComplete) return;
        }

        mask.transform.localScale += Vector3.right + Vector3.up;

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

            Debug.Log("Task Complete!");
            sceneManager.TaskComplete();
        }
    }
}
