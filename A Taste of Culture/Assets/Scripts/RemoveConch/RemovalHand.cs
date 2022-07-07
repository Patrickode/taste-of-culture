using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalHand : RemovalTool
{
    private bool hoveringTool;
    [SerializeField] private string hoverToolName;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        hoveringTool = false;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (hoveringTool && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hoverToolName == "Hammer")
            {
                RemovalManager.Instance.SetHammer();
            }
            else if (hoverToolName == "Knife")
            {
                RemovalManager.Instance.SetKnife();
            }
            
        }
    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            hoveringTool = true;
            hoverToolName = collision.gameObject.name;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            hoveringTool = false;
            hoverToolName = "";
        }
    }
}
