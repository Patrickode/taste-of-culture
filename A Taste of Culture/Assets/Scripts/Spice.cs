using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spice : MonoBehaviour
{
    enum TypeOfSpice { CayennePepper, Cumin, Ginger, Garlic, Paprika, Cinnamon, Nutmeg, Coriander };
    
    [SerializeField] TypeOfSpice spiceCategory;
    [SerializeField] GameObject pinchedSpicePrefab;

    public CookingDialogueTrigger dialogueTrigger;
    public GameObject dialogue;
    
    HandController hand;

    bool colliding;

    void Awake() 
    {
        hand = FindObjectOfType<HandController>();
    }

    // void Update() 
    // {
    //     if(Input.GetMouseButtonDown(0))
    //     {
    //         if(colliding) { hand.SpicePrefab = pinchedSpicePrefab; }
    //         else { hand.SpicePrefab = null; }

    //         Debug.Log("Current spice: " + pinchedSpicePrefab.name);
    //     }
    // }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag != "Hand") { return; }

        // TODO: Trigger Tooltip
        Debug.Log("Tooltip: " + spiceCategory);

        dialogueTrigger.TriggerDialogue();

        colliding = true;
        // if(hand != null) { hand.SpicePrefab = pinchedSpicePrefab; }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag != "Hand") { return; }

        dialogueTrigger.DisableDialogue();

        // if(hand != null) { hand.SpicePrefab = null; }

        colliding = false;
    }
}
