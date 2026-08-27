using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int coins;
    public int level;
    public int mapCurrentCount;
    public List<float> positionYLevel = new List<float>();

    public bool unlockedAssassin;
    public bool unlockedDwarfs;
    public bool unlockedMage;
    public bool unlockedRanger;
    public bool unlockedFort;

    public List<TroopUpgradeSave> troopUpgrades = new List<TroopUpgradeSave>();
    public List<TerritoryUpgradeSave> territoryUpgrade = new List<TerritoryUpgradeSave>();

    public List<TutorialFlag> tutorialFlag = new List<TutorialFlag>();
}

[System.Serializable]
public class TutorialFlag
{
    public string id;
    public bool completed;
}

[System.Serializable]
public class TroopUpgradeSave
{
    public UnitType type;
    public int vigor;
    public int moveSpeed;
    public int specialBuff;
    public int[] cost;
}

[System.Serializable]
public class TerritoryUpgradeSave
{
    public TerritoryType type;
    public int production;
    public int capacity;
    public int radius;
    public int[] cost;
}