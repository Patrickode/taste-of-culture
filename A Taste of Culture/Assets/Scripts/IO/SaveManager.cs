using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelID { Generic, Makhani, ConchSalad }

[Flags]
public enum ChoiceFlags
{
    None = 0,
    Tofu = 1 << 0,
    Chicken = 1 << 1,
}

[Serializable]
public struct LevelData
{
    public LevelID level;
    public int sceneIndex;
    public Dictionary<FlavorType, int> flavorProfile;
    public ChoiceFlags choices;
}

public static class SaveManager
{
    public static readonly string fileExt = ".sav";

    private static Dictionary<LevelID, LevelData> cachedData;

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
        //Go through the scene's path and try to get the number next to "Level " (case insensitive)
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

    //This attribute and argument make this method run when indicated. Afaik, they're all on
    //game startup, just at different points of startup.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded += SaveOnLoad;
    }

    public static void SaveLevelData(LevelData dataToSave, string filename = "generic")
    {
        BinaryFormatter binFormatter = new BinaryFormatter();
        string dataPath = Path.Combine(
            Application.persistentDataPath,
            filename + fileExt);

        using FileStream stream = new FileStream(dataPath, FileMode.Create);
        binFormatter.Serialize(stream, dataToSave);

        cachedData[dataToSave.level] = dataToSave;
    }

    private static void SaveOnLoad(Scene sceneLoaded, LoadSceneMode loadMode)
    {
        LevelID lvl = ScnIndToLvlID(sceneLoaded);

        SaveLevelData(new LevelData()
        {
            level = lvl,
            sceneIndex = sceneLoaded.buildIndex,
            flavorProfile = FlavorProfileData.Instance.FlavorDict
        }, Enum.GetName(typeof(LevelID), lvl));
    }

    /// <summary>
    /// Attempts to get cached level data by ID; failing that, attempts to load that level data from a file.
    /// <br/><br/>
    /// Returns a nullable <see cref="LevelData"/>; check it for validity with <see langword="is"/> or similar.
    /// </summary>
    public static LevelData? GetLevelData(LevelID idToLoad)
    {
        //If we already have data cached, don't load any, just get that.
        if (cachedData.TryGetValue(idToLoad, out LevelData cacheData))
            return cacheData;

        string dataPath = Path.Combine(
            Application.persistentDataPath,
            Enum.GetName(typeof(LevelID), idToLoad) + fileExt);

        if (!File.Exists(dataPath)) return null;

        BinaryFormatter binFormatter = new BinaryFormatter();
        using FileStream stream = new FileStream(dataPath, FileMode.Open);

        if (binFormatter.Deserialize(stream) is LevelData lvlData)
        {
            //If we got here, there's no data cached, so cache what we just loaded.
            cachedData[idToLoad] = lvlData;
            return lvlData;
        }

        return null;
    }
}