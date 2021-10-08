using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCutter : MonoBehaviour
{
    public class ResultingIngredients 
    {
        public GameObject firstHalf;
        public GameObject secondHalf;
    }

    public static ResultingIngredients CutIngredient(Vector2 startPosition, Vector2 endPosition, GameObject ingredient)
    {
        Debug.Log("Cutting: " + ingredient.name);
        
        Vector2 cutStart = ingredient.transform.InverseTransformPoint(startPosition);
        Vector2 cutEnd = ingredient.transform.InverseTransformPoint(endPosition);

        return new ResultingIngredients(){ firstHalf = null, secondHalf = null };
    }
}
