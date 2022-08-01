using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite pinchedSprite;
    [SerializeField] private Transform spiceSpawnPoint;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private float distanceTillDesynced = 0.5f;
    [SerializeField] private bool followMouseOffscreen;
    [SerializeField] private int startingSpiceOrder;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbodyRef;
    private Camera cachedCam;
    private int layerDropCounter;

    private SpriteRenderer spicePrefab;
    public SpriteRenderer SpicePrefab { set { spicePrefab = value; } }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbodyRef = GetComponent<Rigidbody2D>();
        cachedCam = Camera.main;
        layerDropCounter = startingSpiceOrder;
    }

    private void FixedUpdate()
    {
        //Move the rigidbody toward the mouse, clamped to the screen's edge + the size of the hand's
        //rendered sprite. This prevents the hand from getting stuck behind cursor-only zones.
        rigidbodyRef.MovePosition(GetClampedTarget());

        #region Alternate Behavior; Stop tracking mouse entirely when offscreen
        /*
        if (followMouseOffscreen
            || Vector2.Distance(UtilFunctions.PixelCoordsOffScreen(
                Input.mousePosition), Vector2.zero) < 0.01
            || Vector2.Distance(UtilFunctions.PixelCoordsOffScreen(
                spriteRenderer.GetBoundsSansPadding(), cachedCam), Vector2.zero) < 0.01)
        {
            rigidbodyRef.MovePosition(cachedCam.ScreenToWorldPoint(Input.mousePosition));
        }
        */
        #endregion
    }

    void Update()
    {
        if (!spriteRenderer)
            return;

        bool handSynced = Vector2.Distance(rigidbodyRef.position,
            cachedCam.ScreenToWorldPoint(Input.mousePosition)) <= distanceTillDesynced;

        crosshair.SafeSetActive(handSynced);

        //If the mouse and hand are desynced for whatever reason, don't pick/drop any spice
        if (handSynced && Input.GetMouseButtonDown(0))
        {
            TrySetSprite(pinchedSprite);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            TrySetSprite(openSprite);

            if (handSynced)
            {
                if (!spicePrefab)
                    return;
                //Spawn the spice, increment the drop counter, then set the new spice's sort order to the counter
                SpriteRenderer newSpice = Instantiate(spicePrefab, spiceSpawnPoint.position, Quaternion.identity);
                newSpice.sortingOrder = ++layerDropCounter;
            }

            spicePrefab = null;
        }
    }

    private Vector2 GetClampedTarget()
    {
        if (followMouseOffscreen)
            return rigidbodyRef.position;

        //First, get how far the hand bounds are offscreen.
        Bounds bnds = spriteRenderer.GetBoundsSansPadding();
        Vector2 boundsOffscreen = UtilFunctions.PixelsOffscreen(bnds, cachedCam);

        //Next, use those bounds to determine the closest corner to the screen's edge.
        Vector2 cornerClosestToScreen = Vector2.zero;
        cornerClosestToScreen.x = boundsOffscreen.x < 0 ? bnds.max.x : bnds.min.x;
        cornerClosestToScreen.y = boundsOffscreen.y < 0 ? bnds.max.y : bnds.min.y;

        //Then get the direction/distance to the hand pivot from that corner.
        Vector2 cornerToPivot = cornerClosestToScreen - rigidbodyRef.position;

        //Init:
        // - mouse world position,
        // - how many pixels the closest corner would be offscreen if it were following the mouse,
        // - and the target, which defaults to the current position (i.e., don't move)
        Vector2 worldMouse = cachedCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 closeCornerOffscreen = UtilFunctions.PixelsOffscreen(worldMouse + cornerToPivot, cachedCam);
        Vector2 target = rigidbodyRef.position;

        //Finally, if the bounds are on screen (zero pixels off), or they *WOULD* be on screen if they were
        //following the mouse, set the target to the mouse's world position on that axis.
        if (boundsOffscreen.x.EqualWithinRange(0, 0.01f) || closeCornerOffscreen.x.EqualWithinRange(0, 0.01f))
            target.x = worldMouse.x;

        if (boundsOffscreen.y.EqualWithinRange(0, 0.01f) || closeCornerOffscreen.y.EqualWithinRange(0, 0.01f))
            target.y = worldMouse.y;

        return target;
    }

    private void TrySetSprite(Sprite spr) { if (spr) spriteRenderer.sprite = spr; }
}
