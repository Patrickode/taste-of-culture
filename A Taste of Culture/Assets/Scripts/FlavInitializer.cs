using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavInitializer : MonoBehaviour
{
    [SerializeField] private bool addToCurrent;
    [SerializeField] private Bewildered.UDictionary<FlavorType, int> initValues;

    private void Awake()
    {
        if (addToCurrent)
        {
            FlavorProfileData.Instance.Add(initValues);
            return;
        }

        FlavorProfileData.Instance.Set(initValues);
    }
}