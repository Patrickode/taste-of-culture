using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookConfirmer : MonoBehaviour
{
    [SerializeField] private KeyCode confirmKey;
    [SerializeField] [Range(0, 1)] private float undercookedLeeway;
    [SerializeField] [Range(0, 1)] private float burnedLeeway;

    private void Update()
    {
        if (Input.GetKeyDown(confirmKey))
        {
            if (CookStirIngredients.MaxCookProgress < 1 - undercookedLeeway)
            {
                Debug.Log("yain't done cooking dingus " +
                    $"(Progress = {CookStirIngredients.MaxCookProgress})");
            }
            else if (CookStirIngredients.MaxCookProgress > 1 + burnedLeeway)
            {
                Debug.Log("Some of it's a little burnt but good job I guess " +
                    $"(Progress = {CookStirIngredients.MaxCookProgress})");
            }
            else
            {
                Debug.Log("You doed it!!" +
                    $"(Progress = {CookStirIngredients.MaxCookProgress})");
            }
        }
    }
}
