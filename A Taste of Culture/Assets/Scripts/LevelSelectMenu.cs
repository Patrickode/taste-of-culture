using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField] private List<LevelSelectData> levelData;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelSelect;

    public void MenuSwitch()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        levelSelect.SetActive(!levelSelect.activeSelf);
    }
}

public class LevelSelectData
{
    public string Name { get; set; }
    public int Level { get; set; }
    public bool InProgress { get; set; }

    public LevelSelectData(string name, int levelNum, bool inProgress = false)
    {
        Name = name;
        Level = levelNum;
        
        if (!inProgress)
        {
            // Check if in progress from the
            // save data etc..
            InProgress = false;
        }
    }
}
