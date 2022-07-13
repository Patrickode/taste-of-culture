using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalKnife : RemovalTool
{
    [Header("Removal Knife Fields")]
    [SerializeField] private bool canSetBack;
    [Space(5)]
    [SerializeField] private float unstickProgress;

    public override bool Active
    {
        get => base.Active;
        set
        {
            base.Active = value;
            if (value)
            {
                canSetBack = false;
            }
        }
    }

    public override void Use()
    {
        if (properArea)
        {
            Debug.Log("Being used properly");
            isUsing = true;
        }
        else
        {
            RemovalManager.Instance.WarnPlayerHammer(); // change to knife
        }
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (canSetBack && Input.GetKeyDown(KeyCode.Mouse0))
        {
            RemovalManager.Instance.ResetCurrentTool();
        }

        if (isUsing && Input.GetKey(KeyCode.Mouse0))
        {
            unstickProgress += Time.deltaTime;
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if (canUse && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Use();
        }

        if (isUsing && Input.GetKeyUp(KeyCode.Mouse0))
        {
            isUsing = false;
            GetComponent<SpriteRenderer>().color = Color.white;

            if (unstickProgress >= 5.0f)
            {
                Debug.Log("Move to the hand!");
                heldZ = -5;
                RemovalManager.Instance.StartHandPlay();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            canSetBack = true;
        }
        else if (collision.gameObject.layer == 6)
        {
            if (collision.gameObject.CompareTag("Ingredient"))
            {
                Debug.Log("In proper area");
                properArea = true;
            }
            canUse = true;
        }
        else
        {
            canSetBack = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            canSetBack = false;
        }
        else if (collision.gameObject.layer == 6 && !collision.CompareTag("Ingredient"))
        {
            canUse = false;
        }
        else if (collision.gameObject.layer == 6 && collision.CompareTag("Ingredient"))
        {
            Debug.Log("Out proper area");
            properArea = false;
            canUse = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ForbiddenZone"))
        {
            canUse = false;
            isUsing = false;

            Debug.LogWarning("This person is going to kill this thing with the knife!");
            // do dialogue warning for knife maybe?
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        canUse = true;
    }
}
