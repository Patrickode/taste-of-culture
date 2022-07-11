using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMousePos : MonoBehaviour
{
    [SerializeField] private bool onlyMoveIfHeld;
    [SerializeField] private bool returnWhenDropped;
    [SerializeField] private LayerMask holdMask;
    [SerializeField] [TagSelector] private string holdTag;
    [VectorLabels(0.5f, 5, "Min", "Max")]
    [Tooltip("The minimum/maximum (inclusive) Z coordinates that'll be recognized by the hold check. " +
        "See https://docs.unity3d.com/ScriptReference/Physics2D.OverlapPoint.html.")]
    [SerializeField] private Vector2 holdDepthRange = new Vector2(Mathf.NegativeInfinity, Mathf.Infinity);
    [Space(10)]
    [SerializeField] private bool moveWithPhysics;
    [SerializeField] private GameObject thingToMove;
    [Tooltip("If assigned, thingToMove will be constrained to positions inside this collider.")]
    [SerializeField] private Collider2D moveZone;
    [SerializeField] private Collider moveZone3D;
    [SerializeField] private float screenPointDistance;
    [SerializeField] private bool preserveDepthPos;

    private Rigidbody2D movedRb2D;
    private Rigidbody movedRb3D;
    private Vector3 holdOffset = Vector3.zero;
    private Vector3 originalPos;

    public bool CanMove { get; set; } = true;
    public bool Held { get; private set; }

    private Camera _cachedCam;
    private Camera CachedCam
    {
        get
        {
            if (!_cachedCam) _cachedCam = Camera.main;
            return _cachedCam;
        }
    }

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        holdDepthRange.x = Mathf.Min(holdDepthRange.x, holdDepthRange.y);
        holdDepthRange.y = Mathf.Max(holdDepthRange.y, holdDepthRange.x);
    });

    private void Start()
    {
        originalPos = thingToMove.transform.position;

        if (!thingToMove)
            thingToMove = gameObject;

        if (moveWithPhysics)
            if (!thingToMove.TryGetComponent(out movedRb2D) && !thingToMove.TryGetComponent(out movedRb3D))
            {
                Debug.LogWarning($"{name} was told to move {thingToMove.name} with physics, but no 2D rigidbody was found. " +
                    $"Defaulting to non-physics movement.");
                moveWithPhysics = false;
            }

        CookStirIngredient.DoneCooking += OnStirringComplete;
    }

    private void OnDestroy()
    {
        CookStirIngredient.DoneCooking -= OnStirringComplete;
    }

    private void OnStirringComplete()
    {
        CookStirIngredient.DoneCooking -= OnStirringComplete;
        CanMove = false;
    }

    private void Update()
    {
        //No need to check if this obj's held or not if being held doesn't matter.
        if (!onlyMoveIfHeld) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Since the mouse is down, check if it's down on any of the colliders in the hold layer mask.
            Vector3 clickPos = CachedCam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * screenPointDistance);
            Collider2D clickedColl = Physics2D.OverlapPoint(clickPos, holdMask, holdDepthRange.x, holdDepthRange.y);

            //If we're looking for a specific tag as well, check for that, too.
            //If not, just go ahead (so long as we found *something*).
            if (clickedColl && (string.IsNullOrEmpty(holdTag) || clickedColl.CompareTag(holdTag)))
            {
                Held = true;
                holdOffset = thingToMove.transform.position - clickPos;
            }
        }
        else if (Held && !Input.GetKey(KeyCode.Mouse0))
        {
            Held = false;
            holdOffset = Vector3.zero;
            if (returnWhenDropped)
                thingToMove.transform.position = originalPos;
        }
    }

    private void FixedUpdate()
    {
        //If not held and we can only move when held, bail out. If moving's disabled in general, bail out.
        if (!CanMove || (onlyMoveIfHeld && !Held)) return;

        Vector3 destination = CachedCam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * screenPointDistance);
        destination += holdOffset;

        //If we've got a moveZone to constrain the destination to, get the closest point on it to the destination.
        if (moveZone)
            destination = moveZone.ClosestPoint(destination);
        else if (moveZone3D)
        {
            destination = moveZone3D.ClosestPoint(destination);
            //Y was used instead of Z for a 3D experiment where Y was the depth dir (so typical gravity was away from camera)
            if (preserveDepthPos)
                destination.y = thingToMove.transform.position.y;
        }

        if (moveWithPhysics)
        {
            if (movedRb2D)
                movedRb2D.MovePosition(destination);
            else
                movedRb3D.MovePosition(destination);
        }
        else
            thingToMove.transform.position = destination;
    }
}