using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGravity3D : MonoBehaviour
{
    [SerializeField] private Vector3 gravity = Physics.gravity;
    private Vector3 originalGrav;

    private void Awake()
    {
        originalGrav = Physics.gravity;
        Physics.gravity = gravity;
    }

    private void OnDestroy()
    {
        Physics.gravity = originalGrav;
    }
}
