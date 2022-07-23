using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiceOnCountertop : MonoBehaviour
{
    public float dialogueTimer = 2.5f;

    [SerializeField] private UnityEngine.UI.Button[] buttonsToDisable;
    private int timesScolded = 0;

    [SerializeField] private NPCConversation scoldKind;
    [SerializeField] private NPCConversation scoldMean;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Spice"))
            return;

        StopAllCoroutines();
        StartCoroutine(ScoldPlayer());
    }

    IEnumerator ScoldPlayer()
    {
        //Trigger dialogue and prevent tooltips or early escape until it's disabled.
        SpiceBowl.CanDisplayTooltip = false;
        TryToggleButtons(false);

        if (timesScolded < 2)
        {
            ConversationManager.Instance.StartConversation(scoldKind);
        }
        else
        {
            ConversationManager.Instance.StartConversation(scoldMean);
        }

        yield return new WaitForSeconds(2.5f);

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
