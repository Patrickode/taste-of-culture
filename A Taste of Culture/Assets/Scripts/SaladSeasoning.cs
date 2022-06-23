using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaladSeasoning : MonoBehaviour
{
    [Header("Base Seasoning Fields")]
    [SerializeField] protected SpriteRenderer spRend;
    [Tooltip("The amount of rotation it'll take to fully mix this seasoning in.")]
    [SerializeField] protected float mixDegrees;

    protected Quaternion previousRot;
    protected float anglesMoved;

    public float MixProgress { get; protected set; }

    protected virtual void Start()
    {
        previousRot = transform.parent.localRotation;
    }

    protected virtual void Update()
    {
        anglesMoved += Quaternion.Angle(previousRot, transform.parent.localRotation);
        MixProgress = anglesMoved / mixDegrees;
        MixAction();

        previousRot = transform.parent.localRotation;
    }

    protected abstract void MixAction();
}