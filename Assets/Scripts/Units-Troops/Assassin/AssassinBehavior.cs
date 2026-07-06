using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinBehavior : IUnitBehavior
{
    float nextScanTime = 0f;
    float scanDelay = 0.1f;

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
            troop.MoveTroop();
            yield return null;
        }
    }
    public void Attack(UnitTroop troop)
    {

    }

    void HandleChasing(UnitTroop troop, UnitTroop enemy)
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
        troop.location = enemy.transform;
    }
    void HandleObjective(UnitTroop troop, UnitTroop enemy)
    {
        if (enemy)
        {
            troop.territoryLocation = troop.location;
            troop.State = TroopState.Chasing;
            return;
        }

        if (Time.time >= nextScanTime) // scan enemy every 0.1
        {
            nextScanTime = Time.time + scanDelay;
            //no enemy see if there is any 
            Collider2D[] hits = Physics2D.OverlapCircleAll(troop.transform.position, troop.range, troop.troopLayer);

            foreach (Collider2D h in hits)
            {
                UnitTroop found = h.GetComponent<UnitTroop>();

                if (found != null && found.ownercl != troop.ownercl && found.isTargeted == false)
                {
                    troop.SetCurrentEnemy(found);
                    found.isTargeted = true;   // mark enemy as targeted
                    break;
                }
            }
        }
    }

    void HandleAttacking(UnitTroop troop, UnitTroop enemy)
    {

    }
}
