using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMousePos : MonoBehaviour
{
    [SerializeField] private bool onlyMoveIfHeld;
    [Space(5)]
    [SerializeField] private bool moveWithPhysics;
    [SerializeField] private GameObject thingToMove;
    [Tooltip("If assigned, thingToMove will be constrained to positions inside this collider.")]
    [SerializeField] private Collider2D moveZone;
    [SerializeField] private Collider moveZone3D;
    [SerializeField] private float screenPointDistance;
    [SerializeField] private bool preserveDepthPos;

    private Rigidbody2D movedRb2D;
    private Rigidbody movedRb3D;

    private int holdMask;
    private bool held;
    private Vector3 holdOffset = Vector3.zero;

    public bool CanMove { get; set; } = true;

    private Camera _cachedCam;
    private Camera CachedCam
    {
        get
        {
            if (!_cachedCam) _cachedCam = Camera.main;
            return _cachedCam;
        }
    }

    private void Start()
    {
        holdMask = LayerMask.GetMask("Tool");

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
        if (!onlyMoveIfHeld) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 clickPos = CachedCam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * screenPointDistance);
            Collider2D clickedColl = Physics2D.OverlapPoint(clickPos, holdMask);

            if (clickedColl && clickedColl.CompareTag("Player"))
            {
                held = true;
                holdOffset = thingToMove.transform.position - clickPos;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            held = false;
            holdOffset = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!CanMove || (onlyMoveIfHeld && !held)) return;

        Vector3 destination = CachedCam.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * screenPointDistance);
        destination += holdOffset;

        if (moveZone)
            destination = moveZone.ClosestPoint(destination);
        else if (moveZone3D)
        {
            destination = moveZone3D.ClosestPoint(destination);
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