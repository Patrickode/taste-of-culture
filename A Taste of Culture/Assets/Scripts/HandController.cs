using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] Sprite openSprite;
    [SerializeField] Sprite pinchedSprite;
    [SerializeField] Transform spiceSpawnPoint;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbodyRef;

    private GameObject spicePrefab;
    public GameObject SpicePrefab { set { spicePrefab = value; } }

    Vector2 spawnPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbodyRef = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Make hand move according to mouse location.
        rigidbodyRef.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!spriteRenderer)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            TrySetSprite(pinchedSprite);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            TrySetSprite(openSprite);

            if (!spicePrefab)
                return;

            Instantiate(spicePrefab, spiceSpawnPoint.position, Quaternion.identity);
            spicePrefab = null;
        }
    }

    private void TrySetSprite(Sprite spr) { if (spr) spriteRenderer.sprite = spr; }
}
