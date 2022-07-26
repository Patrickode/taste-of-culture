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

    private int continueTargetInd = 0;

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
                continueTargetInd = lvlData.sceneIndex;
            }
            else
            {
                continueButton.interactable = false;
            }
        }
    }

    public void LoadSceneAtDataInd() => Transitions.LoadWithTransition?.Invoke(continueTargetInd, -1);
    public void SetFlavProfileWithData()
    {
        if (DataManager.GetLevelData(level) is LevelData data)
            FlavorProfileData.Instance.FlavorDict = data.flavorProfile;
    }
}
