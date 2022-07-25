using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using DialogueEditor;

public class CookConfirmer : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float minorBurn;
    [SerializeField] [Range(0, 1)] private float fullyBurned;
    //[Space(5)]
    //[SerializeField] [Range(0, 1)] private float minorityThreshold = 0.25f;
    //[SerializeField] [Range(0, 1)] private float majorityThreshold = 0.5f;

    [SerializeField] private NPCConversation onionDialogue;

    private void Start()
    {
        fullyBurned = Mathf.Max(fullyBurned, minorBurn);
        //majorityThreshold = Mathf.Max(majorityThreshold, minorityThreshold);

        CookStirIngredient.DoneCooking += ConfirmOnionSaute;
    }
    private void OnDestroy()
    {
        CookStirIngredient.DoneCooking -= ConfirmOnionSaute;
    }

    private void ConfirmOnionSaute()
    {
        CookStirIngredient.DoneCooking -= ConfirmOnionSaute;

        List<float> burnAmounts = CookStirIngredient.BurnAmounts;
        int numBurned = burnAmounts.Count((value) => value > minorBurn);

        //        string msg = "<color=#E05E00>";
        //        if (numBurned < 1)
        //        {
        //            msg += "Great job! Let's move on.";
        //        }
        //        else
        //        {
        //            float mostBurned = burnAmounts.Max();

        //            if (numBurned < 2)
        //                msg += "One of them's";
        //            else if (numBurned < Mathf.RoundToInt(burnAmounts.Count * minorityThreshold))
        //                msg += "A few of them are";
        //            else if (numBurned < Mathf.RoundToInt(burnAmounts.Count * majorityThreshold))
        //                msg += "Some of them are";
        //            else if (numBurned < burnAmounts.Count)
        //                msg += "Most of them are";
        //            else
        //                msg += "They're all";

        //            if (mostBurned < fullyBurned)
        //                msg += " a little";

        //            msg += " burned, but don't worry, I can make more";

        //            if (numBurned < Mathf.RoundToInt(burnAmounts.Count * majorityThreshold)
        //                || mostBurned < fullyBurned)
        //            {
        //                msg += " if we need to";
        //            }

        //            msg += ".";
        //        }
        //        msg += "</color>";

//#if UNITY_EDITOR
//        msg += $"\n<color=#999>Burned count = {numBurned} out of {burnAmounts.Count}; " +
//            $"Most burned = {burnAmounts.Max()}</color>";
//#endif

        Debug.Log($"\n<color=#999>Burned count = {numBurned} out of {burnAmounts.Count}; " +
            $"Most burned = {burnAmounts.Max()}</color>");

        ConversationManager.Instance.StartConversation(onionDialogue);

        float percentBurnt = (float)numBurned / (float)burnAmounts.Count;

        ConversationManager.Instance.SetInt("PercentBurnt", Mathf.RoundToInt(percentBurnt * 100f));
        ConversationManager.Instance.SetBool("VeryBurnt", burnAmounts.Max() > fullyBurned);

        Debug.Log(Mathf.RoundToInt(percentBurnt * 100f) + ", " + (burnAmounts.Max() > fullyBurned));

        CookStirIngredient.CookingPaused = true;

        //Transitions.LoadWithTransition?.Invoke(-1, -1);
    }
}
