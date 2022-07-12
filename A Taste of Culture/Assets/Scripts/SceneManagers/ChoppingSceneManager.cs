using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingSceneManager : BaseIngredientSceneManager
{
    [SerializeField] List<GameObject> ingredients;
    [SerializeField] List<GameObject> instructions;
    [Space(5)]
    
    [SerializeField] GameObject progressBar;

    GameObject currentIngredient;
    GameObject currentInstruction;

    GameObject progressIndicator;

    public bool bAtLastIngredient = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if(!(ingredients.Count == 0)) { currentIngredient = ingredients[0]; }
        else { Debug.Log("Ingredient list is empty!"); }

        if(!(instructions.Count == 0)) { currentInstruction = instructions[0]; }
        else { Debug.Log("Instruction list is empty!"); }

        progressIndicator = progressBar.transform.GetChild(1).transform.Find("Foreground").gameObject;
        if(progressIndicator != null) { progressIndicator.transform.localScale = new Vector2(0, 1); }
    }

    protected override IEnumerator CompleteTask()
    {
        base.CompleteTask();

        if(progressIndicator != null) 
        { 
            float progress = (float)(ingredients.IndexOf(currentIngredient) + 1) / (float)ingredients.Count;
            progressIndicator.transform.localScale = new Vector2(progress, 1);
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
            if(ingredients.IndexOf(currentIngredient) == ingredients.Count - 1) { bAtLastIngredient = true; }

            currentInstruction = instructions[currentInstructionIndex + 1];

            currentIngredient.SetActive(true);
            currentInstruction.SetActive(true);

            yield break;
        }
        else { base.HandleSceneCompletion(); }
    }
}
