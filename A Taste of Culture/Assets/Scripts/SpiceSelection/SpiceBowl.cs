using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiceBowl : MonoBehaviour
{
    enum TypeOfSpice { CayennePepper, Cumin, Ginger, Garlic, Paprika, Cinnamon, Nutmeg, Coriander, Salt };

    [SerializeField] TypeOfSpice spiceCategory;
    [SerializeField] GameObject pinchedSpicePrefab;

    public CookingDialogueTrigger dialogueTrigger;
    // public GameObject dialogue;

    HandController hand;
    SpiceStation spiceStation;

    /// <summary>
    /// Which SpiceBowl owns the current active tooltip? (null if there is no active tooltip)<br/>
    /// </summary>
    private SpiceBowl activeTooltipOwner;
    private static System.Action<SpiceBowl> TooltipOwnerStateChange;
    private void OnSpiceTooltipStateChange(SpiceBowl owner) => activeTooltipOwner = owner;

    /// <summary>
    /// How many bowls the hand is hovering over right now.
    /// </summary>
    private int spiceBowlsHovered;
    private static System.Action<bool> TriggerOccupiedStateChange;
    private void OnTriggerOccupiedStateChange(bool occupied) => spiceBowlsHovered += occupied ? 1 : -1;

    private Coroutine DisableTooltipCorout;

    void Awake()
    {
        hand = FindObjectOfType<HandController>();

        spiceStation = FindObjectOfType<SpiceStation>();

        TooltipOwnerStateChange += OnSpiceTooltipStateChange;
        TriggerOccupiedStateChange += OnTriggerOccupiedStateChange;
    }
    private void OnDestroy()
    {
        TooltipOwnerStateChange -= OnSpiceTooltipStateChange;
        TriggerOccupiedStateChange -= OnTriggerOccupiedStateChange;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&
            activeTooltipOwner && activeTooltipOwner == this &&
            spiceStation.CanDisplayTooltip)
        {
            hand.SpicePrefab = pinchedSpicePrefab;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Hand")) { return; }

        TriggerOccupiedStateChange?.Invoke(true);
        TryEnableTooltip();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Hand")) { return; }

        if (!activeTooltipOwner)
            TryEnableTooltip();
    }

    void TryEnableTooltip()
    {
        if (spiceStation.CanDisplayTooltip)
        {
            TooltipOwnerStateChange?.Invoke(this);
            Coroutilities.TryStopCoroutine(this, ref DisableTooltipCorout);

            dialogueTrigger.TriggerDialogue();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Hand")) { return; }

        TriggerOccupiedStateChange?.Invoke(false);
        if (activeTooltipOwner == this)
            TooltipOwnerStateChange?.Invoke(null);

        DisableTooltipCorout = Coroutilities.DoAfterDelayFrames(this, TryDisableTooltip, 1);
    }

    void TryDisableTooltip()
    {
        if (spiceStation.CanDisplayTooltip && spiceBowlsHovered < 1)
        {
            Debug.Log($"Disabling tooltips; {name} was the last occupant and just exited");
            dialogueTrigger.DisableDialogue();
        }
    }
}
