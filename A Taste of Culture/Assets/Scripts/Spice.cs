using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spice : MonoBehaviour
{
    enum TypeOfSpice { Garlic, Tumeric, Cumin, Ginger, Peppercorn, Salt };
    
    [SerializeField] TypeOfSpice spiceCategory;

    public CookingDialogueTrigger dialogueTrigger;
    public GameObject dialogue;
    // public string dialogueString;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag != "Hand") { return; }

        // TODO: Trigger Tooltip
        Debug.Log("Tooltip: " + spiceCategory);

        dialogueTrigger.TriggerDialogue();
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag != "Hand") { return; }

        dialogueTrigger.DisableDialogue();
    }
}
