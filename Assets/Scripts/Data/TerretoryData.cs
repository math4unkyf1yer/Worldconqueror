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

    [Tooltip("radius range")]
    public float radiusSize = 0;

    [Tooltip("Units produced per second.")]
    public float productionRate = 1f;

    public string radiusEffect = "";

    public TerretoryData TerritoryTier(int tierProduction, int tierCapacity,int tierRadius, TerritoryType terType)
    {
        float tProduction = tierProduction - 1;
        float tCapacity = tierCapacity - 1;
        float tRadius = tierRadius - 1;

        switch (terType)// change production and capacity for the territory for now for the specific type
        {
            case TerritoryType.SoldierProd:
                productionRate = 1.7f;
                maxCapacity = 40;
                radiusSize = 1.1f;
                radiusEffect = "Give health to troops";
                break;
            case TerritoryType.DwarfProd:
                //half the production rate and lower capacity for it 
                productionRate = 2.4f;
                maxCapacity = 34;
                radiusSize = 1.1f;
                radiusEffect = "Damage Enemy troops";
                break;
            case TerritoryType.AssassinProd:
                // increase the production rate and lower    capacity for it 
                productionRate = 1.2f;
                maxCapacity = 30;
                radiusSize = 1.1f;
                radiusEffect = "Slow Enemy Troops";
                break;
            case TerritoryType.MageProd:
                //similar prod rate much lower capacity
                productionRate = 2f;
                maxCapacity = 30;
                radiusSize = 1.1f;
                radiusEffect = "fires projectile to enemies";
                break;
            case TerritoryType.RangerProd:
                // for now same as the others
                productionRate = 1.7f;
                maxCapacity = 40;
                radiusSize = 1.1f;
                radiusEffect = "Shotts arrow to enemies";
                break;
            case TerritoryType.Fort:
                productionRate = 2.5f;
                maxCapacity = 40;
                radiusSize = 1.1f;
                break;
            case TerritoryType.Fog:
                break;

        }

        //need fix increase by 10 percent
        float baseProduction = productionRate * (1f + tProduction * -0.1f);
        int baseCapacity = Mathf.RoundToInt(maxCapacity * (1f + tCapacity * 0.1f));
        float baseSize = radiusSize * (1f + tRadius * 0.1f);

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
            radiusSize = baseSize,
            radiusEffect = radiusEffect
        };
    }
}
