using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaladFinishCheck : MonoBehaviour
{
    [SerializeField] private Transform seasoningContainer;
    [SerializeField] private TMPro.TextMeshProUGUI targetTxt;
    [Space(5)]
    [SerializeField] private bool startActive;
    [SerializeField] [Min(0)] private int perfectLeeway;
    [SerializeField] [Min(0)] private int tooMuchThreshold;
    [SerializeField] private Bewildered.UDictionary<FlavorType, int> targetFlavor;
    private Dictionary<FlavorType, int> deltaDict;

    private void Start()
    {
        gameObject.SetActive(startActive);

        SetTargetText();

        deltaDict = new Dictionary<FlavorType, int>();
        foreach (var flav in targetFlavor)
            deltaDict[flav.Key] = -flav.Value;

        FlavorProfileData.FlavorUpdated += OnFlavorUpdate;
    }
    private void OnDestroy()
    {
        FlavorProfileData.FlavorUpdated -= OnFlavorUpdate;
    }

    private void OnFlavorUpdate(FlavorType updatedType, int updatedValue)
    {
        deltaDict[updatedType] = updatedValue - targetFlavor[updatedType];
        gameObject.SetActive(true);
    }

    private void SetTargetText()
    {
        string txt = "Target:\n";
        txt += string.Join("\n", targetFlavor);
        txt = txt.TrimEnd('\n');
        txt = txt.Replace("[", "");
        txt = txt.Replace("]", "");
        targetTxt.text = txt;
    }

    public void TryFinish()
    {
        string msg = "<color=#F80>";

        if (seasoningContainer.childCount > 0)
            msg += "You forgot to mix it!";

        else if (deltaDict.Any(flav => flav.Value < 0))
            msg += "Hold on, now, the salad needs more seasoning.";

        else if (deltaDict.Any(flav => flav.Value > perfectLeeway + tooMuchThreshold))
            msg += "Oof. That's, uh, not normal. You get the idea, though, right? Right. Let's move on.";

        else if (deltaDict.Any(flav => flav.Value > perfectLeeway))
            msg += "It's a little different than we usually have our salad, but you've got the gist. Good job.";

        else
            msg += "Perfect! Well done.";

        msg += "</color>";

        Debug.Log(msg);
    }
}