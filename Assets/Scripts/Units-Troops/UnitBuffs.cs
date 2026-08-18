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

    float ownerHealthMult = 1f;

    public bool insideAura = false;

    public void SetUp()
    {
        troop = GetComponent<UnitTroop>();
        baseSpeed = troop.speed;
        baseHealth = troop.vigor;
        ownerHealthMult = 1f;
        ownerSpeedMult = 1f;
        globalSpeedMult = 1f;

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
        troop.TakeDamage(damage, true);
    }

    // ---------------- HEALTH ----------------
    public void AddHealth(float healthChange, Owner own)
    {
        if(troop.ownercl == own)
        {
            ownerHealthMult *= healthChange;
            troop.vigor = baseHealth * ownerHealthMult;
        }
    }
    public void ResetHealth(float healthChange, Owner own)
    {
        if(troop.ownercl == own)
        {
            ownerHealthMult /= healthChange;
            troop.vigor = baseHealth * ownerHealthMult;
        }
    }

}
