using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiceBowl : MonoBehaviour
{
    public enum TypeOfSpice { CayennePepper, Cumin, Ginger, Garlic, Paprika, Cinnamon, Nutmeg, Coriander, Salt, Cardamon, GaramMasala };

    [SerializeField] TypeOfSpice spiceCategory;
    [Tooltip("The max distance a spice's center can be from this bowl's center and still be destroyed (put back " +
        "in the bowl, reclaimed)")]
    [SerializeField] [Min(0)] float maxReclaimDistance;
    [SerializeField] Collider2D triggerRef;
    [Space(5)]
    [SerializeField] SpriteRenderer pinchedSpicePrefab;
    //public CookingDialogueTrigger dialogueTrigger;
    // public GameObject dialogue;

    HandController hand;
    SpiceOnCountertop spiceStation;

    public static bool CanDisplayTooltip { get; set; } = true;

    /// <summary>
    /// Which SpiceBowl owns the current active tooltip? (null if there is no active tooltip)<br/>
    /// </summary>
    private SpiceBowl activeTooltipOwner;
    private static System.Action<SpiceBowl> TooltipOwnerStateChange;
    private void OnSpiceTooltipStateChange(SpiceBowl owner) => activeTooltipOwner = owner;

    /// <summary>
    /// How many bowls the hand is hovering over (colliding with) right now.
    /// </summary>
    private int spiceBowlsHovered;
    private static System.Action<bool> TriggerOccupiedStateChange;
    private void OnTriggerOccupiedStateChange(bool occupied) => spiceBowlsHovered += occupied ? 1 : -1;

    private Coroutine DisableTooltipCorout;

    void Awake()
    {
        hand = FindObjectOfType<HandController>();

        spiceStation = FindObjectOfType<SpiceOnCountertop>();

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
        //If the mouse is down and this is the spice bowl it's hovering over, pinch this spice
        if (Input.GetMouseButtonDown(0) &&
            activeTooltipOwner && activeTooltipOwner == this &&
            CanDisplayTooltip)
        {
            hand.SpicePrefab = pinchedSpicePrefab;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Spice")
            && other.TryGetComponent(out Spice spi) && spi.spiceType == spiceCategory)
        {
            Vector3 oCenter = other.transform.TransformPoint(other.offset);
            Vector3 thisCenter = transform.TransformPoint(triggerRef.offset);

            if ((thisCenter - oCenter).sqrMagnitude <= maxReclaimDistance * maxReclaimDistance)
                Destroy(other.gameObject);
            return;
        }

        if (!other.gameObject.CompareTag("Hand")) { return; }

        //Once when first hovering, note that we're hovering
        TriggerOccupiedStateChange?.Invoke(true);
        TryEnableTooltip();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Hand")) { return; }

        //If we're hovering over this and there's not a tooltip, try to enable one
        if (!activeTooltipOwner)
            TryEnableTooltip();
    }

    void TryEnableTooltip()
    {
        if (CanDisplayTooltip)
        {
            //Note that this spice bowl is the owner of the tooltip we're spawning
            TooltipOwnerStateChange?.Invoke(this);
            Coroutilities.TryStopCoroutine(this, ref DisableTooltipCorout);
            Tooltip.ShowTooltip_Static(spiceCategory.ToString());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Hand")) { return; }

        //Note that we've stopped hovering over this spice bowl
        TriggerOccupiedStateChange?.Invoke(false);
        //If this spice bowl is also the one with a tooltip right now, since we just 
        //stopped hovering over it, discard ownership of that tooltip
        if (activeTooltipOwner == this)
            TooltipOwnerStateChange?.Invoke(null);

        //Wait a frame before trying to disable the tooltip, just in case another spice bowl picks 
        //it up immediately after this
        DisableTooltipCorout = Coroutilities.DoAfterDelayFrames(this, TryDisableTooltip, 1);
    }

    void TryDisableTooltip()
    {
        if (CanDisplayTooltip && spiceBowlsHovered < 1)
        {
            Tooltip.HideTooltip_Static();
        }
    }
}
