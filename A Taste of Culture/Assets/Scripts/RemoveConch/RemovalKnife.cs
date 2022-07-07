using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalKnife : RemovalTool
{
    private bool canSetBack;

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
        throw new System.NotImplementedException();
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            canSetBack = true;
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
    }
}
