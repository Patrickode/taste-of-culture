using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RemovalTool : MonoBehaviour
{
    [SerializeField] protected Vector3 startPos;
    [SerializeField] protected bool active;

    [SerializeField] protected Vector2 offset;
    [SerializeField] protected int heldZ;

    [SerializeField] protected bool canUse;
    [SerializeField] protected bool properArea;

    public virtual bool Active
    {
        get { return active; }
        set { active = value; }
    }

    public Vector3 StartPosition
    {
        get { return startPos; }
    }
    // Start is called before the first frame update
    protected void Start()
    {
        transform.position = startPos;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (active)
        {
            Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(temp.x + offset.x, temp.y + offset.y, heldZ);
        }
    }

    public void ResetPosition()
    {
        transform.position = startPos;
        canUse = false;
        properArea = false;
    }

    public abstract void Use();
}
