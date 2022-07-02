using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalManager : MonoBehaviour
{
    public RemovalTool current;
    [SerializeField] private RemovalTool hand;

    private void Start()
    {
        current = hand;
    }

    public void ResetCurrentTool()
    {
        if (current.name == "Hammer")
        {
            // place in hamer position
        }
        else if (current.name == "Knife")
        {
            // place in knife position
        }

        current = hand;
    }
}
