using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class TerretoryData
{
    public int TerretoryID;
    public TerritoryType Type;

    public Owner Owner;

    public int StartingUnits = 0;

    public float scale;

    [Tooltip("Position on the map in world space. Set by MapGenerator.")]
    public Vector2 position;

    [Tooltip("Max units this territory can hold.")]
    public int maxCapacity = 50;

    [Tooltip("Buff percentage")]
    public float buffPercentage = 0;

    [Tooltip("Units produced per second.")]
    public float productionRate = 1f;

    public TerretoryData TerritoryTier(int tierProduction, int tierCapacity,int tierBuff, TerritoryType terType)
    {
        float tProduction = tierProduction - 1;
        float tCapacity = tierCapacity - 1;
        float tBuff = tierBuff - 1;

        switch (terType)// change production and capacity for the territory for now for the specific type
        {
            case TerritoryType.SoldierProd:
                productionRate = 1.7f;
                maxCapacity = 40;
                buffPercentage = 2.5f;
                break;
            case TerritoryType.DwarfProd:
                //half the production rate and lower capacity for it 
                productionRate = 2.4f;
                maxCapacity = 34;
                buffPercentage = 2.5f;
                break;
            case TerritoryType.AssassinProd:
                // increase the production rate and lower    capacity for it 
                productionRate = 1.2f;
                maxCapacity = 30;
                buffPercentage = 2.5f;
                break;
            case TerritoryType.MageProd:
                //similar prod rate much lower capacity
                productionRate = 2f;
                maxCapacity = 30;
                buffPercentage = 2.5f;
                break;
            case TerritoryType.RangerProd:
                // for now same as the others
                productionRate = 1.7f;
                maxCapacity = 40;
                buffPercentage = 2.5f;
                break;
            case TerritoryType.Fort:
                productionRate = 2.5f;
                maxCapacity = 40;
                buffPercentage = 2.5f;
                break;
            case TerritoryType.Fog:
                break;

        }

        float baseProduction = productionRate * (1f + tProduction * -0.05f);
        int baseCapacity = Mathf.RoundToInt(maxCapacity + tCapacity * 2f);
        float baseBuff = buffPercentage * (1f + tBuff * 0.05f);

        //assign value base on the type that it starts with 
        return new TerretoryData
        {
            TerretoryID = TerretoryID,
            Type = terType,
            Owner = Owner,
            StartingUnits = StartingUnits,
            scale = scale,
            position = position,
            maxCapacity = baseCapacity,
            productionRate = baseProduction,
            buffPercentage = baseBuff,
        };
    }
}
