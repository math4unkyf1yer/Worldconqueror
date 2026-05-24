using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats 
{
    [Tooltip("Which unit type these stats apply to.")]
    public UnitType unitType;

    [Tooltip("Movement speed along a path.")]
    public float moveSpeed = 3f;

    [Tooltip("How many defending units this unit removes per combat tick.")]
    public float capturePower = 1f;

    [Tooltip("Max units this territory can hold.")]
    public int maxCapacity = 50;

    [Tooltip("Units produced per second.")]
    public float productionRate = 1f;


    public UnitStats WithTier(int tier)
    {
        float t = tier - 1; // tier 1 = no bonus, tier 5 = full bonus
        return new UnitStats
        {
            unitType = unitType,
            moveSpeed = moveSpeed * (1f + t * 0.05f),  // +5% per tier
            capturePower = capturePower * (1f + t * 0.25f),  // +25% per tier
            maxCapacity = Mathf.RoundToInt(maxCapacity + t * 10f), // +10 per tier
            productionRate = productionRate * (1f + t * 0.15f),  // +15% per tier
        };
    }
}
