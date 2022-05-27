using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiceStation : MonoBehaviour
{
    public CookingDialogueTrigger dialogueTrigger;
    public float dialogueTimer = 2.5f;

    [SerializeField] private UnityEngine.UI.Button[] buttonsToDisable;
    private int timesScolded = 0;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Spice"))
            return;

        StopAllCoroutines();
        StartCoroutine(ScoldPlayer());
    }

    IEnumerator ScoldPlayer()
    {
        if (timesScolded > 0)
        {
            dialogueTrigger.dialogue.sentences[0] = timesScolded < 2
                ? "Oh, don't worry, I'm used to mess-makers."
                : "...";
        }

        //Trigger dialogue and prevent tooltips or early escape until it's disabled.
        dialogueTrigger.TriggerDialogue();
        SpiceBowl.CanDisplayTooltip = false;
        TryToggleButtons(false);

        yield return new WaitForSeconds(2.5f);

        dialogueTrigger.DisableDialogue();
        SpiceBowl.CanDisplayTooltip = true;
        TryToggleButtons(true);

        timesScolded++;
    }

    private void TryToggleButtons(bool enabled)
    {
        foreach (var button in buttonsToDisable)
            if (button)
                button.interactable = enabled;
    }
}
