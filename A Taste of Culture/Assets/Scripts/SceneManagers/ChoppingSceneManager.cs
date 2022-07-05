using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingSceneManager : BaseIngredientSceneManager
{
    [SerializeField] List<GameObject> Ingredients;
    [SerializeField] List<GameObject> Instructions;

    GameObject currentIngredient;
    GameObject currentInstruction;

    public bool bAtLastIngredient = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if(!(Ingredients.Count == 0)) { currentIngredient = Ingredients[0]; }
        else { Debug.Log("Ingredient list is empty!"); }

        if(!(Instructions.Count == 0)) { currentInstruction = Instructions[0]; }
        else { Debug.Log("Instruction list is empty!"); }
    }

    protected override IEnumerator CompleteTask()
    {
        base.CompleteTask();

        // If the current ingredient isn't the last in the list, disable it and enable the next ingredient
        if (!(currentIngredient == Ingredients[Ingredients.Count - 1]))
        {
            yield return new WaitForSeconds(1f);

            int currentIngredientIndex = Ingredients.IndexOf(currentIngredient);
            int currentInstructionIndex = Instructions.IndexOf(currentInstruction);
            
            if (currentIngredient != null) { currentIngredient.SetActive(false); }
            if (currentInstruction != null) { currentInstruction.SetActive(false); }

            GameObject spriteMask = GameObject.Find("Chunks Sprite Mask(Clone)");
            if (spriteMask != null) { GameObject.Destroy(spriteMask); }

            currentIngredient = Ingredients[currentIngredientIndex + 1];
            if(Ingredients.IndexOf(currentIngredient) == Ingredients.Count - 1) { bAtLastIngredient = true; }

            currentInstruction = Instructions[currentInstructionIndex + 1];

            currentIngredient.SetActive(true);
            currentInstruction.SetActive(true);

            yield break;
        }
        else { base.HandleSceneCompletion(); }
    }
}
