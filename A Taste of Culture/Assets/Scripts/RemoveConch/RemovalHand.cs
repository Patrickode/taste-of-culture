using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalHand : RemovalTool
{
    private bool hoveringTool;
    [SerializeField] private string hoverToolName;

    [SerializeField] private Sprite idleHand;
    [SerializeField] private Sprite grabHand;
    [SerializeField] private SpriteRenderer render;

    [SerializeField] private Vector2 useStartPos;
    [SerializeField] private Vector2 useDistance;
    [SerializeField] private float resistScalar;
    [SerializeField] private int framesDown;

    [SerializeField] private GameObject animal;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        hoveringTool = false;
        render.sprite = idleHand;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            render.sprite = grabHand;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            render.sprite = idleHand;
        }

        if (hoveringTool && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hoverToolName == "Hammer")
            {
                RemovalManager.Instance.SetHammer();
            }
            else if (hoverToolName == "Knife")
            {
                RemovalManager.Instance.SetKnife();
            }
            
        }

        if (isUsing && Input.GetKey(KeyCode.Mouse0))
        {
            framesDown++;
            if (framesDown % 20 == 0)
            {
                useStartPos = transform.position;
            }
            else if (framesDown % 20 == 19)
            {
                useDistance = (Vector2)transform.position - useStartPos;
                useDistance *= resistScalar;

                float x = useDistance.x > 0 ? 0 : useDistance.x;
                float y = useDistance.y < 0 ? 0 : useDistance.y;
                useDistance = new Vector2(x, y);

                Vector3 pos = animal.transform.position;
                pos = new Vector3(pos.x + useDistance.x, pos.y + useDistance.y, 0);

                animal.transform.position = new Vector3(pos.x + useDistance.x, pos.y + useDistance.y, 0);
            }
        }
        else if (canUse && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Use();
        }

        if (isUsing && Input.GetKeyUp(KeyCode.Mouse0))
        {
            isUsing = false;
        }
    }

    public override void Use()
    {
        if (properArea)
        {
            Debug.Log("Being used properly");
            isUsing = true;
            useDistance = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            hoveringTool = true;
            hoverToolName = collision.gameObject.name;
        }
        else if (collision.gameObject.layer == 6)
        {
            if (collision.gameObject.CompareTag("Ingredient"))
            {
                properArea = true;
                canUse = true;
                Debug.Log("In Proper area");
            }
            else if (collision.gameObject.CompareTag("Guideline"))
            {
                canUse = true;
                Debug.Log("In Wrong area");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            hoveringTool = false;
            hoverToolName = "";
        }
        else if (collision.gameObject.layer == 6)
        {
            canUse = false;
            if (collision.gameObject.CompareTag("Ingredient"))
            {
                properArea = false;
                isUsing = false;
                Debug.Log("Left Proper area");
            }
            else if (collision.gameObject.CompareTag("Guideline"))
            {
                Debug.Log("Left Wrong area");
            }
        }
    }
}
