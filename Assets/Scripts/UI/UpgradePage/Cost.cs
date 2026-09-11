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

    [SerializeField] private TextMeshProUGUI[] costtext;
    [SerializeField] private TextMeshProUGUI buffTroopText;
    [SerializeField] private TextMeshProUGUI[] currentAmountText;
    [SerializeField] private TextMeshProUGUI[] newAmountText;
    [SerializeField] private TextMeshProUGUI[] topStatText;
    [SerializeField] private Button[] Buttons;

    public bool troopUpgrade;


    [SerializeField] private int whichUpgrade;
    UnitType type = UnitType.Soldier;
    TerritoryType territoryType = TerritoryType.SoldierProd;
    int whichType = 0;
    private int currentIndex = 0;

    UnitStats currentTroopStat;
    TerretoryData currentTerStat;

    // Start is called before the first frame update
    void Start()
    {
        assignLevelScript = AssignLevel.Instance;
        buttonController = ButtonLockController.Instance;
        selectionHighlighter = GetComponent<SelectionHighlighter>();
        spriteSwitcher = GetComponent<SpriteSwitcher>();

        ChangeText();
        selectionHighlighter.ChangeButtonColor(Buttons[whichType]);
    }


    public void ClickTroopType(int typeInt)
    {
        if (!IsTroopUnlocked(typeInt)) return;

        whichType = typeInt;
        currentIndex = typeInt;
        type = (UnitType)typeInt;

        ChangeText();
        selectionHighlighter.ChangeButtonColor(Buttons[whichType]);
        
    }

    public void ClickTerritoryType(int typeint)
    {
        if (!isTerritoryUnlocked(typeint)) return;

        whichType = typeint;
        currentIndex = typeint;
        territoryType = (TerritoryType)typeint;

        ChangeText();
        selectionHighlighter.ChangeButtonColor(Buttons[whichType]);
    }

    bool isTerritoryUnlocked(int index)
    {
        if(index == 1) return buttonController.unlockFortUpgrades;
        if (index == 2) return buttonController.unlockAssassinUpgrades;
        if (index == 3) return buttonController.unlockDwarfUpgrades;
        if (index == 4) return buttonController.unlockMageUpgrades;
        if (index == 5) return buttonController.unlockRangerUpgrades;
        return true;
    }
    bool IsTroopUnlocked(int index)
    {
        if (index == 1) return buttonController.unlockAssassinUpgrades;
        if (index == 2) return buttonController.unlockDwarfUpgrades;
        if (index == 3) return buttonController.unlockMageUpgrades;
        if (index == 4) return buttonController.unlockRangerUpgrades;
        return true;
    }

    public void ClickUpgrade(int Upgrade)// 1 is Attack Power, 2 is move speed and 3 is Health
    {
        if (troopUpgrade)
        {
            if (assignLevelScript.TryUpgradeTroop(Upgrade, type))
            {
                costtext[Upgrade].text = "Cost: " + assignLevelScript.troopUpgrades[type].cost[Upgrade].ToString();
                Menu.Instance.SetText();
                ChangeText();
                assignLevelScript.audioManager.PlayButtonCoinSound();
            }
            else { Debug.Log("not enough coins"); }
        }
        else
        {
            if (assignLevelScript.TryUpgradeTerritory(Upgrade, territoryType))
            {
                costtext[Upgrade].text = "Cost: " + assignLevelScript.territoryUpgrades[territoryType].cost[Upgrade].ToString();
                Menu.Instance.SetText();
                ChangeText();
                assignLevelScript.audioManager.PlayButtonCoinSound();
            }
            else { Debug.Log("not enough coins"); }
        }
    }

    void ChangeText()
    {
        if (troopUpgrade)
        {
            currentTroopStat = assignLevelScript.GetCurrentStats(type);

            for (int i = 0; i < costtext.Length; i++)
            {
                costtext[i].text = "Cost: " + assignLevelScript.troopUpgrades[type].cost[i].ToString();
            }
            UpdateTopStats();
        }
        else
        {
            currentTerStat = assignLevelScript.GetCurrentTerStat(territoryType);

            for (int i = 0; i < costtext.Length; i++)
            {
                costtext[i].text = "Cost: " + assignLevelScript.territoryUpgrades[territoryType].cost[i].ToString();
            }

            UpdateTerritoryStat();
        }
    }

    void UpdateTerritoryStat()
    {
        topStatText[0].text = "Production speed: " + currentTerStat.productionRate.ToString("F2");
        currentAmountText[0].text = currentTerStat.productionRate.ToString("F2");
        topStatText[1].text = "Capacity: " + currentTerStat.maxCapacity.ToString("F1");
        currentAmountText[1].text = currentTerStat.maxCapacity.ToString("F1");
        topStatText[2].text = "Size Radius: " + currentTerStat.radiusSize.ToString("F2");
        currentAmountText[2].text = currentTerStat.radiusSize.ToString("F2");

        TerretoryData nextStats = new TerretoryData().TerritoryTier(assignLevelScript.GetProduction(territoryType) + 1,assignLevelScript.GetCapacity(territoryType) + 1,assignLevelScript.GetRadius(territoryType) + 1,territoryType);

        newAmountText[0].text = nextStats.productionRate.ToString("F2");
        newAmountText[1].text = nextStats.maxCapacity.ToString("F1");
        newAmountText[2].text = nextStats.radiusSize.ToString("F2");

        spriteSwitcher.ChangeInfo(whichType);
    }

    void UpdateTopStats()
    {
        topStatText[0].text = "Move Speed: " + currentTroopStat.moveSpeed.ToString("F2");
        currentAmountText[0].text = currentTroopStat.moveSpeed.ToString("F2");

        topStatText[1].text = "Vigor: " + currentTroopStat.vigor.ToString("F2");
        currentAmountText[1].text = currentTroopStat.vigor.ToString("F2");

        topStatText[2].text = assignLevelScript.troopUpgrades[type].specialBuffTroopName + ": " + currentTroopStat.specialFloat.ToString("F2");
        currentAmountText[2].text = currentTroopStat.specialFloat.ToString("F2");

        buffTroopText.text = assignLevelScript.troopUpgrades[type].specialBuffTroopName + "++".ToString();

        UnitStats nextStats = assignLevelScript.GetCurrentStats(type).WithTier(assignLevelScript.GetMoveSpeed(type) + 1, assignLevelScript.GetAttack(type) + 1, assignLevelScript.GetSpecialBuff(type) + 1, type);

        newAmountText[0].text = nextStats.moveSpeed.ToString("F2");
        newAmountText[1].text = nextStats.vigor.ToString("F2");
        newAmountText[2].text = nextStats.specialFloat.ToString("F2");

        spriteSwitcher.ChangeInfo(whichType);
    }

    public UnitStats GetCurrentTroopStats()
    {
        return currentTroopStat;
    }
    public TerretoryData GetCurrentTerritoryData()
    {
        return currentTerStat;
    }

}
