using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreMoneyManager : MonoBehaviour
{
    public static OreMoneyManager instance;

    public OreObject[] oreObjects = new OreObject[8];

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void UpdateMyOre()
    {

    }
}
