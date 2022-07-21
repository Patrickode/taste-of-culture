using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelID { Generic, Makhani, ConchSalad }

[System.Serializable]
public struct LevelData
{
    public LevelID level;
    public int sceneIndex;
    public Dictionary<FlavorType, int> flavorProfile;
}

public static class SaveManager
{
    public static readonly string fileExt = ".sav";

    /// <summary>
    /// Takes a scene build index and returns the level that scene belongs to.<br/>
    /// <b>This overload must be manually updated through code whenever scenes are reordered/added/removed.</b>
    /// </summary>
    /// <remarks>Implementation inspired by <see href="https://stackoverflow.com/q/56676260"/>.</remarks>
    public static LevelID ScnIndToLvlID(int index) => index switch
    {
        var i when (i >= 1 && i <= 6) => LevelID.Makhani,
        var i when (i >= 7 && i <= 9) => LevelID.ConchSalad,
        _ => LevelID.Generic,
    };

    /// <summary>
    /// Takes a scene and returns the level it belongs to.<br/>
    /// This method uses Regex to look at the scene's path, which should contain<br/>what number level it's 
    /// in (for example in the name of its folder).
    /// <br/><br/>
    /// If "Level #" (case insensitive) can't be found in the path, returns <see cref="LevelID.Generic"/>.
    /// </summary>
    public static LevelID ScnIndToLvlID(Scene scn)
    {
        //Go through the scene's path and get the number next to "Level " (case insensitive), if there is one.
        var lvlNumMatch = System.Text.RegularExpressions.Regex.Match(scn.path, @"(?<=(?i)Level\s(?-i))\d*");

        if (lvlNumMatch.Success && int.TryParse(lvlNumMatch.Value, out int lvlNum))
        {
            return lvlNum switch
            {
                1 => LevelID.Makhani,
                2 => LevelID.ConchSalad,
                _ => LevelID.Generic,
            };
        }

        return LevelID.Generic;
    }

    //This attribute and argument make this method run just before the unity splash screen plays; I.e., on game startup.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Init()
    {
        SceneManager.sceneLoaded += SaveOnLoad;
    }

    public static void SaveLevelData(LevelData dataToSave, string filename = "generic")
    {
        BinaryFormatter binFormatter = new BinaryFormatter();
        string dataPath = Path.Combine(Application.persistentDataPath, filename + fileExt);

        using FileStream stream = new FileStream(dataPath, FileMode.Create);
        binFormatter.Serialize(stream, dataToSave);
    }

    private static void SaveOnLoad(Scene sceneLoaded, LoadSceneMode loadMode)
    {
        LevelID lvl = ScnIndToLvlID(sceneLoaded);

        SaveLevelData(new LevelData()
        {
            level = lvl,
            sceneIndex = sceneLoaded.buildIndex,
            flavorProfile = FlavorProfileData.Instance.FlavorDict
        }, System.Enum.GetName(typeof(LevelID), lvl));
    }
}