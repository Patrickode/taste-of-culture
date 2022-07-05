using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityEstimator : MonoBehaviour
{
    [SerializeField] private Transform velTransform;
    [Tooltip("The max number of elements allowed in the velocity bank. The oldest element will be removed if an " +
        "addition would exceed the capacity.")]
    [SerializeField] [Min(1)] private int velBankCapacity = 10;
    [Tooltip("Whether this estimator is actually estimating right now, or not. Basically, an on/off switch.")]
    [SerializeField] private bool estimating = true;

    private Vector3 previousPos;
    private LinkedList<Vector3> estVelocityBank;
    private Vector3? currentAvgVel = null;
    public Vector3? CurrentAvgVelocity
    {
        get => currentAvgVel;
        private set => currentAvgVel = value;
    }

    private void Start()
    {
        //If we weren't given a transform to estimate the velocity of, assume it's this script's.
        if (!velTransform)
            velTransform = transform;

        estVelocityBank = new LinkedList<Vector3>();
        previousPos = velTransform.localPosition;
    }

    /// <summary>
    /// Turn estimation on/off.<br/>
    /// If switching from on to off or vice-versa, clears <see cref="estVelocityBank"/> to avoid polluting averages 
    /// with old samples.
    /// </summary>
    /// <param name="shouldEstimate">If true, turn estimation on. If false, turn estimation off.</param>
    public void SetEstimationActve(bool shouldEstimate)
    {
        if (estimating != shouldEstimate)
        {
            estimating = shouldEstimate;
            estVelocityBank.Clear();
        }
    }

    private void FixedUpdate()
    {
        if (estimating)
        {
            UpdateVelocityBank();
            CurrentAvgVelocity = GetAverageFromBank();
        }

        //After doing everything else, set previousPos to be the current pos (since it's about to be "previous")
        previousPos = velTransform.localPosition;
    }

    /// <summary>
    /// Estimates the current velocity, adds it to <see cref="estVelocityBank"/>, and removes the oldest estimate 
    /// if the bank exceeds <see cref="velBankCapacity"/>.<br/>
    /// This method does nothing if <see cref="Time.fixedDeltaTime"/> is a near-zero float.
    /// </summary>
    private void UpdateVelocityBank()
    {
        if (!Mathf.Approximately(Time.fixedDeltaTime, 0))
        {
            Vector3 vel = (velTransform.localPosition - previousPos) / Time.fixedDeltaTime;
            if (estVelocityBank.Count >= velBankCapacity)
            {
                estVelocityBank.RemoveLast();
            }
            estVelocityBank.AddFirst(vel);
        }
    }

    /// <summary>
    /// Loops through <see cref="estVelocityBank"/> to return the mean average of its elements.
    /// </summary>
    /// <returns>The mean average of <see cref="estVelocityBank"/>.</returns>
    private Vector3? GetAverageFromBank()
    {
        if (estVelocityBank.Count < 1)
            return null;

        Vector3 avg = Vector3.zero;
        foreach (Vector3 vel in estVelocityBank)
        {
            avg += vel;
        }
        return avg / estVelocityBank.Count;
    }
}