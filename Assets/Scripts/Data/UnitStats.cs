using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats 
{
    [Tooltip("Which unit type these stats apply to.")]
    public UnitType unitType;

    [Tooltip("Movement speed along a path.")]
    public float moveSpeed = 1f;

    public float strenght = 1;

    [Tooltip("Max units this territory can hold.")]
    public int maxCapacity = 50;

    [Tooltip("Units produced per second.")]
    public float productionRate = 1f;


    public UnitStats WithTier(int tier, UnitType newType)
    {
        float t = tier - 1; // tier 1 = no bonus, tier 5 = full bonus

        unitType = newType;

        if (unitType == UnitType.Heavy) { productionRate = 1.3f; moveSpeed = 1.5f; strenght = 2; }
        else if (unitType == UnitType.Scout) { productionRate = 0.7f; moveSpeed = 3f; strenght = 0.5f; }
        return new UnitStats
        {
            unitType = unitType,
            strenght = strenght,
            moveSpeed = moveSpeed * (1f + t * 0.05f),  // +5% per tier
            maxCapacity = Mathf.RoundToInt(maxCapacity + t * 10f), // +10 per tier
            productionRate = productionRate * (1f + t * -0.10f),  // +10% per tier
        };
    }

}
