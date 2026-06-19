using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AssignLevel : MonoBehaviour
{
    public LevelData[] LevelData;
    public LevelData customData;
    public bool customGame;
    public int levelCount = 0;
    [SerializeField]private int UnitProduction = 1;
    [SerializeField] private int UnitMoveSpeed = 1;
    [SerializeField] private int UnitCapacity = 1;
    private int coin;
    int territroyCount;
    int hazardCount;
    float territoryScale = 1f;

    //troops upgrade
    public int[] cost;

    //Custom game 
    public Vector2[] smallPosition;
    public Vector2[] mediumPosition;
    public Vector2[] largePosition;
    public Vector2[] superPosition;
    private Vector2[] chosenPosition;
    private int[] enemyPosition = new int[3];

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
        cost[0] = 10;
        cost[1] = 10;
        cost[2] = 10;
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
    public int GetUnitStrenght(int upgrade)
    {
        if(upgrade == 0) { return UnitProduction; }
        else if(upgrade == 1) { return UnitMoveSpeed; }
        return UnitCapacity;
    }
    public void SetUnitStrenght(int strenght, int upgrade)
    {
        if (upgrade == 0)
        {
            UnitProduction = strenght;
        }
        else if (upgrade == 1) { UnitMoveSpeed = strenght; }
        else { UnitCapacity = strenght; }
    }

    public bool TryUpgradeTroop(int whichUpgrade)
    {

        if (coin >= cost[whichUpgrade])// this is only production
        {
            coin -= cost[whichUpgrade];
            int pt = GetUnitStrenght(whichUpgrade);
            pt++;
            SetUnitStrenght(pt,whichUpgrade);
            cost[whichUpgrade] = GetUpgradeCost(whichUpgrade);
            return true;
        }

        return false;
    }


    //0 is production. 1 is move speed, 2 is capacity
    public int GetUpgradeCost(int whichUpgrade)
    {
        int baseCost = 10;
        float growth = 1.5f;

        if(whichUpgrade == 0)
        { 
            return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, UnitProduction));
        }
        else if(whichUpgrade == 1) { return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, UnitMoveSpeed)); }
        return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, UnitCapacity));
    }

    public void SetCoin(int cost)
    {
        coin -= cost;
    }


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
}
