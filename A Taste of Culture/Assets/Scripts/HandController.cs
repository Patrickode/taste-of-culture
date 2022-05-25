using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] Sprite openSprite;
    [SerializeField] Sprite pinchedSprite;
    [SerializeField] Transform spiceSpawnPoint;
    [SerializeField] bool followMouseOffscreen;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidbodyRef;
    Camera cachedCam;

    private GameObject spicePrefab;
    public GameObject SpicePrefab { set { spicePrefab = value; } }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbodyRef = GetComponent<Rigidbody2D>();
        cachedCam = Camera.main;
    }

    private void FixedUpdate()
    {
        Bounds newBounds = spriteRenderer.GetBoundsSansPadding();

        //If the mouse or the hand is on screen, move the hand to the mouse position. If not, don't.
        //This prevents the hand from getting stuck behind cursor-only zones.
        if (followMouseOffscreen ||
            !UtilFunctions.PixelCoordsOffScreen(Input.mousePosition) ||
            !UtilFunctions.PixelCoordsOffScreen(newBounds, cachedCam))
        {
            rigidbodyRef.MovePosition(cachedCam.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    void Update()
    {
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

            //If the mouse and hand are desynced for whatever reason, don't drop any spice
            if (Vector2.Distance(rigidbodyRef.position, cachedCam.ScreenToWorldPoint(Input.mousePosition)) <= 0.1)
                Instantiate(spicePrefab, spiceSpawnPoint.position, Quaternion.identity);

            spicePrefab = null;
        }
    }

    private void TrySetSprite(Sprite spr) { if (spr) spriteRenderer.sprite = spr; }
}
