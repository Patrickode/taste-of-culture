using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMousePos : MonoBehaviour
{
    [SerializeField] private bool moveWithPhysics;
    [SerializeField] private GameObject thingToMove;
    [Tooltip("If assigned, thingToMove will be constrained to positions inside this collider.")]
    [SerializeField] private Collider2D moveZone;
    private Rigidbody2D rbToMove;

    private Camera _cachedCam;
    private Camera CachedCam
    {
        get
        {
            if (!_cachedCam) _cachedCam = Camera.main;
            return _cachedCam;
        }
    }

    public bool CanMove { get; set; } = true;

    private void Start()
    {
        if (!thingToMove)
            thingToMove = gameObject;

        if (moveWithPhysics)
            if (!thingToMove.TryGetComponent(out rbToMove))
            {
                Debug.LogError($"{name} was told to {thingToMove} with physics, but no rigidbody was found." +
                    $"Defaulting to non-physics movement.");
                moveWithPhysics = false;
            }
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;

        Vector2 destination = CachedCam.ScreenToWorldPoint(Input.mousePosition);
        if (moveZone)
            destination = moveZone.ClosestPoint(destination);

        if (moveWithPhysics)
            rbToMove.MovePosition(destination);
        else
            thingToMove.transform.position = destination;
    }
}