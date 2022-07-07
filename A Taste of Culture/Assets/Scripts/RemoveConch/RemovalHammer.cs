using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalHammer : RemovalTool
{
    [SerializeField] private bool canSetBack;
    [SerializeField] private bool canUse;
    [SerializeField] private bool properArea;

    [SerializeField] private int holeProgress;
    [SerializeField] private bool isUsing;
    [SerializeField] private int hitPower;

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
            RemovalManager.Instance.WarnPlayerHammer();
        }
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        canSetBack = false;
        holeProgress = 0;
        hitPower = 0;
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
            hitPower++;
        }
        else if (canUse && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Use();
        }

        if (isUsing && Input.GetKeyUp(KeyCode.Mouse0))
        {
            holeProgress += hitPower / 2;
            hitPower = 0;
            isUsing = false;

            if (holeProgress >= 1200)
            {
                Debug.LogWarning("This player is going to kill this poor thing with the hammer!");
                
            }
            else if (holeProgress >= 1000)
            {
                Debug.Log("Move to the knife!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11 || collision.gameObject.CompareTag("Knife"))
        {
            canSetBack = true;
        }
        else if (collision.gameObject.layer == 6)
        {
            canUse = true;
            if (collision.gameObject.CompareTag("Guideline"))
            {
                Debug.Log("In proper area");
                properArea = true;
            }
        }
        else
        {
            canUse = false;
            canSetBack = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Knife"))
        {
            canSetBack = true;
        }
        else if (collision.gameObject.layer == 11)
        {
            canSetBack = false;
        }
        else if (collision.gameObject.layer == 6 && !collision.CompareTag("Guideline"))
        {
            canUse = false;
        }
        else if (collision.gameObject.layer == 6 && collision.CompareTag("Guideline"))
        {
            Debug.Log("Out proper area");
            properArea = false;
            canUse = true;
        }
    }
}
