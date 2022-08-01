using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutGuideline : MonoBehaviour
{
    [SerializeField] private SpriteRenderer guidelinePrefab;
    public bool drawInitialGuideline = true;
    [Space(5)]
    [SerializeField] private float cutWidth;
    [Tooltip("Percentage of cutWidth to compare against the guideline position when determining where the last cut is. " +
        "0 = cutWidth. 1 = cutWidth * 2.")]
    [SerializeField] [Range(0, 1)] private float endCutPadding = 0.6f;
    [SerializeField] private float marginOfError;
    [SerializeField] private bool scaleLineWithMargin;
    [Space(5)]
    [Tooltip("How much to decrement from the guideline image's alpha after each cut.")]
    [SerializeField] [Range(0, 1)] private float alphaDecrement = 0f;
    [Tooltip("If true, will treat alphaDecrement as a percentage to decrease by, rather than an absolute alpha value. " +
        "Note: This assumes that guidelinePrefab's alpha will not change mid-scene by other means.")]
    [SerializeField] private bool relativeDecrement;

    private Collider2D colliderComponent;
    private SpriteRenderer guidelineCache;
    private Vector2 guidelinePosition;

    public float MarginOfError { get { return marginOfError; } }
    public Vector2 GuidelinePosition { get => guidelinePosition; }

    void Start()
    {
        colliderComponent = GetComponent<Collider2D>();

        if (guidelinePrefab && relativeDecrement)
            alphaDecrement = guidelinePrefab.color.a * alphaDecrement;

        endCutPadding = cutWidth * endCutPadding;

        // Set initial starting position (left-most edge of ingredient)
        guidelinePosition.x = colliderComponent.bounds.min.x;
        guidelinePosition.y = 0;

        // Ensures that new pieces don't draw new guidelines on start (no duplicate guidelines being drawn)
        if (drawInitialGuideline) { DrawNewGuideline(guidelinePosition.x); }
    }

    public void DrawNewGuideline(float position)
    {
        guidelinePosition.x = position;
        guidelinePosition.x += cutWidth;

        // Check that drawing new guideline won't ask player to cut a piece that is too small
        if (colliderComponent.bounds.max.x - guidelinePosition.x >= cutWidth + endCutPadding)
        {
            //If we haven't made the guideline yet, make it. Otherwise, move it to where we want and adjust alpha.
            if (!guidelineCache)
                guidelineCache = Instantiate(guidelinePrefab, guidelinePosition, Quaternion.identity);
            else
            {
                guidelineCache.transform.position = guidelinePosition;
                guidelineCache.color = guidelineCache.color.Adjust(3, -alphaDecrement, true);
            }

            if (scaleLineWithMargin)
            {
                //We want X, the width that'll make this rend's unpadded bounds = the margin of error.
                //The formula for that is marginOfError * currentScaleX / unpaddedRadius, via cross-multiplication.
                guidelineCache.transform.localScale = new Vector3(
                    marginOfError * guidelineCache.transform.localScale.x / guidelineCache.GetBoundsSansPadding().extents.x,
                    guidelineCache.transform.localScale.y,
                    guidelineCache.transform.localScale.z);
            }
        }
        // If unable to draw new guideline then task is complete
        else BroadcastTaskCompletion();
    }

    // Disable knife interaction and inform scene controller that the task has been completed.
    void BroadcastTaskCompletion()
    {
        CuttingKnife knife = FindObjectOfType<CuttingKnife>();
        if (knife != null) { knife.CanChop = false; }

        // TODO: Replace sceneControllers in level 1 scene and delete this code
        SceneController sceneController = FindObjectOfType<SceneController>();
        if (sceneController != null) { sceneController.TaskComplete(); }

        BaseIngredientSceneManager sceneManager = FindObjectOfType<BaseIngredientSceneManager>();
        if (sceneManager != null) { sceneManager.TaskComplete(); }
    }
}