using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutGuideline : MonoBehaviour
{
    [SerializeField] GameObject guidelinePrefab;
    [SerializeField] float cutWidth;
    [SerializeField] float marginOfError;
    public float MarginOfError { get { return marginOfError; } }

    Collider2D colliderComponent;

    private Vector2 guidelinePosition;
    public Vector2 GuidelinePosition { get { return guidelinePosition; } }

    public bool drawInitialGuideline = true;

    // Start is called before the first frame update
    void Start()
    {
        colliderComponent = GetComponent<Collider2D>();

        // Set initial starting position (left-most edge of ingredient)
        guidelinePosition.x = colliderComponent.bounds.min.x;  
        guidelinePosition.y = 0;

        // Ensures that new pieces don't draw new guidelines on start (no duplicate guidelines being drawn)
        if(drawInitialGuideline) { DrawNewGuideline(guidelinePosition.x); }
    }

    public void DrawNewGuideline(float position)
    {
        guidelinePosition.x = position;
        guidelinePosition.x += cutWidth;

        // Check that drawing new guideline won't ask player to cut a piece that is too small
        if((Vector2.Distance(guidelinePosition, colliderComponent.bounds.max)) >= (cutWidth + cutWidth * 0.6))
        {
            Instantiate(guidelinePrefab, guidelinePosition, Quaternion.identity);
        }
        else { BroadcastTaskCompletion(); }         // If unable to draw new guideline then task is complete
    }

    // Disable knife interaction and inform scene controller that the task has been completed.
    void BroadcastTaskCompletion()
    {
        CuttingKnife knife = FindObjectOfType<CuttingKnife>();
        if(knife != null) { knife.CanChop = false; }

        // TODO: Replace sceneControllers in level 1 scene and delete this code
        SceneController sceneController = FindObjectOfType<SceneController>();
        if(sceneController != null) { sceneController.TaskComplete(); }

        BaseIngredientSceneManager sceneManager = FindObjectOfType<BaseIngredientSceneManager>();
        if(sceneManager != null) { sceneManager.TaskComplete(); }
    }
}
