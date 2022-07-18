using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasteSalad : MonoBehaviour
{
    [SerializeField] private FlavorGraphLine flavLineToUpdate;

    private void Update()
    {
        if (flavLineToUpdate.isActiveAndEnabled && Input.GetKeyDown(KeyCode.Space))
        {
            flavLineToUpdate.SetGraphLine();
        }
    }
}