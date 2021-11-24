using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] Sprite openSprite;
    [SerializeField] Sprite pinchedSprite;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbodyComponent;

    private GameObject spicePrefab;
    public GameObject SpicePrefab { set { spicePrefab = value; } }

    Vector2 spawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbodyComponent = GetComponent<Rigidbody2D>();

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rigidbodyComponent.position =  mousePosition;               // Make hand move according to mouse location.

        if(Input.GetMouseButtonDown(0))
        {
            if(spriteRenderer != null && pinchedSprite != null) { spriteRenderer.sprite = pinchedSprite; }
        }

        if(Input.GetMouseButtonUp(0))
        {
            StartCoroutine(DropSpice());

            if(spriteRenderer != null && openSprite != null) { spriteRenderer.sprite = openSprite; }
        }
    }

    // Generate spice and have it fall into bowl
    // TODO: Test -> Does this need to be a coroutine?
    IEnumerator DropSpice()
    {
        spawnPosition = gameObject.transform.GetChild(0).gameObject.transform.position;

        if(spicePrefab != null) 
        { 
            Instantiate(spicePrefab, spawnPosition, Quaternion.identity);
        }

        yield return new WaitForSeconds(.1f);

        spicePrefab = null;
    }
}
