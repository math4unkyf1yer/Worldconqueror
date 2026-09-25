using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class Cost : MonoBehaviour
{
    //scripts
    private AssignLevel assignLevelScript;
    private ButtonLockController buttonController;
    private SelectionHighlighter selectionHighlighter;
    private SpriteSwitcher spriteSwitcher;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI[] costtext;
    [SerializeField] private TextMeshProUGUI buffTroopText;
    [SerializeField] private TextMeshProUGUI[] currentAmountText;
    [SerializeField] private TextMeshProUGUI[] newAmountText;
    [SerializeField] private TextMeshProUGUI[] topStatText;
    [SerializeField] private Button[] Buttons;

    [Header("Stats")]
    //stats panel 
    [SerializeField] TextMeshProUGUI statName;
    [SerializeField] TextMeshProUGUI statLevel;
    [SerializeField] TextMeshProUGUI explanationText;

    public string[] statsExplenation;
    private PreviewManager previewManager;



    [SerializeField] private int whichUpgrade;
    UnitType type = UnitType.Soldier;
    TerritoryType territoryType = TerritoryType.SoldierProd;
    public int whichType = 0;
    public bool troopUpgrade;

    UnitStats currentTroopStat;
    TerretoryData currentTerStat;

    TroopMode troopMode;
    TerritoryMode territoryMode;
    IUpgradeMode mode;

    // Read-only access for the mode classes
    public TextMeshProUGUI[] CostText => costtext;
    public TextMeshProUGUI BuffTroopText => buffTroopText;
    public TextMeshProUGUI[] CurrentAmountText => currentAmountText;
    public TextMeshProUGUI[] NewAmountText => newAmountText;
    public TextMeshProUGUI[] TopStatText => topStatText;
    public TextMeshProUGUI StatName => statName;
    public TextMeshProUGUI StatLevel => statLevel;
    public TextMeshProUGUI ExplanationText => explanationText;
    public string[] StatsExplanation => statsExplenation;
    public SpriteSwitcher Sprites => spriteSwitcher;

    public PreviewManager preview => previewManager;
    public int WhichType => whichType;

    void Start()
    {
        assignLevelScript = AssignLevel.Instance;
        buttonController = ButtonLockController.Instance;
        selectionHighlighter = GetComponent<SelectionHighlighter>();
        spriteSwitcher = GetComponent<SpriteSwitcher>();

        troopMode = new TroopMode(this, assignLevelScript);
        territoryMode = new TerritoryMode(this, assignLevelScript);
        previewManager = StatPanalManager.Instance.Preview;
        SetMode(troopUpgrade);
    }

    // Hook this to your Troops / Territory tab buttons (pass true for troops).
    public void SetMode(bool troops)
    {
        troopUpgrade = troops;
        mode = troops ? (IUpgradeMode)troopMode : territoryMode;

        whichType = 0;
        mode.Select(0);
        mode.RefreshTexts();
        selectionHighlighter.ChangeButtonColor(Buttons[whichType]);
    }

    // Replaces ClickTroopType and ClickTerritoryType
    public void ClickType(int index)
    {
        if (!IsUnlocked(index)) return;

        whichType = index;
        mode.Select(index);
        mode.RefreshTexts();
        selectionHighlighter.ChangeButtonColor(Buttons[whichType]);
    }

    // Replaces IsTroopUnlocked and isTerritoryUnlocked (they were identical)
    bool IsUnlocked(int index)
    {
        switch (index)
        {
            case 1: return buttonController.unlockAssassinUpgrades;
            case 2: return buttonController.unlockDwarfUpgrades;
            case 3: return buttonController.unlockMageUpgrades;
            case 4: return buttonController.unlockRangerUpgrades;
            default: return true;
        }
    }

    public void ClickUpgrade(int slot) // 0 = first upgrade, 1 = second, 2 = third
    {
        if (mode.TryUpgrade(slot))
        {
            Menu.Instance.SetText();
            mode.RefreshTexts();
            assignLevelScript.audioManager.PlayButtonCoinSound();
        }
        else
        {
            Debug.Log("not enough coins");
        }
    }

    public UnitStats GetCurrentTroopStats() => troopMode.Current;
    public TerretoryData GetCurrentTerritoryData() => territoryMode.Current;

    // Stats panel
    public void ShowStats(int whichStats)
    {
        StatPanalManager.Instance.Show();

        preview.SetData(troopMode.Current, territoryMode.Current);
        mode.ShowStat(whichStats);
    }

    public void HideStats()
    {
        StatPanalManager.Instance.Hide();
    }
}





/// <summary>
/// troop modes
/// </summary>

public interface IUpgradeMode
{
    void Select(int index);          // which troop / territory type is selected
    bool TryUpgrade(int slot);       // 0 = first upgrade, 1 = second, 2 = third
    void RefreshTexts();             // cost text, top stats, current/new amounts, sprite
    void ShowStat(int which);        // stat panel name, level, preview, explanation
}

public class TroopMode : IUpgradeMode
{
    readonly Cost ui;
    readonly AssignLevel level;

    UnitType type = UnitType.Soldier;
    TerritoryType previewTerritory = TerritoryType.SoldierProd; // preview calls expect this
    public UnitStats Current { get; private set; }

    public TroopMode(Cost ui, AssignLevel level)
    {
        this.ui = ui;
        this.level = level;
    }

    public void Select(int index)
    {
        type = (UnitType)index;
        previewTerritory = (TerritoryType)index;
    }

    public bool TryUpgrade(int slot) => level.TryUpgradeTroop(slot, type);

    public void RefreshTexts()
    {
        Current = level.GetCurrentStats(type);

        for (int i = 0; i < ui.CostText.Length; i++)
            ui.CostText[i].text = "Cost: " + level.troopUpgrades[type].cost[i];

        var top = ui.TopStatText;
        var cur = ui.CurrentAmountText;
        var next = ui.NewAmountText;
        string buffName = level.troopUpgrades[type].specialBuffTroopName;

        top[0].text = "Move Speed: " + Current.moveSpeed.ToString("F2");
        cur[0].text = Current.moveSpeed.ToString("F2");

        top[1].text = "Vigor: " + Current.vigor.ToString("F2");
        cur[1].text = Current.vigor.ToString("F2");

        top[2].text = buffName + ": " + Current.specialFloat.ToString("F2");
        cur[2].text = Current.specialFloat.ToString("F2");

        ui.BuffTroopText.text = buffName + "++";

        UnitStats nextStats = level.GetCurrentStats(type).WithTier(
            level.GetMoveSpeed(type) + 1,
            level.GetAttack(type) + 1,
            level.GetSpecialBuff(type) + 1,
            type);

        next[0].text = nextStats.moveSpeed.ToString("F2");
        next[1].text = nextStats.vigor.ToString("F2");
        next[2].text = nextStats.specialFloat.ToString("F2");

        ui.Sprites.ChangeInfo(ui.WhichType);
    }

    public void ShowStat(int which)
    {
        //ui button assign here 
        int specialLevel = level.GetSpecialBuff(type) + 1;
        var preview = ui.preview;

        // 0 = speed, 1 = vigor, 2 = the special stat, which depends on the troop type
        if (which == 2) which += ui.WhichType;

        switch (which)
        {
            case 0:
                ui.StatName.text = "Speed";
                ui.StatLevel.text = "Level " + (level.GetMoveSpeed(type) + 1);
                preview.Show(preview.speed,previewTerritory);
                break;
            case 1:
                ui.StatName.text = "Vigor";
                ui.StatLevel.text = "Level " + (level.GetAttack(type) + 1);
                preview.Show(preview.vigor,previewTerritory);
                break;
            case 2:
                ui.StatName.text = "Sturdy";
                ui.StatLevel.text = "Level " + specialLevel;
                preview.Show(preview.sturdy, previewTerritory);
                break;
            case 3:
                ui.StatName.text = "Critical Chance";
                ui.StatLevel.text = "Level " + specialLevel;
                preview.Show(preview.critical, previewTerritory);
                break;
            case 4:
                ui.StatName.text = "Strength";
                ui.StatLevel.text = "Level " + specialLevel;
                preview.Show(preview.strength, previewTerritory);
                break;
            case 5:
                ui.StatName.text = "Attack Range";
                ui.StatLevel.text = "Level " + specialLevel;
                preview.Show(preview.attackRange, previewTerritory);
                break;
            case 6:
                ui.StatName.text = "Fire Rate";
                ui.StatLevel.text = "Level " + specialLevel;
                preview.Show(preview.fireRate, previewTerritory);
                break;
        }

        ui.ExplanationText.text = ui.StatsExplanation[which];
    }
}

public class TerritoryMode : IUpgradeMode
{
    readonly Cost ui;
    readonly AssignLevel level;

    TerritoryType territoryType = TerritoryType.SoldierProd;
    public TerretoryData Current { get; private set; }

    public TerritoryMode(Cost ui, AssignLevel level)
    {
        this.ui = ui;
        this.level = level;
    }

    public void Select(int index) => territoryType = (TerritoryType)index;

    public bool TryUpgrade(int slot) => level.TryUpgradeTerritory(slot, territoryType);

    public void RefreshTexts()
    {
        Current = level.GetCurrentTerStat(territoryType);

        for (int i = 0; i < ui.CostText.Length; i++)
            ui.CostText[i].text = "Cost: " + level.territoryUpgrades[territoryType].cost[i];

        var top = ui.TopStatText;
        var cur = ui.CurrentAmountText;
        var next = ui.NewAmountText;

        top[0].text = "Production speed: " + Current.productionRate.ToString("F2");
        cur[0].text = Current.productionRate.ToString("F2");

        top[1].text = "Capacity: " + Current.maxCapacity.ToString("F1");
        cur[1].text = Current.maxCapacity.ToString("F1");

        top[2].text = "Size Radius: " + Current.radiusSize.ToString("F2");
        cur[2].text = Current.radiusSize.ToString("F2");

        TerretoryData nextStats = new TerretoryData().TerritoryTier(
            level.GetProduction(territoryType) + 1,
            level.GetCapacity(territoryType) + 1,
            level.GetRadius(territoryType) + 1,
            territoryType);

        next[0].text = nextStats.productionRate.ToString("F2");
        next[1].text = nextStats.maxCapacity.ToString("F1");
        next[2].text = nextStats.radiusSize.ToString("F2");

        ui.Sprites.ChangeInfo(ui.WhichType);
    }

    public void ShowStat(int which)
    {
        var preview = ui.preview;
        switch (which)
        {
            case 0:
                ui.StatName.text = "Production";
                ui.StatLevel.text = "Level " + (level.GetProduction(territoryType) + 1);
                ui.preview.Show(preview.production, territoryType);
                ui.ExplanationText.text = ui.StatsExplanation[which];
                break;
            case 1: // TODO: Capacity
                ui.StatName.text = "Capacity";
                ui.StatLevel.text = "Level " + (level.GetCapacity(territoryType) + 1);
                ui.preview.Show(preview.capacity, territoryType);
                ui.ExplanationText.text = ui.StatsExplanation[which]; 
                break;
            case 2: // TODO: Size Radius
                ui.StatName.text = "Size Radius";
                ui.StatLevel.text = "Level " + (level.GetRadius(territoryType) + 1);
                ui.preview.Show(preview.radius, territoryType);
                ui.ExplanationText.text = ui.StatsExplanation[SizeRadiusExplanation()];
                break;
        }

    }

    //each radius have their own specialtys
    int SizeRadiusExplanation()
    {
        int size = 2;
        switch (territoryType)
        {
            case TerritoryType.SoldierProd:
                size = 2;
                break;
            case TerritoryType.AssassinProd:
                size = 3;
                break;
            case TerritoryType.DwarfProd:
                size = 4;
                break;
        }
        return size;
    }
}

