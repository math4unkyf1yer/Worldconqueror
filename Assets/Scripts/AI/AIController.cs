using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AIPersonalityType { Aggressive, Defensive, Neutral }

public class AIController : MonoBehaviour
{
    public Owner aiOwner;
    [SerializeField] private AIPersonality aiPersonality;
    [SerializeField] private AIPersonalityType aipersonalityType;

    public List<TerretoryController> ownedTerritories = new List<TerretoryController>();
    

    private List<TerretoryController> allTerritories;

    private int consecutiveNoGoodOption = 0; // tracks desperation for Aggressive AI
    float ourTroops;


    public void SetUp(Owner owner)
    {
        //get all territories
        allTerritories = FindObjectsOfType<TerretoryController>().ToList();
        aiOwner = owner;
        StartCoroutine(AIDecisionLoop());
    }

    public void OnTerritoryGain(TerretoryController t, Owner owner)
    {
        if(owner == aiOwner)
        {
            if (!ownedTerritories.Contains(t))
                ownedTerritories.Add(t);
        }
    }

    public void OnTerritoryLost(TerretoryController t, Owner owner)
    {
        if(owner == aiOwner)
        {
            ownedTerritories.Remove(t);
        }
    }


    IEnumerator AIDecisionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(aiPersonality.decisionInterval);

            //add the defensive ai testing 
            switch (aipersonalityType)
            {
                case AIPersonalityType.Aggressive:
                    RunAggressiveTurn();
                    break;
                case AIPersonalityType.Defensive:
                    RunDefensiveTurn();
                    break;
                case AIPersonalityType.Neutral:
                    RunNeutralTurn();
                    break;
            }
        }
    }
    TerretoryController FindBestTarget(TerretoryController from)
    {
        TerretoryController best = null;
        float bestScore = -1f;

        foreach (TerretoryController target in allTerritories)
        {
            // Don't attack your own territories
            if (target.owner == aiOwner) continue;

            float score = ScoreTarget(from, target);
            if (score > bestScore)
            {
                bestScore = score;
                best = target;
            }
        }

        if(bestScore!= 0) { return best; }
        return null;
    }

    float ScoreTarget(TerretoryController from, TerretoryController target)
    {
        float score = 0;

        float distance = Vector3.Distance(from.transform.position, target.transform.position);
        if(distance > aiPersonality.Range) { return -1f; }

        if(distance > aiPersonality.inRangeButFar) {  score -= 1; }

        float troopDiff;
        
        if (from.TerretoryData.Type == TerritoryType.HeavyProd) { ourTroops = from.amountOfTroops * 2; }
        else if(from.TerretoryData.Type == TerritoryType.scoutProd) { ourTroops = from.amountOfTroops / 2; }
        else { ourTroops = from.amountOfTroops; }

        if (target.TerretoryData.Type == TerritoryType.HeavyProd || target.TerretoryData.Type == TerritoryType.Fort)
        {
            troopDiff = ourTroops - target.amountOfTroops * 1.4f;
        }
        else
        {
            troopDiff = ourTroops - target.amountOfTroops;
        }

        switch (target.TerretoryData.Type)
        {
            case TerritoryType.Fort: score += 1f; break;
            case TerritoryType.HeavyProd: score += 1f; break;
            case TerritoryType.scoutProd: score += 1f; break;
        }

        if (target.owner == Owner.Player)
        {
            score += aiPersonality.playerTargetBonus;
        }
        else if(target.owner == Owner.Neutral)
        {
            score += aiPersonality.neutralBonus;
        }else if(target.owner == aiOwner && aipersonalityType == AIPersonalityType.Neutral)
        {
            score += 1;
        }
        else
        {
            score += aiPersonality.rivalAiBonus;
        }

        if (troopDiff < aiPersonality.minimumToAdvantage) score -= 10;

        
        if(score < 0) score = 0;

        return score;
    }


    //Defensive Ai
    public void RunDefensiveTurn()
    {
        ReinforceWeakTerritories();

        foreach (TerretoryController owned in ownedTerritories.ToList())
        {
            TerretoryController bestTarget = FindBestTarget(owned);
            if (bestTarget != null)
                owned.SpawnTroops(bestTarget.transform, bestTarget.terretoryIndex);
        }
    }
    void ReinforceWeakTerritories()
    {
        foreach (TerretoryController weakOwned in ownedTerritories.ToList())
        {
            if (weakOwned.amountOfTroops >= aiPersonality.defensiveTroopThreshold) continue;

            bool isThreatened = allTerritories.Any(t => t.owner != aiOwner && t.owner != Owner.Neutral && Vector3.Distance(weakOwned.transform.position, t.transform.position) <= aiPersonality.threatProximityRange);

            if (!isThreatened) continue;

            // Find the strongest nearby friendly territory to send reinforcements from
            TerretoryController reinforcer = ownedTerritories.Where(t => t != weakOwned && t.amountOfTroops > aiPersonality.defensiveTroopThreshold * 2).OrderBy(t => Vector3.Distance(t.transform.position, weakOwned.transform.position)).FirstOrDefault();

            if (reinforcer != null)
            {
                reinforcer.SpawnTroops(weakOwned.transform, weakOwned.terretoryIndex);
            }
        }
    }

    //aggresive ai 
    void RunAggressiveTurn()
    {
        bool foundAnyGoodOption = false;

        foreach (TerretoryController owned in ownedTerritories.ToList())
        {
            TerretoryController bestTarget = FindBestTarget(owned);
            if (bestTarget != null)
            {
                owned.SpawnTroops(bestTarget.transform, bestTarget.terretoryIndex);
                foundAnyGoodOption = true;
            }
        }

        if (!foundAnyGoodOption)
        {
            consecutiveNoGoodOption++;

            if (consecutiveNoGoodOption >= aiPersonality.desperationThreshold)
            {
                consecutiveNoGoodOption = 0;
                ReinforcedOne();
            }
        }
        else
        {
            consecutiveNoGoodOption = 0;
        }
    }

    void ReinforcedOne()
    {
        TerretoryController strongest = ownedTerritories.OrderByDescending(t => t.amountOfTroops).FirstOrDefault();
        if (strongest == null) return;
        TerretoryController target = ownedTerritories.Where(t => t != strongest && t.amountOfTroops > 4).OrderBy(t => Vector3.Distance(strongest.transform.position, t.transform.position)).FirstOrDefault();

        if(target != null)
        {
            target.SpawnTroops(strongest.transform, strongest.terretoryIndex);
        }
    }


    //neutral 
    void RunNeutralTurn()
    {
        foreach (TerretoryController owned in ownedTerritories.ToList())
        {
            TerretoryController bestTarget = FindBestTarget(owned);
            if (bestTarget != null)
            {
                owned.SpawnTroops(bestTarget.transform, bestTarget.terretoryIndex);
            }
        }
    }
}
