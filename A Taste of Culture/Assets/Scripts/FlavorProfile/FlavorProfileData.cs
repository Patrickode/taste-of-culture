using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorProfileData : Singleton<FlavorProfileData>
{
    private Dictionary<FlavorType, int> flavors = new Dictionary<FlavorType, int>
    {
        {FlavorType.Bitterness, 0 },
        {FlavorType.Spiciness, 0 },
        {FlavorType.Sweetness, 0 },
        {FlavorType.Saltiness, 0 },
    };

    public Dictionary<FlavorType, int> FlavorDict { get => flavors; }

    public int this[FlavorType type]
    {
        get => flavors[type];
        set => flavors[type] = value;
    }

    public void AddFlavor(Dictionary<FlavorType, int> flavor)
    {
        foreach (var typeVal in flavor)
        {
            flavors[typeVal.Key] += typeVal.Value;
        }
    }

    public void ResetData()
    {
        flavors = new Dictionary<FlavorType, int>
        {
            {FlavorType.Bitterness, 0 },
            {FlavorType.Spiciness, 0 },
            {FlavorType.Sweetness, 0 },
            {FlavorType.Saltiness, 0 },
        };
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
