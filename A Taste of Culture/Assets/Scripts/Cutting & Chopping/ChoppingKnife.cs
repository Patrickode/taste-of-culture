using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChoppingKnife : MonoBehaviour
{
    [SerializeField] private Transform knifeTip;
    [SerializeField] private Transform knifeBase;
    [Space(5)]
    [UnityEngine.Serialization.FormerlySerializedAs("layerMask")]
    [SerializeField] private LayerMask layersToChop;
    [SerializeField] private float knifeDownDuration = 0.1f;

    public Animator animator;

    private AudioSource choppingAudio;

    private bool canChop = true;
    public bool CanChop { set { canChop = value; } }

    private bool madeFirstCut = false;

    private void Start()
    {
        choppingAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canChop)
        {
            Chop();
            choppingAudio.Play();

            //true = knife is down, false = knife is up
            SetChopAnim(true);
            Coroutilities.DoAfterDelay(this, () => SetChopAnim(false), knifeDownDuration);
        }
    }

    private void Chop()
    {
        RaycastHit2D[] ingrHits = Physics2D.LinecastAll(knifeTip.position, knifeBase.position, layersToChop);

        foreach (RaycastHit2D hit in ingrHits)
        {
            if (hit.transform.TryGetComponent(out DoubleIngredient doubIngr))
            {
                TryCutIngr(doubIngr.Ingredient1, doubIngr.Mover);
                TryCutIngr(doubIngr.Ingredient2, doubIngr.Mover);
            }
            else
            {
                TryCutIngr(hit.transform.parent);
            }
        }
    }

    private void TryCutIngr(GameObject ingr, IngredientMover moverToReference = null)
    {
        //Unity warns not to use null coalescing on unity objects (rightly), but it's fine in this specific
        //case; we only want to do this if the mover is *exactly* null (i.e., has the default value)
        moverToReference ??= ingr.GetComponent<IngredientMover>();

        //If the ingredient can be moved, we've already cut this ingredient (unless this is the first cut), so
        //bail out; no need to cut again.
        if (madeFirstCut && (!moverToReference || moverToReference.AllowMovement))
            return;

        madeFirstCut = true;
        ingr.GetComponentInChildren<IngredientCutter>().CutIngredient(knifeTip.position);
    }
    private void TryCutIngr(Component ingrSource) => TryCutIngr(ingrSource.gameObject);

    private void SetChopAnim(bool value)
    {
        animator.SetBool("Click", value);
    }
}
