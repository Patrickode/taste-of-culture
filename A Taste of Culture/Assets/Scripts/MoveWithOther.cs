using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithOther : MonoBehaviour
{
    [SerializeField] private GameObject other;

    private void Update()
    {
        transform.position = other.transform.position;
    }
}
