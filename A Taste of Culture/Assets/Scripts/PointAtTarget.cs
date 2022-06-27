using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtTarget : MonoBehaviour
{
    enum Axes { X, Y, Z }

    [SerializeField] private Transform target;
    [SerializeField] private Axes axis;

    private void Update()
    {
        Vector3 facingDir = (target.position - transform.position).normalized;
        switch (axis)
        {
            case Axes.X:
                transform.right = facingDir;
                break;
            case Axes.Y:
                transform.up = facingDir;
                break;
            case Axes.Z:
                transform.forward = facingDir;
                break;
            default:
                break;
        }
    }
}
