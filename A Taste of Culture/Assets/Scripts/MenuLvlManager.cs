using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLvlManager : MonoBehaviour
{
    [SerializeField] private bool lockedByDefault;
    [SerializeField] private LevelID level;
    [Space(5)]
    [SerializeField] private UnityEngine.UI.Button startButton;
    [SerializeField] private UnityEngine.UI.Button continueButton;

    public static bool TEMP_Level1Entered;
    public void TEMP_SetLvl1Entered() => TEMP_Level1Entered = true;

    private void Start()
    {
        //TODO: Replace this with something less temporary
        //If locked by default or level 1 hasn't been entered, deactivate.
        gameObject.SetActive(!lockedByDefault || TEMP_Level1Entered);

        if (continueButton)
        {
            if (DataManager.GetLevelData(level) is LevelData lvlData)
            {
                continueButton.interactable = true;
                //Add method that the button calls on click; that method uses target index which is set using lvlData
            }
            else
            {
                continueButton.interactable = false;
            }
        }
    }
}
