using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorProfileData : Singleton<FlavorProfileData>
{
    private Dictionary<FlavorType, int> flavors = new Dictionary<FlavorType, int>
    {
        { FlavorType.Bitterness, 0 },
        { FlavorType.Spiciness, 0 },
        { FlavorType.Sweetness, 0 },
        { FlavorType.Saltiness, 0 },
    };

    public Dictionary<FlavorType, int> FlavorDict { get => flavors; }

    /// <summary>
    /// <b>Arguments:</b><br/>
    /// - <see cref="FlavorType"/>: The flavor that was updated. -1/Invalid if multiple were 
    /// updated (namely, on reset).<br/>
    /// - <see cref="int"/>: What the flavor was updated to. Check the flavor type for 
    /// validity before use.
    /// </summary>
    public static System.Action<FlavorType, int> FlavorUpdated;

    public int this[FlavorType type]
    {
        get => flavors[type];
        set
        {
            flavors[type] = value;
            FlavorUpdated?.Invoke(type, value);
        }
    }
    public bool TryGetFlav(FlavorType type, out int value) => flavors.TryGetValue(type, out value);

    public void AddFlavor(Dictionary<FlavorType, int> flavor)
    {
        foreach (var typeVal in flavor)
        {
            //Use the indexer property since it handles the invocation of flavor update
            this[typeVal.Key] += typeVal.Value;
        }
    }

    public void ResetData()
    {
        //TODO: Replace with clearing the dict? Add a way to select reset behavior?
        flavors = new Dictionary<FlavorType, int>
        {
            { FlavorType.Bitterness, 0 },
            { FlavorType.Spiciness, 0 },
            { FlavorType.Sweetness, 0 },
            { FlavorType.Saltiness, 0 },
        };

        FlavorUpdated?.Invoke((FlavorType)(-1), 0);
    }

    public int FlavorSum { get => Bitterness + Spiciness + Sweetness + Saltiness; }

    public int Bitterness
    {
        get { return flavors[FlavorType.Bitterness]; }
        set { flavors[FlavorType.Bitterness] = value; }
    }

    public int Spiciness
    {
        get { return flavors[FlavorType.Spiciness]; }
        set { flavors[FlavorType.Spiciness] = value; }
    }

    public int Sweetness
    {
        get { return flavors[FlavorType.Sweetness]; }
        set { flavors[FlavorType.Sweetness] = value; }
    }

    public int Saltiness
    {
        get { return flavors[FlavorType.Saltiness]; }
        set { flavors[FlavorType.Saltiness] = value; }
    }
}
