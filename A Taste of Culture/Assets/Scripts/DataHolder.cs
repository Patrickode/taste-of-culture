using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : Singleton<DataHolder>
{
    [Header("Data Holder Fields")]
    [SerializeField] private int[] spices;
    [SerializeField] private int NumSpices = 8;

    // Start is called before the first frame update
    void Start()
    {
        spices = new int[NumSpices];
        for (int i = 0; i < NumSpices; i++)
        {
            spices[i] = 0;
        }
    }

    public int Cayenne
    {
        get { return spices[0]; }
        set { spices[0] = value; }
    }

    public int Cumin
    {
        get { return spices[1]; }
        set { spices[1] = value; }
    }

    public int Ginger
    {
        get { return spices[2]; }
        set { spices[2] = value; }
    }

    public int Garlic
    {
        get { return spices[3]; }
        set { spices[3] = value; }
    }

    public int Paprika
    {
        get { return spices[4]; }
        set { spices[4] = value; }
    }

    public int Cinnamon
    {
        get { return spices[5]; }
        set { spices[5] = value; }
    }

    public int Nutmeg
    {
        get { return spices[6]; }
        set { spices[6] = value; }
    }

    public int Coriander
    {
        get { return spices[7]; }
        set { spices[7] = value; }
    }
}
