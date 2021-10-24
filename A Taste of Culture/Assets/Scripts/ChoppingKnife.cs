using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ChoppingKnife : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;                           // Layer to detect colliders on.
    
    public Animator animator;

    Vector2 knifeCenter;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        knifeCenter = gameObject.transform.GetChild(0).gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // this will turn the knife "down" on click
            animator.SetBool("Click", true);

            Chop();
        }

        if(Input.GetMouseButtonUp(0))
        {
            // this will turn the knife "up" on release"
            animator.SetBool("Click", false);
        }
    }

    void Chop()
    {
        Debug.Log("Chop");
        List<GameObject> objectsToCut = new List<GameObject>();

        // Get an array of all ingredients that were hit. 
        RaycastHit2D[] hitObjects = Physics2D.LinecastAll(knifeCenter, knifeCenter, layerMask);       

        foreach(RaycastHit2D hitObject in hitObjects)
        {
            objectsToCut.Add(hitObject.transform.gameObject);
        }    

        foreach(GameObject objectToCut in objectsToCut)
        {
            objectToCut.GetComponent<IngredientCutter>().CutIngredient(knifeCenter, objectToCut);
        }  
    }
}
