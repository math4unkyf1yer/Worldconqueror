using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MageBehavior : IUnitBehavior
{
    float nextAttackTime;
    float attackDelay = 1;
    public UnitTroop enemyInRange;
    float nextScanTime = 0f;
    float scanDelay = 0.1f;

    float fallbackTargetTime = 0f;
    float fallbackDelay = 0.5f;
    public IEnumerator Move(UnitTroop troop)
    {
        while (troop.isAlive)
        {
            UnitTroop currentEnemy = troop.GetCurrentEnemy();

            switch (troop.State)
            {
                case TroopState.Objective:
                    HandleObjective(troop, currentEnemy);
                    break;

                case TroopState.Chasing:
                    HandleChasing(troop, currentEnemy);
                    break;

                case TroopState.Attacking:
                    HandleAttacking(troop, currentEnemy);
                    break;
            }

            if(!currentEnemy && !enemyInRange)
            {
                troop.MoveTroop();
            }
            yield return null;
        }
    }

    public void Attack(UnitTroop troop)
    {
        //spawn object(will pool later)
        GameObject fireball = FireBallPool.Instance.GetFireBall();
        FireBall fireBallRef = fireball.GetComponent<FireBall>();
        if (fireBallRef != null)
        {
            fireball.transform.position = troop.transform.position;
            fireBallRef.SetUp(troop.ownercl, troop.GetCurrentEnemy().transform);
        }
    }
    void HandleObjective(UnitTroop troop, UnitTroop enemy)
    {
        if (enemy)
        {
            troop.territoryLocation = troop.location;
            troop.State = TroopState.Attacking;
            return;
        }

        if (Time.time >= nextScanTime) // scan enemy every 0.1
        {
            nextScanTime = Time.time + scanDelay;
            //no enemy see if there is any 
            Collider2D[] hits = Physics2D.OverlapCircleAll(troop.transform.position, troop.range, troop.troopLayer);
            if (enemyInRange) { enemyInRange = null; }

            foreach (Collider2D h in hits)
            {
                UnitTroop found = h.GetComponent<UnitTroop>();

                if (found != null && found.ownercl != troop.ownercl)
                {
                    if (found.isTargeted == false)
                    {
                        troop.SetCurrentEnemy(found);
                        found.isTargeted = true;
                        enemyInRange = null;
                        fallbackTargetTime = 0f;
                        break;
                    }
                    else
                    {
                        enemyInRange = found;
                        fallbackTargetTime = Time.time + fallbackDelay;
                    }
                }
            }
            // No free enemy found, but a targeted enemy exists
            if (!troop.GetCurrentEnemy() && enemyInRange != null)
            {
                if (Time.time >= fallbackTargetTime)
                {
                    troop.SetCurrentEnemy(enemyInRange);
                    troop.State = TroopState.Attacking;
                }
            }

        }
    }

    void HandleAttacking(UnitTroop troop, UnitTroop enemy)
    {
        if (!enemy || !enemy.gameObject.activeInHierarchy)
        {
            troop.EnemyGone();
            troop.State = TroopState.Objective;
            return;
        }

        float dist = Vector3.Distance(troop.transform.position, enemy.transform.position);

        if (dist > troop.releasedRadius)
        {
            troop.EnemyGone();
            troop.State = TroopState.Objective;
            return;
        }
        else
        {
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackDelay;
                //start the attack for the mage (pool object send to the current enemy position && give owner)
                Attack(troop);
            }
        }
    }

    void HandleChasing(UnitTroop troop, UnitTroop enemy)
    {

    }
}

