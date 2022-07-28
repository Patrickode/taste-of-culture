using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingSceneManager : BaseIngredientSceneManager
{
    [Header("Chopping Scene Manager Fields")]
    [SerializeField] List<GameObject> ingredients;
    [SerializeField] List<GameObject> instructions;
    [Space(5)]
    [SerializeField] GameObject progressFill;
    [SerializeField] [Min(0)] float fillDuration;
    Vector3 progressScale = Vector3.up;

    GameObject currentIngredient;
    GameObject currentInstruction;

    [HideInInspector] public bool bAtLastIngredient = false;

    private Coroutine fillAnim;
    private float animProgress;

    void Start()
    {
        if (!(ingredients.Count == 0))
            currentIngredient = ingredients[0];
        else Debug.Log("Ingredient list is empty!");

        if (!(instructions.Count == 0))
            currentInstruction = instructions[0];
        else Debug.Log("Instruction list is empty!");

        if (progressFill != null)
            progressFill.transform.localScale = progressScale;
    }

    protected override IEnumerator CompleteTask()
    {
        base.CompleteTask();

        if (progressFill != null)
        {
            progressScale.x = (ingredients.IndexOf(currentIngredient) + 1f) / ingredients.Count;

            //I realized too late this doesn't save the intial scale needed for a proper lerp, but it has a nice
            //easing effect so it was 100% intentional actually
            Coroutilities.TryStopCoroutine(this, ref fillAnim);
            fillAnim = Coroutilities.DoUntil(this,
                () => AdvanceFill(progressFill.transform.localScale, progressScale, fillDuration),
                () => animProgress >= 1);

            //When the above completes, reset anim progress
            Coroutilities.DoAfterYielder(this, () => animProgress = 0, fillAnim);
        }

        // If the current ingredient isn't the last in the list, disable it and enable the next ingredient
        if (!(currentIngredient == ingredients[ingredients.Count - 1]))
        {
            yield return new WaitForSeconds(1f);

            int currentIngredientIndex = ingredients.IndexOf(currentIngredient);
            int currentInstructionIndex = instructions.IndexOf(currentInstruction);

            if (currentIngredient != null) { currentIngredient.SetActive(false); }
            if (currentInstruction != null) { currentInstruction.SetActive(false); }

            GameObject spriteMask = GameObject.Find("Chunks Sprite Mask(Clone)");
            if (spriteMask != null) { GameObject.Destroy(spriteMask); }

            currentIngredient = ingredients[currentIngredientIndex + 1];
            if (ingredients.IndexOf(currentIngredient) == ingredients.Count - 1) { bAtLastIngredient = true; }

            currentInstruction = instructions[currentInstructionIndex + 1];

            currentIngredient.SetActive(true);
            currentInstruction.SetActive(true);

            yield break;
        }
        else { base.HandleSceneCompletion(); }
    }

    void AdvanceFill(Vector3 start, Vector3 target, float duration)
    {
        animProgress = Mathf.Min(
            animProgress + duration > 0 ? Time.deltaTime / duration : 1,
            1);
        progressFill.transform.localScale = Vector3.Lerp(start, target, animProgress);
    }
}