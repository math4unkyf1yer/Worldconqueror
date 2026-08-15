using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuffs : MonoBehaviour
{
    public UnitTroop troop;

    private List<UnitBuffs> troopsInDamage = new List<UnitBuffs>();

    float baseSpeed;
    float baseHealth;

    float globalSpeedMult = 1f;
    float ownerSpeedMult = 1f;

    float multiplier = 1f;

    public void SetUp()
    {
        troop = GetComponent<UnitTroop>();
        baseSpeed = troop.speed;
        baseHealth = troop.health;
    }

    // ---------------- SPEED ----------------

    public void AddGlobalSpeed(float mult)
    {
        globalSpeedMult *= mult;
        UpdateSpeed();
    }

    public void RemoveGlobalSpeed(float mult)
    {
        globalSpeedMult /= mult;
        UpdateSpeed();
    }

    public void AddOwnerSpeed(float mult, Owner own)
    {
        if (troop.ownercl == own) return;

        ownerSpeedMult *= mult;
        UpdateSpeed();
    }

    public void RemoveOwnerSpeed(float mult, Owner own)
    {
        if (troop.ownercl == own) return;

        ownerSpeedMult /= mult;
        UpdateSpeed();
    }

    void UpdateSpeed()
    {
        troop.speed = baseSpeed * globalSpeedMult * ownerSpeedMult;
    }

    // ---------------- DAMAGE ----------------

    public void AddDamage(float damage)
    {
        troop.TakeDamage(damage);
    }

    // ---------------- HEALTH ----------------
    public void AddHealth(float healthChange, Owner own)
    {
        if(troop.ownercl == own)
        {
            multiplier *= healthChange;
            troop.health = baseHealth * multiplier;
        }
    }
    public void ResetHealth(float healthChange, Owner own)
    {
        if(troop.ownercl == own)
        {
            multiplier /= healthChange;
            troop.health = baseHealth * multiplier;
        }
    }

}
