using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaladSeasoning : MonoBehaviour
{
    [Header("Base Seasoning Fields")]
    [SerializeField] protected SpriteRenderer spRend;
    [Tooltip("The amount of rotation it'll take to fully mix this seasoning in.")]
    [SerializeField] protected float mixDegrees;
    [SerializeField] protected Bewildered.UDictionary<FlavorType, int> flavor;

    protected Quaternion previousRot;
    protected float anglesMoved;

    public float MixProgress { get; protected set; }
    public bool Inert { get; private set; }

    protected virtual void Start()
    {
        if (!transform.parent)
        {
            Inert = true;
            return;
        }

        previousRot = transform.parent.localRotation;
    }

    protected virtual void Update()
    {
        if (Inert) return;

        anglesMoved += Quaternion.Angle(previousRot, transform.parent.localRotation);
        MixProgress = anglesMoved / mixDegrees;
        MixAction();

        previousRot = transform.parent.localRotation;

        if (MixProgress >= 1)
        {
            FlavorProfileData.Instance.Add(flavor);
            FullyMixed();
        }
    }

    protected virtual void FullyMixed()
    {
        Destroy(gameObject);
    }

    protected abstract void MixAction();
}