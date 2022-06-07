using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CookConfirmer : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float minorBurn;
    [SerializeField] [Range(0, 1)] private float fullyBurned;
    [Space(5)]
    [SerializeField] [Range(0, 1)] private float minorityThreshold = 0.25f;
    [SerializeField] [Range(0, 1)] private float majorityThreshold = 0.5f;

    private void Start()
    {
        fullyBurned = Mathf.Max(fullyBurned, minorBurn);
        majorityThreshold = Mathf.Max(majorityThreshold, minorityThreshold);

        CookStirIngredients.DoneCooking += ConfirmCooking;
    }
    private void OnDestroy()
    {
        CookStirIngredients.DoneCooking -= ConfirmCooking;
    }

    private void ConfirmCooking()
    {
        CookStirIngredients.DoneCooking -= ConfirmCooking;

        List<float> burnAmounts = CookStirIngredients.BurnAmounts;
        int numBurned = burnAmounts.Count((value) => value > minorBurn);

        string msg = "<color=#E05E00>";
        if (numBurned < 2)
        {
            msg += "Great job! Let's move on.";
        }
        else
        {
            float mostBurned = burnAmounts.Max();

            if (numBurned < Mathf.RoundToInt(burnAmounts.Count * minorityThreshold))
                msg += "A few";
            else if (numBurned < Mathf.RoundToInt(burnAmounts.Count * majorityThreshold))
                msg += "Some";
            else
                msg += "Most";

            msg += " of them are";

            if (mostBurned < fullyBurned)
                msg += " a little";

            msg += " burned, but don't worry, I can make some more";

            if (numBurned < Mathf.RoundToInt(burnAmounts.Count * minorityThreshold)
                || mostBurned < fullyBurned)
            {
                msg += " if we need to";
            }

            msg += ". Let's move on.";
        }
        msg += "</color>";

#if UNITY_EDITOR
        msg += $"\n<color=#999>Burned count = {numBurned}; " +
            $"Most burned = {(burnAmounts.Count > 0 ? burnAmounts.Max() : 0)}</color>";
#endif

        Debug.Log(msg);
        CookStirIngredients.CookingPaused = true;
        //Transition to next scene
    }
}
