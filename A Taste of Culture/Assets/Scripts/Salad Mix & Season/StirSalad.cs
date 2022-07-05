using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StirSalad : MonoBehaviour
{
    [SerializeField] private VelocityEstimator velEst;
    [SerializeField] private Rigidbody2D[] layersToStir;
    [SerializeField] [Min(0)] private float minimumDelta;

    private void FixedUpdate()
    {
        //If there's no average velocity, or that velocity is below the minimum, bail out.
        if (!(velEst.CurrentAvgVelocity is Vector3 avgVel)
            || avgVel.sqrMagnitude < minimumDelta * minimumDelta)
            return;

        foreach (var layer in layersToStir)
        {
            layer.AddForceAtPosition(avgVel, transform.position, ForceMode2D.Force);
        }
    }
}