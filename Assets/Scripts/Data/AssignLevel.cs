using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.SceneTemplate;
using UnityEngine;
using static UnityEditor.MaterialProperty;

public class AssignLevel : MonoBehaviour
{
    //Custom game 
    public Vector2[] smallPosition;
    public Vector2[] mediumPosition;
    public Vector2[] largePosition;
    public Vector2[] superPosition;
    private Vector2[] chosenPosition;
    private int[] enemyPosition = new int[3];

    public LevelData[] LevelData;
    public LevelData customData;
    public bool customGame;
    public int levelCount = 0;
    private int coin = 20;
    int territroyCount;
    int hazardCount;

    public UnitStats currentTroopStats;
    public TerretoryData terretoryData;
    //troops upgrade
    public int[] cost;

    //Have you unlock the other troops
    public bool unlockedAssassin { get; private set; } = false;
    public bool unlockedDwarfs { get; private set; } = false;
    public bool unlockedMage { get; private set; } = false;
    public bool unlockedRanger { get; private set; } = false;

    public Dictionary<UnitType, TroopUpgradeStats> troopUpgrades = new Dictionary<UnitType, TroopUpgradeStats>();
    public Dictionary<TerritoryType, TerritoryUpgradeStats> territoryUpgrades = new Dictionary<TerritoryType, TerritoryUpgradeStats>();

    [System.Serializable]
    public class TroopUpgradeStats
    {
        public int Attack = 1;
        public int MoveSpeed = 1;
        public int Health = 1;

        public int[] cost = new int[3];
    }
    [System.Serializable]
    public class TerritoryUpgradeStats
    {
        public int Production = 1;
        public int Capacity = 1;
        //will need whatever buff this is 
        public int buff = 1;

        public int[] cost = new int[3];
    }

    public static AssignLevel Instance { get; private set; }

    private void Awake()
    {
        // 1. If an instance already exists and it isn't this one, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (UnitType type in System.Enum.GetValues(typeof(UnitType)))
        {
            TroopUpgradeStats stats = new TroopUpgradeStats();
            stats.cost[0] = 10;
            stats.cost[1] = 10;
            stats.cost[2] = 10;

            troopUpgrades[type] = stats;
        }
        foreach (TerritoryType type in System.Enum.GetValues(typeof(TerritoryType)))
        {
            TerritoryUpgradeStats stats = new TerritoryUpgradeStats();
            stats.cost[0] = 10;
            stats.cost[1] = 10;
            stats.cost[2] = 10;

            territoryUpgrades[type] = stats;
        }
    }

    public void NewLevel(int amountGain)
    {
        if (!customGame)
        {
            levelCount++;
            coin += amountGain;
        }
    }
    public LevelData WhichLevel()
    {
        if (!customGame)
        {
            return LevelData[levelCount];
        }
        return customData;
    }
    public int GetCoin()
    {
        return coin;
    }

    public UnitStats GetCurrentStats(UnitType troopType)
    {
        return currentTroopStats.WithTier(GetMoveSpeed(troopType), GetAttack(troopType), GetHealth(troopType), troopType);
    }
    public TerretoryData GetCurrentTerStat(TerritoryType territoryType)
    {
        return terretoryData.TerritoryTier(GetProduction(territoryType), GetCapacity(territoryType),GetBuff(territoryType), territoryType);
    }

    public bool TryUpgradeTroop(int whichUpgrade, UnitType troopType)
    {
        TroopUpgradeStats stats = troopUpgrades[troopType];

        if (coin < stats.cost[whichUpgrade])
            return false;

        coin -= stats.cost[whichUpgrade];

        switch (whichUpgrade)
        {
            case 0: stats.Attack++; break;
            case 1: stats.MoveSpeed++; break;
            case 2: stats.Health++; break;
        }

        stats.cost[whichUpgrade] = GetUpgradeCost(stats,null, whichUpgrade);
        return true;
    }
    public bool TryUpgradeTerritory(int whichUpgrade, TerritoryType terType)
    {
        TerritoryUpgradeStats stats = territoryUpgrades[terType];

        if (coin < stats.cost[whichUpgrade])
            return false;

        coin -= stats.cost[whichUpgrade];

        switch (whichUpgrade)
        {
            case 0: stats.Production++; break;
            case 1: stats.Capacity++; break;
            case 2: stats.buff++; break;
        }

        stats.cost[whichUpgrade] = GetUpgradeCost(null, stats, whichUpgrade);
        return true;
    }


    //0 is production. 1 is move speed, 2 is capacity
    public int GetUpgradeCost(TroopUpgradeStats stats,TerritoryUpgradeStats terStats, int upgradeInt)
    {
        int baseCost = 10;
        float growth = 1.5f;

        if(stats != null)
        {
            switch (upgradeInt)
            {
                case 0: return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, stats.Attack));
                case 1: return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, stats.MoveSpeed));
                case 2: return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, stats.Health));
            }
        }
        else
        {
            switch (upgradeInt)
            {
                case 0: return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, terStats.Production));
                case 1: return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, terStats.Capacity));
                case 2: return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, terStats.buff));
            }
        }

         return baseCost;
    }

    public void SetCoin(int cost)
    {
        coin -= cost;
    }

    public int GetAttack(UnitType type) => troopUpgrades[type].Attack;
    public int GetMoveSpeed(UnitType type) => troopUpgrades[type].MoveSpeed;
    public int GetHealth(UnitType type) => troopUpgrades[type].Health;

    public int GetProduction(TerritoryType type) => territoryUpgrades[type].Production;
    public int GetCapacity(TerritoryType type) => territoryUpgrades[type].Capacity;
    public int GetBuff(TerritoryType type) => territoryUpgrades[type].buff;


    public void SetupLevel(int enemyCount, bool hazard, MapSize mapSize, DifficultyConfiguration difficulty)
    {
        customGame = true;

        customData.enemyCount = enemyCount + 1;
        customData.hasHazard = hazard;
        customData.DifficultyConfiguration = difficulty;

        switch (mapSize)
        {
            case MapSize.small:
                territroyCount = 6;
                hazardCount = 1;
                chosenPosition = smallPosition;
                enemyPosition[0] = 5;
                enemyPosition[1] = 2;
                enemyPosition[2] = 3;
                break;
            case MapSize.medium:
                territroyCount = 9;
                hazardCount = 3;
                chosenPosition = mediumPosition;
                enemyPosition[0] = 8;
                enemyPosition[1] = 6;
                enemyPosition[2] = 2;
                break;
            case MapSize.large:
                territroyCount = 12;
                hazardCount = 4;
                chosenPosition = largePosition;
                enemyPosition[0] = 11;
                enemyPosition[1] = 5;
                enemyPosition[2] = 6;
                break;
            case MapSize.super:
                territroyCount = 16;
                hazardCount = 6;
                chosenPosition = superPosition;
                enemyPosition[0] = 15;
                enemyPosition[1] = 3;
                enemyPosition[2] = 12;
                break;
        }

        for(int i = 0; i < territroyCount; i++)
        {
            TerretoryData data = new TerretoryData();

            data.TerretoryID = i;

            if(i == 0) { data.Owner = Owner.Player; }
            if(i == enemyPosition[0]) { data.Owner = Owner.AI1; }
            if (i == enemyPosition[1] && customData.enemyCount >= 2) { data.Owner = Owner.AI2; }
            if (i == enemyPosition[2] && customData.enemyCount == 3) { data.Owner = Owner.AI3; }

            data.scale = 1;

            data.position = chosenPosition[i];

            customData.terretories.Add(data);
        }

        if (customData.hasHazard)
        {
            List<int> usedTerritories = new List<int>();
            for (int i = 0; i < hazardCount; i++)
            {
                HazardZone zone = new HazardZone();

                int index;
                do
                {
                    index = Random.Range(0, territroyCount);
                }
                while (usedTerritories.Contains(index));

                usedTerritories.Add(index);
                zone.terretory = index;
                zone.intensity = 0.5f;
                // Randomize hazard type
                zone.Type = (HazardType)Random.Range(0, System.Enum.GetValues(typeof(HazardType)).Length);

                customData.Zones.Add(zone);
            }
        }

    }


    public void UnlockedAssassin()
    {
        unlockedAssassin = true;
    }
    public void UnlockedDwarf()
    {
        unlockedDwarfs = true;
    }
    public void UnlockedMage()
    {
        unlockedMage = true;
    }
    public void UnlockedRanger()
    {
        unlockedRanger = true;
    }
}
