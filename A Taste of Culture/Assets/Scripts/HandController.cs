using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Animator animator;

    Rigidbody2D rigidbodyComponent;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rigidbodyComponent.position =  mousePosition;               // Make hand move according to mouse location.

        if(Input.GetMouseButtonDown(0))
        {
            // this will pinch the hand on click
            // animator.SetBool("Click", true);
        }

        if(Input.GetMouseButtonUp(0))
        {
            // this will unpinch/open the hand on release"
            // animator.SetBool("Click", false);

            // TODO: Generate spice and have it fall into bowl
        }
    }
}
