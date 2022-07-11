using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalConch : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            Debug.Log("The game is over!!!");
            RemovalManager.Instance.MoveToNextScene();
        }
    }
}
