using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerSpoon : MonoBehaviour
{
    [SerializeField] private Transform visuals;
    [SerializeField] private GameObject shadow;
    [SerializeField] private Collider2D spoonCollider;
    [Space(5)]
    [SerializeField] private Vector3 raisedOffset;
    private Vector3 originalPos;

    private void Start()
    {
        originalPos = visuals.localPosition;
        ToggleLower(false);
    }

    private void Update()
    {
        ToggleLower(Input.GetKey(KeyCode.Mouse0));
    }

    private void ToggleLower(bool lowered)
    {
        visuals.localPosition = originalPos + (lowered ? Vector3.zero : raisedOffset);
        shadow.SetActive(!lowered);
        spoonCollider.enabled = lowered;
    }
}
