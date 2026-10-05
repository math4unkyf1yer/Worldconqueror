using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum AIPersonalityType { Invasive, Defensive, Neutral }

public class AIController : MonoBehaviour
{
    public Owner aiOwner;
    public AIPersonalityType type;
    [SerializeField] private AIPersonality p;

    public List<TerretoryController> ownedTerritories = new List<TerretoryController>();

    private BlockType blockMask;

    //beahvior Time
    public int maxBehavior;
    int behaviorCount;
    float scoreDistance;
    

    private List<TerretoryController> allTerritories;
    Dictionary<TerretoryController, (TerretoryController target, float score)> workingTerritory = new Dictionary<TerretoryController, (TerretoryController, float)>();

    public void SetUp(Owner owner)
    {
        allTerritories = FindObjectsOfType<TerretoryController>().ToList();
        aiOwner = owner;
        StartCoroutine(AIDecisionLoop());
    }

    public void OnTerritoryGain(TerretoryController t, Owner owner)
    {
        if (owner == aiOwner && !ownedTerritories.Contains(t))
            ownedTerritories.Add(t);
    }

    public void OnTerritoryLost(TerretoryController t, Owner owner)
    {
        if (owner == aiOwner)
            ownedTerritories.Remove(t);
    }

    IEnumerator AIDecisionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(p.decisionInterval);
            RunTurn();
        }
    }

    // ---------- Unified scoring ----------
    float ScoreTarget(TerretoryController from, TerretoryController target)
    {
        float score = 0;

        BlockType mask = from.troopsStatsEnemy.blockType;

        if (!from.CanSendTo(target)) return -1f;   

        float dist = PathGrid.Instance.GetPathDistance(from.terretoryIndex, target.terretoryIndex,from.transform.position, target.transform.position, mask);
        scoreDistance = dist;
        if (dist > p.Range) return -1f;
        if (dist > p.inRangeButFar) score -= 1f;

        float time = dist / from.troopsStatsEnemy.moveSpeed;
        float interval = target.terretoryData.productionRate;

        float produceUnit = Mathf.FloorToInt(time / interval);
        float futureTroops = target.amountOfTroops + produceUnit;

        // Our troop power
        float ourTroops = from.terretoryData.Type switch
        {
            TerritoryType.DwarfProd => from.amountOfTroops * 2f,
            TerritoryType.AssassinProd => from.amountOfTroops * 0.5f,
            _ => from.amountOfTroops
        };

        // Enemy troop power
        float enemyTroops = target.terretoryData.Type switch
        {
            TerritoryType.DwarfProd => futureTroops * 2f,
            TerritoryType.AssassinProd => futureTroops * 0.5f,
            _ => futureTroops
        };


        float troopDiff = ourTroops - enemyTroops;

        // Production value
        if (target.terretoryData.Type == TerritoryType.SoldierProd)
        {
            score += 0.5f;
        }
        else
        {
            score += 1f;
        }

         // Owner bonuses
         score += target.owner switch
         {
             Owner.Player => p.playerTargetBonus,
             Owner.Neutral => p.neutralBonus,
             _ => p.rivalAiBonus
         };

        // Minimum advantage
        if (troopDiff <= p.minimumToAdvantage)
            score -= 10f;

        return Mathf.Max(score, 0);
    }

    // ---------- Unified AI behavior ----------
    void RunTurn()
    {
        ownedTerritories.RemoveAll(t => t == null);

        TerretoryController bestFrom = null;
        TerretoryController bestTarget = null;
        float bestScore = -1f;
        float testScoreDist = 0f;

        foreach (var from in ownedTerritories)
        {
            foreach (var target in allTerritories)
            {
                if (target.owner == aiOwner) continue;

                float score = ScoreTarget(from, target);
                if (score > bestScore)
                {
                    bestScore = score;
                    testScoreDist = scoreDistance;
                    bestFrom = from;
                    bestTarget = target;
                }
                else if (Mathf.Approximately(score, bestScore))
                {
                    if (scoreDistance < testScoreDist)
                    {
                        testScoreDist = scoreDistance;
                        bestFrom = from;
                        bestTarget = target;
                    }
                }
            }
        }
        //reset it 
        scoreDistance = 0;

        if (bestTarget != null && bestScore > 0)
        {
            Attack(bestFrom, bestTarget);
            return;
        }

        if (type == AIPersonalityType.Invasive)
            RunInvasiveFallback();
        else if(type == AIPersonalityType.Defensive)
            RunDefensiveFallback();
        else
        {
            //nothing yet 
        }
    }

    void RunInvasiveFallback()
    {

        ownedTerritories.RemoveAll(t => t == null);
        if (ownedTerritories.Count < 2) return;
        if (behaviorCount < maxBehavior)
        {
            behaviorCount++;
            return;
        }

        var enemyTerritories = allTerritories.Where(t => t.owner != aiOwner).ToList();

        TerretoryController bestTarget = null;
        List<TerretoryController> bestSenders = null;

        foreach (var enemy in enemyTerritories)
        {
            // Find all AI territories in range of this enemy
            var inRange = ownedTerritories.Where(o => Vector3.Distance(o.transform.position, enemy.transform.position) <= p.Range).OrderByDescending(o => o.amountOfTroops).ToList();

            // Must have at least 2 in range
            if (inRange.Count < 2)
                continue;

            // Take the 2 strongest in-range territories
            var strongestTwo = inRange.Take(2).ToList();

            // Total troops = only these 2
            float totalTroops = strongestTwo.Sum(t => t.amountOfTroops);

            // Check if enough to beat enemy
            if (totalTroops < p.minimumToAdvantage + enemy.amountOfTroops)
                continue;

            // This enemy is a valid target — pick the weakest valid one
            if (bestTarget == null || enemy.amountOfTroops < bestTarget.amountOfTroops)
            {
                bestTarget = enemy;
                bestSenders = strongestTwo;
            }
        }

        // -----------------------------------------
        // 3. If no valid target found, stop
        // -----------------------------------------
        if (bestTarget == null)
            return;

        // -----------------------------------------
        // 4. Attack using the 2 strongest in-range territories
        // -----------------------------------------
        foreach (var sender in bestSenders)
        {
            sender.StartSpawn(bestTarget.transform, bestTarget.terretoryIndex);
        }
        behaviorCount = 0;
    }

    void RunDefensiveFallback()
    {
        ownedTerritories.RemoveAll(t => t == null);
        if (ownedTerritories.Count < 2) return;
        if (behaviorCount < maxBehavior)
        {
            behaviorCount++;
            return;
        }

        TerretoryController closestEnemy = null;
        float closestEnemyDist = 30;

        //need to find own territory to send to 
        foreach (var enemy in allTerritories.Where(t => t.owner != aiOwner))
        {
            foreach (var own in ownedTerritories)
            {
                float dist = Vector3.Distance(own.transform.position, enemy.transform.position);
                if (dist < closestEnemyDist)
                {
                    closestEnemyDist = dist;
                    closestEnemy = enemy;
                }
            }
        }

        if (closestEnemy == null)
        {
            behaviorCount = 0;
            return;
        }

        TerretoryController threatened = ownedTerritories.OrderBy(t => Vector3.Distance(t.transform.position, closestEnemy.transform.position)).First();

        var strongest = ownedTerritories.Where(t => t != threatened).OrderByDescending(t => t.amountOfTroops).Take(1).ToList();

        foreach (var sender in strongest)
        {
            sender.StartSpawn(threatened.transform, threatened.terretoryIndex);
        }

        behaviorCount = 0;
    }

    void Attack(TerretoryController from, TerretoryController target)
    {
        from.StartSpawn(target.transform, target.terretoryIndex);
    }

}
