using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelID { Generic, Makhani, ConchSalad }

[Flags]
public enum ChoiceFlag
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
    public ChoiceFlag choices;

    public LevelData(LevelID level = default, int sceneIndex = default,
        Dictionary<FlavorType, int> flavorProfile = default, ChoiceFlag choices = default)
    {
        this.level = level;
        this.sceneIndex = sceneIndex;
        this.flavorProfile = flavorProfile;
        this.choices = choices;
    }

    public LevelData(LevelData other) : this(
        other.level, other.sceneIndex, other.flavorProfile, other.choices)
    { }
}

public static class DataManager
{
    public static readonly string fileExt = ".sav";

    private static readonly HashSet<int> dontSaveScenes = new HashSet<int>()
    {
        1,  //IntroDialogue
    };

    private static Dictionary<LevelID, LevelData> cachedData = new Dictionary<LevelID, LevelData>();

    //This attribute and argument make this method run when indicated. Afaik, they're all on
    //game startup, just at different points (so they won't make this run more than once).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded += SaveOnLoad;
    }

    private static void SaveOnLoad(Scene sceneLoaded, LoadSceneMode loadMode)
    {
        if (dontSaveScenes.Contains(sceneLoaded.buildIndex)) return;
        LevelID lvlID = ScnIndToLvlID(sceneLoaded);

        //If we have cached data, get it and only overwrite the stuff that changed on load.
        if (cachedData.TryGetValue(lvlID, out LevelData lvlData))
        {
            lvlData.level = lvlID;
            lvlData.sceneIndex = sceneLoaded.buildIndex;
            lvlData.flavorProfile = FlavorProfileData.Instance.FlavorDict;
        }
        else lvlData = new LevelData(lvlID, sceneLoaded.buildIndex, FlavorProfileData.Instance.FlavorDict);

        SaveLevelData(lvlData, IDToName(lvlID));
    }

    public static void SaveLevelData(LevelData dataToSave, string filename = "")
    {
        //If no name was passed, get the name of the level the current scene is part of. Failing that, "Generic".
        if (string.IsNullOrWhiteSpace(filename))
            filename = IDToName(ScnIndToLvlID(SceneManager.GetActiveScene()));

        BinaryFormatter binFormatter = new BinaryFormatter();
        using FileStream stream = new FileStream(MakePathWithFilename(filename), FileMode.Create);

        binFormatter.Serialize(stream, dataToSave);
        cachedData[dataToSave.level] = dataToSave;
    }

    public static void SaveChoice(LevelID idToSaveAt, ChoiceFlag choice, bool overwriteAll = false)
    {
        if (cachedData.TryGetValue(idToSaveAt, out LevelData data))
        {
            data.choices = overwriteAll ? choice : data.choices | choice;
        }
        else data = new LevelData(choices: choice);

        SaveLevelData(data, IDToName(idToSaveAt));
    }

    /// <summary>
    /// Attempts to get cached level data by ID; failing that, attempts to load that level data from a file.
    /// <br/><br/>
    /// Returns a nullable <see cref="LevelData"/>; check it for validity with <see langword="is"/> or similar.
    /// </summary>
    public static LevelData? GetLevelData(LevelID idToLoad)
    {
        //If we already have data cached, don't load any, just get that.
        if (cachedData.TryGetValue(idToLoad, out LevelData cData))
            return cData;

        return LoadLevelData(idToLoad, true);
    }

    /// <summary>
    /// Attempts to load level data for the ID passed (returning null on failure; use <see langword="is"/> or
    /// similar to check for validity).
    /// <br/><br/>
    /// <b>Use <see cref="GetLevelData(LevelID)"/> instead</b> unless you want to force a load (to avoid redundant loads).
    /// </summary>
    /// <param name="updateCache">If we successfully load data, should we update the cache with that data?</param>
    public static LevelData? LoadLevelData(LevelID idToLoad, bool updateCache = true)
    {
        string dataPath = MakePathWithFilename(IDToName(idToLoad));
        if (!File.Exists(dataPath)) return null;

        BinaryFormatter binFormatter = new BinaryFormatter();
        using FileStream stream = new FileStream(dataPath, FileMode.Open);

        if (binFormatter.Deserialize(stream) is LevelData lvlData)
        {
            if (updateCache)
                cachedData[idToLoad] = lvlData;

            return lvlData;
        }

        return null;
    }

    public static void DeleteLevelData(LevelID idToDelete)
    {
        File.Delete(MakePathWithFilename(IDToName(idToDelete)));
        cachedData.Remove(idToDelete);
    }

    public static void ResetAllData()
    {
        foreach (var id in (LevelID[])Enum.GetValues(typeof(LevelID)))
            DeleteLevelData(id);
    }

    private static string MakePathWithFilename(string filename)
        => Path.Combine(Application.persistentDataPath, filename + fileExt);

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
        var lvlNumMatch = System.Text.RegularExpressions.Regex.Match(scn.path, @"(?i)(?<=Level\s)\d*");

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

    public static string IDToName(LevelID id) => Enum.GetName(typeof(LevelID), id);
}