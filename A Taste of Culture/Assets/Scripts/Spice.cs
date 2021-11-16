using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spice : MonoBehaviour
{
    enum TypeOfSpice { Garlic, Tumeric, Cumin, Ginger, Peppercorn, Salt };
    
    [SerializeField] TypeOfSpice spiceCategory;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag != "Hand") { return; }

        // TODO: Trigger Tooltip
        Debug.Log("Tooltip: " + spiceCategory);
    }
}
