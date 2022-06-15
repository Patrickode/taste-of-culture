using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampVelocity : MonoBehaviour
{
    [SerializeField] private Rigidbody rb3D;
    [SerializeField] private Rigidbody2D rb2D;
    [Space(5)]
    [SerializeField] private bool clampLinear = true;
    [SerializeField] private float maxSpeed;
    [SerializeField] private bool clampAngular;
    [SerializeField] private float maxAngSpeed;

    private void FixedUpdate()
    {
        if (!rb3D && !rb2D)
        {
            Debug.LogWarning($"{name}'s ClampVelocity needs a rigidbody to clamp the velocity of.");
            enabled = false;
        }

        if (clampLinear)
        {
            if (rb3D)
                rb3D.AddForce(
                    GetVectorBrakingForce(rb3D.velocity, maxSpeed),
                    ForceMode.Impulse);
            else
                rb2D.AddForce(
                    GetVectorBrakingForce(rb2D.velocity, maxSpeed),
                    ForceMode2D.Impulse);
        }

        if (clampAngular)
        {
            if (rb3D)
                rb3D.AddTorque(
                    GetVectorBrakingForce(rb3D.angularVelocity, maxAngSpeed),
                    ForceMode.Impulse);
            else
                //We need inertia because, apparently, AddTorque operates in radians, despite the fact angVel
                //is measured in degrees; see https://docs.unity3d.com/ScriptReference/Rigidbody2D.AddTorque.html
                rb2D.AddTorque(
                    GetFloatBrakingForce(rb2D.angularVelocity, maxAngSpeed, rb2D.inertia),
                    ForceMode2D.Impulse);
        }
    }

    private Vector3 GetVectorBrakingForce(Vector3 forceToClamp, float maxMagnitude)
    {
        float speed = forceToClamp.magnitude;
        if (speed > maxMagnitude)
            return -forceToClamp.normalized * (speed - maxSpeed);

        return Vector3.zero;
    }

    private float GetFloatBrakingForce(float forceToClamp, float maxMagnitude, float inertia)
    {
        if (forceToClamp > maxMagnitude || forceToClamp < -maxMagnitude)
        {
            //Invert the signed distance between the clamped & max; this works regardless of > or <
            return (-1 * (forceToClamp - Mathf.Sign(forceToClamp) * maxMagnitude) * Mathf.Deg2Rad) * inertia;
        }

        return 0;
    }
}
