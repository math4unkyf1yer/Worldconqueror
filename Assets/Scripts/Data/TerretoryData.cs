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
    public string effectDescription = "";

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
                radiusEffect = "Health Buff";
                effectDescription = "Health Buff: Within this territory’s radius, troops are infused with Vigor, bolstering their vitality and allowing them to withstand far more damage.";
                break;
            case TerritoryType.DwarfProd:
                //half the production rate and lower capacity for it 
                productionRate = 2.4f;
                maxCapacity = 34;
                radiusSize = 1.1f;
                radiusEffect = "Burning Aura";
                effectDescription = "Burning Aura: Enemy troops inside this territory’s radius are continuously scorched, taking steady damage with a chance to be instantly incinerated.";
                break;
            case TerritoryType.AssassinProd:
                // increase the production rate and lower    capacity for it 
                productionRate = 1.2f;
                maxCapacity = 30;
                radiusSize = 1.1f;
                radiusEffect = "Cripple Radius";
                effectDescription = "Cripple Radius: Enemies caught within the radius suffer severe movement reduction, allowing your territory to dominate the battlefield.";
                break;
            case TerritoryType.MageProd:
                //similar prod rate much lower capacity
                productionRate = 2f;
                maxCapacity = 30;
                radiusSize = 1.1f;
                radiusEffect = "Flame Shot";
                effectDescription = "Flame Shot: Any enemy that steps into the radius awakens the territory’s fiery ward, hurling a blazing projectile that scorches the target.";
                break;
            case TerritoryType.RangerProd:
                // for now same as the others
                productionRate = 1.7f;
                maxCapacity = 40;
                radiusSize = 1.1f;
                radiusEffect = "Sharpshot";
                effectDescription = "Sharpshot: Enemy troops entering this territory’s radius trigger an automatic defense response, firing a precise arrow that strikes the intruder on impact.";
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
            radiusEffect = radiusEffect,
            effectDescription = effectDescription
        };
    }
}
