using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasteSalad : MonoBehaviour
{
    [SerializeField] private FlavorProfile profileToUpdate;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            profileToUpdate.UpdateFlavors();
        }
    }
}