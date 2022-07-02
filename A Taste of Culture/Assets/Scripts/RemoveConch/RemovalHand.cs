using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalHand : RemovalTool
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnEnter");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (collision.gameObject.layer)
            {
                case 7:
                    RemovalTool t = collision.gameObject.GetComponent<RemovalTool>();
                    if (t is RemovalHammer h)
                    {
                        Debug.Log("I did it :)");
                    }
                    else if (t is RemovalKnife k)
                    {
                        Debug.Log("I did it :)");
                    }

                    //if (t.GetType() == typeof(RemovalHammer))
                    //{
                    //    Debug.Log("I did it :)");
                    //}
                    break;

                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnEnter");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (collision.gameObject.layer)
            {
                case 7:
                    RemovalTool t = collision.gameObject.GetComponent<RemovalTool>();
                    if (t is RemovalHammer h)
                    {
                        Debug.Log("I did it :)");
                    }
                    else if (t is RemovalKnife k)
                    {
                        Debug.Log("I did it :)");
                    }

                    //if (t.GetType() == typeof(RemovalHammer))
                    //{
                    //    Debug.Log("I did it :)");
                    //}
                    break;

                default:
                    break;
            }
        }
    }
}
