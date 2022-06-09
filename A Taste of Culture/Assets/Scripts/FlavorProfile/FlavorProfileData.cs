using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorProfileData : Singleton<FlavorProfileData>
{
    private int bitterness, spiciness, sweetness, saltiness = 0;

    public int Bitterness
    {
        get { return bitterness;}
        set { bitterness = value; }
    }

    public int Spiciness
    {
        get { return spiciness;}
        set { spiciness = value; }
    }

    public int Sweetness
    {
        get { return sweetness;}
        set { sweetness = value; }
    }

    public int Saltiness
    {
        get { return saltiness;}
        set { saltiness = value; }
    }
}
