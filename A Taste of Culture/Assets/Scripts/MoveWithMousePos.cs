using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMousePos : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbToMove;
    [SerializeField] private Collider2D moveZone;

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

    private void Update()
    {
        if (!CanMove) return;

        Rigidbody2D moved = rbToMove;
        if (!moved && !TryGetComponent(out moved))
        {
            Debug.LogError(name + "'s MoveWithMousePos was not given a Rigidbody to move, nor was one attached.");
            CanMove = false;
            return;
        }

        Vector2 destination = CachedCam.ScreenToWorldPoint(Input.mousePosition);
        if (moveZone)
            destination = moveZone.ClosestPoint(destination);

        moved.MovePosition(destination);
    }
}