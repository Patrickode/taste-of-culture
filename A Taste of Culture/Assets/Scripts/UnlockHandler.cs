using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockHandler : MonoBehaviour
{
    public static bool TEMP_Level1Entered;
    public void TEMP_SetLvl1Entered() => TEMP_Level1Entered = true;

    [SerializeField] private bool lockedByDefault;

    private void Start()
    {
        //TODO: Replace this with something less temporary
        //If locked by default or level 1 hasn't been entered, deactivate.
        gameObject.SetActive(!lockedByDefault || TEMP_Level1Entered);
    }
}
