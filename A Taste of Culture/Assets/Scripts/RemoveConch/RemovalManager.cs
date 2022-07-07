using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalManager : MonoBehaviour
{
    public RemovalTool current;
    [SerializeField] private RemovalTool hand;
    [SerializeField] private RemovalTool hammer;
    [SerializeField] private RemovalTool knife;
    public static RemovalManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);

        current = hand;
    }

    public void WarnPlayerHammer()
    {
        Debug.Log("Being used Wrong");
    }

    public void ResetCurrentTool()
    {
        if (current.name == "Hammer")
        {
            // place in hamer position
            hammer.Active = false;
            hammer.ResetPosition();
        }
        else if (current.name == "Knife")
        {
            // place in knife position
            knife.ResetPosition();
            knife.Active = false;
        }

        current = hand;
        hand.Active = true;
    }

    public void ResetHand()
    {
        hand.Active = false;
        hand.ResetPosition();
    }

    public void SetHammer()
    {
        hammer.Active = true;
        current = hammer;

        ResetHand();
    }

    public void SetKnife()
    {
        knife.Active = true;
        current = knife;

        ResetHand();
    }
}
