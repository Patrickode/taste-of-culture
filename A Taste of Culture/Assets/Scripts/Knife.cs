using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Knife : MonoBehaviour
{
    Rigidbody2D rigidbodyComponent;

    private bool isCutting;
    public bool IsCutting { get { return isCutting; } }

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        rigidbodyComponent.position =  mousePosition;

        if(Input.GetMouseButtonDown(0)) { isCutting = true; }
        else if(Input.GetMouseButtonUp(0)) { isCutting = false; }
    }
}
