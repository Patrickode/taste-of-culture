using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CookConfirmer : MonoBehaviour
{
    [SerializeField] private KeyCode confirmKey;
    [SerializeField] [Range(0, 1)] private float undercookedLeeway;
    [SerializeField] [Range(0, 1)] private float burnedLeeway;

    private void Update()
    {
        if (Input.GetKeyDown(confirmKey))
        {
            List<float> progress = CookStirIngredients.AllProgress;
            string debugVals = $"\n<color=#999>(Min Progress = {progress.Min()}, Max Progress = {progress.Max()})</color>";

            if (progress.Min() < 1 - undercookedLeeway)
            {
                Debug.Log("yain't done cooking dingus " + debugVals);
            }
            else if (progress.Max() > 1 + burnedLeeway)
            {
                Debug.Log("Some of it's a little burnt but good job I guess " + debugVals);
            }
            else
            {
                Debug.Log("You doed it!!" + debugVals);
            }
        }
    }
}
