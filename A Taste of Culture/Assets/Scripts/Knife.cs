using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Knife : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;            // Layer to detect colliders on.
    
    Rigidbody2D rigidbodyComponent;
    IngredientCutter ingredientCutter;
    LineRenderer lineRenderer;

    Vector2 cutStartPosition;
    Vector2 cutEndPosition;

    public SpriteRenderer spriteRenderer;
    public Sprite knifeUp;
    public Sprite knifeDown;
    public Animation animation;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody2D>();
        ingredientCutter = GetComponent<IngredientCutter>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() 
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rigidbodyComponent.position =  mousePosition;           // Make knife move according to mouse location.

        if(Input.GetMouseButtonDown(0))
        {
            cutStartPosition = mousePosition;

            lineRenderer.enabled = true;                        // Start drawing line at initial "cut location".
            lineRenderer.SetPosition(0, cutStartPosition);

            if (spriteRenderer.sprite == knifeUp)
            {
                // ???
                spriteRenderer.sprite = knifeDown;
            }
        }

        if(Input.GetMouseButton(0) && lineRenderer.enabled == true)
        {
            lineRenderer.SetPosition(1, mousePosition);         // Continue drawing line to current location of mouse.
        }

        if(Input.GetMouseButtonUp(0))
        {
            cutEndPosition = mousePosition;

            lineRenderer.enabled = false;

            CutObjects(cutStartPosition, cutEndPosition);

            if (spriteRenderer.sprite == knifeDown)
            {
                // ???
                spriteRenderer.sprite = knifeUp;
            }
        }
    } 

    void CutObjects(Vector2 startPosition, Vector2 endPosition)
    {
        List<GameObject> objectsToCut = new List<GameObject>();

        // Get an array of all ingredients that were hit. 
        RaycastHit2D[] hitObjects = Physics2D.LinecastAll(startPosition, endPosition, layerMask);       

        foreach(RaycastHit2D hitObject in hitObjects)
        {
            objectsToCut.Add(hitObject.transform.gameObject);
        }    

        foreach(GameObject objectToCut in objectsToCut)
        {
            // if(ingredientCutter == null) { break; }              // If ingredient cutter isn't present stop trying to cut objects.
            // ingredientCutter.CutIngredient(startPosition, endPosition, objectToCut);
            
            objectToCut.GetComponent<CuttableIngredient>().CutIngredient(startPosition, endPosition);
        }
    }
}
