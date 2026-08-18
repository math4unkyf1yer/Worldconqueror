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

    //this will be change to vigor 
    //
    public float vigor = 1;
    public float attackRange = 1;
    public float critChances;        // Assassin
    public float noDeathChances;     // Soldier
    public float fireRate ;          // Ranger
    public float specialFloat;

    public UnitStats WithTier(int tierMoveSpeed,int tierVigor, int tierSpecial , UnitType newType) // need to add attack power and health 
    {
        float tMoveSpeed = tierMoveSpeed - 1;
        float tVigor = tierVigor - 1;
        float tSpecial = tierSpecial - 1;

        // Start from BASE values, not mutated ones
        float baseMoveSpeed = moveSpeed;
        float baseStrength = strenght;
        float baseVigor = vigor;
        float baseRange = attackRange;

        float critChance = 0f;
        float noDeathChance = 0f;
        float baseFireRate = 1.7f;

        switch (newType)
        {
            case UnitType.Soldier:
                baseMoveSpeed = 1f;
                baseStrength = 1f;
                baseVigor = 1f;

                // Soldier special: % chance to not die
                noDeathChance = 0.05f + (tSpecial * 0.02f); // scales with tier
                specialFloat = noDeathChance;
                break;

            case UnitType.Dwarf:
                baseMoveSpeed = 0.65f;
                baseStrength = 2f;
                baseVigor = 2f;

                baseStrength = baseStrength * (1f + tierSpecial * 0.1f);
                specialFloat = baseStrength;
                break;

            case UnitType.Assassin:
                baseMoveSpeed = 1.5f;
                baseStrength = 0.5f;
                baseVigor = 1f;

                // Assassin special: crit chance
                critChance = 0.05f + (tSpecial * 0.02f);
                specialFloat = critChance;
                break;

            case UnitType.Mage:
                baseMoveSpeed = 0.75f;
                baseVigor = 0.3f;

                baseRange = 1.35f;
                // Mage special: attack range increase
                baseRange = baseRange * (1f + tSpecial * 0.05f);
                specialFloat = baseRange;
                break;

            case UnitType.Ranger:
                baseMoveSpeed = 0.7f;
                baseStrength = 1f;
                baseVigor = 1f;

                // Ranger special: fire rate && increase the 5 percent of the dire rate 
                baseFireRate = baseFireRate * (1f - (tSpecial * 0.05f)); ;
                specialFloat = baseFireRate;
                break;
        }

        return new UnitStats
        {
            unitType = newType,
            strenght = baseStrength,//change for future(attack power) 
            moveSpeed = baseMoveSpeed * (1f + tMoveSpeed * 0.1f),
            attackRange = baseRange, //increase attack range 
            vigor = baseVigor * (1f + tVigor * 0.1f), // increase attack power
            fireRate = baseFireRate,
            critChances = critChance,
            noDeathChances = noDeathChance,
            specialFloat = specialFloat,
        };
    }


}
