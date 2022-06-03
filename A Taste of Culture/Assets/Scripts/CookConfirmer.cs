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
            Vector2 minMaxProg = new Vector2(progress.Min(), progress.Max());

            string debugVals = $"\n<color=#999>(Min Progress = {minMaxProg.x}, Max Progress = {minMaxProg.y})</color>";

            if (minMaxProg.x < 1 - undercookedLeeway)
            {
                Debug.Log("yain't done cooking dingus" + debugVals);
            }
            else if (minMaxProg.y > 2)
            {
                Debug.Log("They're burnt to high hell and it's all your fault. How could you" + debugVals);
            }
            else if (minMaxProg.y > 1 + burnedLeeway)
            {
                Debug.Log("Some of it's a little burnt but good job I guess" + debugVals);
            }
            else
            {
                Debug.Log("You doed it!!" + debugVals);
            }
        }
    }
}
