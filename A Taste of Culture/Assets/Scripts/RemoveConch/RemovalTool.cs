using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RemovalTool : MonoBehaviour
{
    [SerializeField] private Vector3 startPos;
    [SerializeField] private bool active;

    public bool Active
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
            transform.position = new Vector3(temp.x, temp.y, -3);
        }
    }

    public void ResetPosition()
    {
        transform.position = startPos;
    }

    public abstract void Use();
}
