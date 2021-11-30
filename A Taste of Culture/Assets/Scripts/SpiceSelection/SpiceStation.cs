using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiceStation : MonoBehaviour
{
    public CookingDialogueTrigger dialogueTrigger;
    public float dialogueTimer = 2.5f;

    private bool canDisplayTooltip = true;
    public bool CanDisplayTooltip { get { return canDisplayTooltip; } }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if(!(other.gameObject.tag == "Spice")) { return; }

        StartCoroutine(ScoldPlayer());
    }

    IEnumerator ScoldPlayer()
    {
        dialogueTrigger.TriggerDialogue();
        canDisplayTooltip = false;                      // Prevent new tooltip from being displayed while player is being scolded.

        yield return new WaitForSeconds(2.5f);

        dialogueTrigger.DisableDialogue();
        canDisplayTooltip = true;
    }
}
