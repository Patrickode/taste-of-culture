using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMousePos : MonoBehaviour
{
    [SerializeField] private bool moveWithPhysics;
    [SerializeField] private GameObject thingToMove;
    [Tooltip("If assigned, thingToMove will be constrained to positions inside this collider.")]
    [SerializeField] private Collider2D moveZone;
    private Rigidbody2D movedRb2D;
    private Rigidbody movedRb3D;

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

    private void FixedUpdate()
    {
        if (!CanMove) return;

        Vector2 destination = CachedCam.ScreenToWorldPoint(Input.mousePosition);
        if (moveZone)
            destination = moveZone.ClosestPoint(destination);

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