using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static AssignLevel;

public class SaveInGame : MonoBehaviour
{
    private AssignLevel asssignLevelScript;
    private ButtonLockController lockController;
    private LevelUI levelUiScript;
    [SerializeField] private SaveData testSaveData;

    void Start()
    {
        asssignLevelScript = GetComponent<AssignLevel>();
        lockController = GetComponent<ButtonLockController>();
        levelUiScript = LevelUI.Instance;
        LoadGame();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Clear();
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();


        data.coins = asssignLevelScript.GetCoin();
        data.level = asssignLevelScript.levelCount;

        data.mapCurrentCount = levelUiScript.mapCurrentCount;
        data.positionYLevel = levelUiScript.levelPositionY;

        // Save troop upgrades
        foreach (var kvp in asssignLevelScript.troopUpgrades)
        {
            TroopUpgradeSave save = new TroopUpgradeSave();
            save.type = kvp.Key;
            save.vigor = kvp.Value.Vigor;
            save.moveSpeed = kvp.Value.MoveSpeed;
            save.specialBuff = kvp.Value.SpecialBuff;

            data.troopUpgrades.Add(save);
        }

        // Save territory upgrades
        foreach (var kvp in asssignLevelScript.territoryUpgrades)
        {
            TerritoryUpgradeSave save = new TerritoryUpgradeSave();
            save.type = kvp.Key;
            save.production = kvp.Value.Production;
            save.capacity = kvp.Value.Capacity;
            save.radius = kvp.Value.sizeRadius;

            data.territoryUpgrade.Add(save);
        }

        //Save which tutorial is done 
        data.tutorialFlag.Clear();

        foreach (var kvp in asssignLevelScript.tutorialMenu.tutorialCompleted)
        {
            data.tutorialFlag.Add(new TutorialFlag { id = kvp.Key, completed = kvp.Value });
        }

        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();

        if (data == null) { Menu.Instance.SetCoinText(); levelUiScript.RefreshMap(true); return; }

        asssignLevelScript.SetCoin(data.coins);
        asssignLevelScript.levelCount = data.level;

        levelUiScript.mapCurrentCount = data.mapCurrentCount;
        levelUiScript.levelPositionY = data.positionYLevel;

        //wont have need for this in the furure but for now


        // Load troop upgrades
        foreach (var save in data.troopUpgrades)
        {
            TroopUpgradeStats stats = asssignLevelScript.troopUpgrades[save.type];
            stats.Vigor = save.vigor;
            stats.MoveSpeed = save.moveSpeed;
            stats.SpecialBuff = save.specialBuff;

            stats.cost[0] = asssignLevelScript.GetUpgradeCost(stats, null, 0);
            stats.cost[1] = asssignLevelScript.GetUpgradeCost(stats, null, 1);
            stats.cost[2] = asssignLevelScript.GetUpgradeCost(stats, null, 2);
        }

        // Load territory upgrades
        foreach (var save in data.territoryUpgrade)
        {
            TerritoryUpgradeStats stats = asssignLevelScript.territoryUpgrades[save.type];
            stats.Production = save.production;
            stats.Capacity = save.capacity;
            stats.sizeRadius = save.radius;

            stats.cost[0] = asssignLevelScript.GetUpgradeCost(null, stats, 0);
            stats.cost[1] = asssignLevelScript.GetUpgradeCost(null, stats, 1);
            stats.cost[2] = asssignLevelScript.GetUpgradeCost(null, stats, 2);
        }

        if(asssignLevelScript.tutorialMenu.tutorialCompleted != null)
        {
            asssignLevelScript.tutorialMenu.tutorialCompleted.Clear();

            foreach (var flag in data.tutorialFlag)
            {
                asssignLevelScript.tutorialMenu.tutorialCompleted[flag.id] = flag.completed;
            }
        }

        Menu.Instance.SetCoinText();
        Menu.Instance.SetUp();
        levelUiScript.RefreshMap(true);
    }

    public void Clear()
    {
        SaveSystem.ClearSave();
    }
}
