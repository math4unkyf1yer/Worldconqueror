using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class TerAura : MonoBehaviour
{
    [SerializeField] CircleCollider2D auraCollider;

    float territoryRadius;
    Owner currentOwner;
    TerritoryType territoryType;

    IAuraEffect auraEffect;

    private LineRenderer lr;

    Coroutine tickRoutine;
    float cooldownTimer = 0f;

    //Materials 
    [SerializeField] Material assassinAuraMat;
    [SerializeField] Material dwarfAuraMat;
    [SerializeField] Material mageAuraMat;
    [SerializeField] Material rangerAuraMat;

    void StartTick(float rp)
    {
        if (tickRoutine == null)
            tickRoutine = StartCoroutine(TickLoop(rp));
    }
    IEnumerator TickLoop(float repeatRate)
    {
        cooldownTimer = 0f; // ready to fire the instant a target exists

        while (true)
        {
            bool hasTargets = (auraEffect is DwarfAura d && !d.IsEmpty) || (auraEffect is MageAura m && !m.IsEmpty) || (auraEffect is RangerAura r && !r.IsEmpty);

            if (cooldownTimer <= 0f)
            {
                if (hasTargets)
                {
                    AuraTick();
                    cooldownTimer = repeatRate; // start cooldown right after firing
                }
                // if no targets, stay "ready" (timer left at 0)
                // so the moment a target shows up it fires instantly
            }
            else
            {
                cooldownTimer -= Time.deltaTime;
            }

            yield return null;
        }
    }

    void AuraTick()
    {
        if (auraEffect is DwarfAura dwarfAura)
        {
            dwarfAura.TickDamage();
        }
        if (auraEffect is MageAura mageAura)
        {
            mageAura.TickShoot(gameObject.transform);
        }
        if (auraEffect is RangerAura rageAura)
        {
            rageAura.TickShoot(gameObject.transform);
        }
    }

    public void SetRadius(float radius, Owner owner, TerritoryType type)
    {
        lr = GetComponent<LineRenderer>();
        territoryRadius = radius;
        currentOwner = owner;
        territoryType = type;
        AssignAura();
        DrawCircle();

        if (auraEffect is DwarfAura dwarf)
            StartTick(dwarf.repeatRate);
        else if (auraEffect is MageAura mage)
            StartTick(mage.repeatRate);
        else if (auraEffect is RangerAura ranger)
            StartTick(ranger.repeatRate);
    }

    public void SetCollider()
    {
        auraCollider.radius = territoryRadius;
    }

    void AssignAura()
    {
        switch (territoryType)
        {
            case TerritoryType.SoldierProd:
                auraEffect = new SoldierAura();
                break;

            case TerritoryType.AssassinProd:
                auraEffect = (new AssassinAura());
                lr.material = assassinAuraMat;
                break;

            case TerritoryType.DwarfProd:
                auraEffect = (new DwarfAura());
                lr.material = dwarfAuraMat;
                break;

            case TerritoryType.MageProd:
                auraEffect = (new MageAura());
                lr.material = mageAuraMat;
                break;

            case TerritoryType.RangerProd:
                auraEffect = new RangerAura();
                lr.material = rangerAuraMat;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        UnitBuffs troop = other.GetComponent<UnitBuffs>();
        if (troop == null || auraEffect == null) return;

        if (troop.insideAura) return;   // ← prevents double buff

        troop.insideAura = true;

        auraEffect.ApplyEffect(troop, currentOwner);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UnitBuffs troop = collision.GetComponent<UnitBuffs>();
        if (troop == null || auraEffect == null) return;

        if (!troop.insideAura) return;

        troop.insideAura = false;
        //reamove the aura effect from the troops
        auraEffect.RemoveEffect(troop, currentOwner);

    }

    void DrawCircle()
    {
        int segments = 10;
        lr.positionCount = segments + 1;
        lr.useWorldSpace = false;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            lr.SetPosition(i, new Vector3(Mathf.Cos(angle) * territoryRadius, Mathf.Sin(angle) * territoryRadius, 0));
        }
    }

    private void OnDrawGizmosSelected()
    {
        
            Gizmos.color = Color.cyan;
            // Draw a wire circle for the radius
            Gizmos.DrawWireSphere(transform.position, territoryRadius);
        

    }



    public class SoldierAura : IAuraEffect
    {
        public float GetValue()
        {
            return 1.5f;
        }
        public void ApplyEffect(UnitBuffs troop, Owner territoryOwner)
        {
            troop.AddHealth(GetValue(), territoryOwner);
        }
        public void RemoveEffect(UnitBuffs troop, Owner territoryOwner)
        {
            troop.ResetHealth(GetValue(), territoryOwner);
        }
    }
    public class AssassinAura : IAuraEffect
    {
        public float GetValue()
        {
            return 0.65f;
        }
        public void ApplyEffect(UnitBuffs troop, Owner territoryOwner)
        {
            troop.AddOwnerSpeed(GetValue(), territoryOwner);
        }
        public void RemoveEffect(UnitBuffs troop, Owner territoryOwner)
        {
            //remove heal example
            troop.RemoveOwnerSpeed(GetValue(), territoryOwner);
        }
    }
    public class DwarfAura : IAuraEffect
    {
        public List<UnitBuffs> troopsInDamage = new List<UnitBuffs>();

        public float repeatRate = 1.0f;
        public bool IsEmpty => troopsInDamage.Count == 0;

        public float GetValue()
        {
            return 1.0f;
        }
        public void ApplyEffect(UnitBuffs troop, Owner territoryOwner)
        {
            if (!troopsInDamage.Contains(troop) && territoryOwner != troop.troop.ownercl)
                troopsInDamage.Add(troop);
        }
        public void RemoveEffect(UnitBuffs troop, Owner territoryOwner)
        {
            troopsInDamage.Remove(troop);
        }
        public void TickDamage()
        {
            if (troopsInDamage.Count == 0) return;

            int index = Random.Range(0, troopsInDamage.Count);
            troopsInDamage[index].AddDamage(1);
        }
    }
    public class MageAura : IAuraEffect
    {
        private List<UnitBuffs> enemies = new List<UnitBuffs>();

        public float repeatRate = 4f;
        public bool IsEmpty => enemies.Count == 0;
        public float GetValue()
        {
            return 0.5f;
        }
        public void ApplyEffect(UnitBuffs troop, Owner territoryOwner)
        {
            if (troop.troop.ownercl != territoryOwner)
            {
                if (!enemies.Contains(troop))
                    enemies.Add(troop);
            }
        }
        public void RemoveEffect(UnitBuffs troop, Owner territoryOwner)
        {
            enemies.Remove(troop);
        }

        public void TickShoot(Transform startPos)
        {
            enemies.RemoveAll(t => t == null || t.troop.vigor <= 0);

            if (enemies.Count > 0)
            {
                int index = Random.Range(0, enemies.Count);
                UnitBuffs target = enemies[index];

                FireBall fireball = FireBallPool.Instance.GetFireBall().GetComponent<FireBall>();
                fireball.gameObject.SetActive(true);

                fireball.transform.position = startPos.position;
                fireball.ScaleUp();
                fireball.SetUp(target.troop.ownercl, target.transform, GetValue(), 1.5f);
            }
        }
    }
    public class RangerAura : IAuraEffect
    {
        private List<UnitBuffs> enemies = new List<UnitBuffs>();

        public float repeatRate = 2f;
        public bool IsEmpty => enemies.Count == 0;
        public float GetValue()
        {
            return 1.0f;
        }
        public void ApplyEffect(UnitBuffs troop, Owner territoryOwner)
        {
            if (troop.troop.ownercl != territoryOwner)
            {
                if (!enemies.Contains(troop))
                    enemies.Add(troop);
            }
        }
        public void RemoveEffect(UnitBuffs troop, Owner territoryOwner)
        {
            enemies.Remove(troop);
        }

        public void TickShoot(Transform startPos)
        {
            enemies.RemoveAll(t => t == null || t.troop.vigor <= 0);

            if (enemies.Count > 0)
            {
                int index = Random.Range(0, enemies.Count);
                UnitBuffs target = enemies[index];

                GameObject Arrow = ArrowPool.Instance.GetFireBall();
                Arrow ArrowRef = Arrow.GetComponent<Arrow>();

                ArrowRef.gameObject.SetActive(true);
                ArrowRef.transform.position = startPos.transform.position;
                ArrowRef.ScaleUp();
                ArrowRef.SetUp(target.troop.ownercl, target.transform, GetValue(), 2f);

            }
        }
    }
    public interface IAuraEffect
    {
        float GetValue();
        void ApplyEffect(UnitBuffs troop, Owner territoryOwner);
        void RemoveEffect(UnitBuffs troop, Owner territoryOwner);
    }
}



