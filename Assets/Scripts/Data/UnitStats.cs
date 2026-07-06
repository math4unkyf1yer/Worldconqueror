using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats 
{
    [Tooltip("Which unit type these stats apply to.")]
    public UnitType unitType;

    [Tooltip("Movement speed along a path.")]
    public float moveSpeed = 1;

    public float strenght = 1;

    public float attackPower = 1;

    public float health = 1;

    public float attackRange = 1;

    public UnitStats WithTier(int tierMoveSpeed, UnitType newType)
    {
        float tMoveSpeed = tierMoveSpeed - 1;

        // Start from BASE values, not mutated ones
        float baseMoveSpeed = moveSpeed;
        float baseStrength = strenght;

        // Apply unit type overrides
        if (newType == UnitType.Dwarf)
        {
            baseMoveSpeed = 0.65f;
            baseStrength = 2;
            attackPower = 2;
            health = 2f;
        }
        else if (newType == UnitType.Assassin)
        {
            baseMoveSpeed = 1.5f;
            baseStrength = 0.5f;
            health = 0.5f;
            attackPower = 1;
        }
        else if(newType == UnitType.Mage)
        {
            baseMoveSpeed = 0.75f;
            attackPower = 0;
            health = 0.5f;
            attackRange = 1.8f;
        }
        return new UnitStats
        {
            unitType = newType,
            strenght = baseStrength,//change for future(attack power) 
            moveSpeed = baseMoveSpeed * (1f + tMoveSpeed * 0.05f),
            attackRange = attackRange,
            attackPower= attackPower,
            health = health
        };
    }


}
