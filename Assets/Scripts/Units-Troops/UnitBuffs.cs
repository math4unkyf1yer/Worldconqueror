using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitBuffs : MonoBehaviour
{
    public UnitTroop troop;
    public List<TerAura> currentAura = new List<TerAura>();

    private List<UnitBuffs> troopsInDamage = new List<UnitBuffs>();

    float baseSpeed;
    float baseHealth;

    float globalSpeedMult = 1f;
    float ownerSpeedMult = 1f;

    float ownerHealthMult = 1f;

    public bool insideAura = false;

    public ParticleSystem particles;
    public ParticleSystem particleBurst;
    private int activeBuffs = 0;

    //Colors effect
    public Color slowStart;
    public Color slowEnd;
    public Color speedStart;
    public Color speedEnd;
    public Color damStart;
    public Color damEnd;
    public Color health;

    //colors burst
    public Color sturdy;
    public Color crit;
    public void SetUp()
    {
        troop = GetComponent<UnitTroop>();
        baseSpeed = troop.speed;
        baseHealth = troop.vigor;
        ownerHealthMult = 1f;
        ownerSpeedMult = 1f;
        globalSpeedMult = 1f;
        particles.gameObject.SetActive(false);
    }

    // ---------------- SPEED ----------------

    void ActiveParticles()
    {
        activeBuffs++;

        if (activeBuffs > 0)
            particles.gameObject.SetActive(true);
    }
    void DeactivateParticles()
    {
        activeBuffs--;

        if (activeBuffs <= 0)
        {
            activeBuffs = 0;
            particles.gameObject.SetActive(false);
        }
    }

    public void CritEffect()
    {
        //for show
        particleBurst.Play();
    }
    public void SturdyEffect()
    {
        //change color for show 
        particleBurst.Play();
    }
    public void AddGlobalSpeed(float mult)
    {
        ActiveParticles();
        var col = particles.colorOverLifetime;
        //assigned vfx 
        if (mult < 1)
        {
            //slow
           col.color = GiveParticleColor(slowStart, slowEnd);
        }
        else
        {
            //speed
            col.color = GiveParticleColor(speedStart, speedEnd);
        }
            //if greater speed buff else other 
        globalSpeedMult *= mult;
        UpdateSpeed();
    }

    public void RemoveGlobalSpeed(float mult)
    {
        DeactivateParticles();
        globalSpeedMult /= mult;
        UpdateSpeed();
    }

    public void AddOwnerSpeed(float mult, Owner own, TerAura ters)
    {
        if (troop.ownercl == own) return;

        currentAura.Add(ters);
        ActiveParticles();
        var col = particles.colorOverLifetime;

        col.color = GiveParticleColor(slowStart, slowEnd);
        ownerSpeedMult *= mult;
        UpdateSpeed();
    }

    public void RemoveOwnerSpeed(float mult, Owner own)
    {
        DeactivateParticles();
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

    public void DamageEffect(bool active)
    {
        if (active)
        {
            ActiveParticles();
            var col = particles.colorOverLifetime;

            col.color = GiveParticleColor(damStart, damEnd);
        }
        else
        {
            DeactivateParticles();
        }
    }

    // ---------------- HEALTH ----------------
    public void AddHealth(float healthChange, Owner own, TerAura ters)
    {
        if(troop.ownercl == own)
        {
            currentAura.Add(ters);
            ActiveParticles();
            var col = particles.colorOverLifetime;

            col.color = GiveParticleColor(health, health);

            ownerHealthMult *= healthChange;
            troop.vigor = baseHealth * ownerHealthMult;
        }
    }
    public void ResetHealth(float healthChange, Owner own)
    {
            float newVigor = 0;
            DeactivateParticles();
             ownerHealthMult /= healthChange;
            if(troop.vigor > baseHealth)
            {
                newVigor = baseHealth;
            }
            else
            {
                newVigor = troop.vigor;
            }
             troop.vigor = newVigor * ownerHealthMult;
        
    }

    public Gradient GiveParticleColor(Color a , Color b)
    {
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
        new GradientColorKey(a, 0f),
        new GradientColorKey(b, 1f)
            },
            new GradientAlphaKey[] {
        new GradientAlphaKey(1f, 0f),
        new GradientAlphaKey(1f, 1f)
            }
        );

        return grad;
    }

}
