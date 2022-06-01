using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampVelocity : MonoBehaviour
{
    [SerializeField] private Rigidbody rb3D;
    [SerializeField] private Rigidbody2D rb2D;
    [Space(5)]
    [SerializeField] private float maxSpeed;

    private void FixedUpdate()
    {
        if (!rb3D && !rb2D)
        {
            Debug.LogWarning($"{name}'s ClampVelocity needs a rigidbody to clamp the velocity of.");
            enabled = false;
        }

        Vector3 velToClamp = rb3D ? rb3D.velocity : (Vector3)rb2D.velocity;
        float speed = velToClamp.magnitude;

        if (speed > maxSpeed)
        {
            Vector3 brakingForce = -velToClamp.normalized * (speed - maxSpeed);

            if (rb3D)
                rb3D.AddForce(brakingForce, ForceMode.Impulse);
            else
                rb2D.AddForce(brakingForce, ForceMode2D.Impulse);
        }
    }
}
