using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCollider : MonoBehaviour
{
    private bool hasCollided;
    public bool HasCollided { get { return hasCollided; } }

    bool mouseDown;

    void Update() 
    {
        if(Input.GetMouseButtonDown(0)) { mouseDown = true; }
        if(Input.GetMouseButtonUp(0)) { mouseDown = false; }
    }

    // void OnTriggerEnter2D(Collider2D other) 
    // {
    //     // if(other.gameObject.tag == "Knife" && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))) { hasCollided = true; }
    //     if(other.gameObject.tag == "Knife" && mouseDown) { hasCollided = true; }
    //     Debug.Log("Has Collided: " + hasCollided);
    // }
    
    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Knife" && mouseDown && !hasCollided) { hasCollided = true; }
        Debug.Log(gameObject.name + " Has Collided: " + hasCollided);
    }

    // void OnMouseDown() 
    // {
    //     hasCollided = true;
    //     Debug.Log("Has Collided: " + hasCollided);
    // }

    public void ResetCollider()
    {
        Debug.Log(gameObject.name + " Reset Collider");
        hasCollided = false;
    }
}
