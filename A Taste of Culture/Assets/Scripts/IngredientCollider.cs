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

    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Knife" && mouseDown && !hasCollided) { hasCollided = true; }
    }

    public void ResetCollider()
    {
        hasCollided = false;
    }
}
