using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spice : MonoBehaviour
{
    enum TypeOfSpice { CayennePepper, Cumin, Ginger, Garlic, Paprika, Cinnamon, Nutmeg, Coriander, Salt };
    
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

    void Update() 
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(colliding) { hand.SpicePrefab = pinchedSpicePrefab; }
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag != "Hand") { return; }

        // Trigger tooltip
        dialogueTrigger.TriggerDialogue();
        Debug.Log("Tooltip: " + spiceCategory);

        colliding = true;
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag != "Hand") { return; }

        dialogueTrigger.DisableDialogue();

        colliding = false;
    }
}
