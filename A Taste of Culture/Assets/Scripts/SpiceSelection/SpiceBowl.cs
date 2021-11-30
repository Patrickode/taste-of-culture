using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiceBowl : MonoBehaviour
{
    enum TypeOfSpice { CayennePepper, Cumin, Ginger, Garlic, Paprika, Cinnamon, Nutmeg, Coriander, Salt };
    
    [SerializeField] TypeOfSpice spiceCategory;
    [SerializeField] GameObject pinchedSpicePrefab;

    public CookingDialogueTrigger dialogueTrigger;
    public GameObject dialogue;
    
    HandController hand;
    SpiceStation spiceStation;

    bool colliding;

    void Awake() 
    {
        hand = FindObjectOfType<HandController>();

        spiceStation = FindObjectOfType<SpiceStation>();
    }

    void Update() 
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(colliding && spiceStation.CanDisplayTooltip) { hand.SpicePrefab = pinchedSpicePrefab; }
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag != "Hand") { return; }

        if(spiceStation.CanDisplayTooltip) 
        {
            dialogueTrigger.TriggerDialogue();
            colliding = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag != "Hand") { return; }

        dialogueTrigger.DisableDialogue();
        colliding = false;
    }
}
