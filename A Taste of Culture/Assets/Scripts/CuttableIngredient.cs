using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableIngredient : MonoBehaviour
{
    Vector2 cutStartPosition;
    Vector2 cutEndPosition;

    LineRenderer cutLine;

    Knife knife;

    // Start is called before the first frame update
    void Start()
    {
        cutLine = GetComponent<LineRenderer>();

        knife = FindObjectOfType<Knife>();
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Knife")
        {
            if(knife.IsCutting)
            {
                Debug.Log("AHH YOU'RE CUTTING ME!!!");

                cutStartPosition = other.gameObject.transform.position;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Knife")
        {
            if(knife.IsCutting)
            {
                Debug.Log("YOU MONSTER!!!");
                
                cutEndPosition = other.gameObject.transform.position;

                DrawCutLine();
            }
        }
    }

    void DrawCutLine()
    {
        cutLine.SetPosition(0, cutStartPosition);
        cutLine.SetPosition(1, cutEndPosition);
    }
}
