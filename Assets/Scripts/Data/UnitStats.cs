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


    public UnitStats WithTier(int tierProduction,int tierCapacity,int tierMoveSpeed, UnitType newType)
    {
        float tProduction = tierProduction - 1;
        float tMoveSpeed = tierMoveSpeed - 1;
        float tCapacity = tierCapacity - 1;

        // Start from BASE values, not mutated ones
        float baseMoveSpeed = moveSpeed;
        float baseProduction = productionRate;
        float baseStrength = strenght;
        int baseCapacity = maxCapacity;

        // Apply unit type overrides
        if (newType == UnitType.Heavy)
        {
            baseProduction = 1.3f;
            baseMoveSpeed = 1.5f;
            baseStrength = 2;
        }
        else if (newType == UnitType.Scout)
        {
            baseProduction = 0.7f;
            baseMoveSpeed = 3f;
            baseStrength = 0.5f;
        }

        return new UnitStats
        {
            unitType = newType,
            strenght = baseStrength,
            moveSpeed = baseMoveSpeed * (1f + tMoveSpeed * 0.05f),
            maxCapacity = Mathf.RoundToInt(baseCapacity + tCapacity * 5f),
            productionRate = baseProduction * (1f + tProduction * -0.05f),
        };
    }


}
