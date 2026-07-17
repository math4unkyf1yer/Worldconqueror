using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cost : MonoBehaviour
{
    //scripts
    private AssignLevel assignLevelScript;
    private SelectionHighlighter selectionHighlighter;
    private SpriteSwitcher spriteSwitcher;

    [SerializeField] private TextMeshProUGUI[] costtext;
    [SerializeField] private TextMeshProUGUI[] increaseAmountText;
    [SerializeField] private Button[] Buttons;

    [SerializeField] private bool troopUpgrade;


    [SerializeField] private int whichUpgrade;
    UnitType type = UnitType.Soldier;
    TerritoryType territoryType = TerritoryType.SoldierProd;
    int whichType = 0;
    private int currentIndex = 0;



    // Start is called before the first frame update
    void Start()
    {
        assignLevelScript = AssignLevel.Instance;
        selectionHighlighter = GetComponent<SelectionHighlighter>();
        spriteSwitcher = GetComponent<SpriteSwitcher>();

        ChangeText();
        selectionHighlighter.ChangeButtonColor(Buttons[whichType]);
        spriteSwitcher.SetSprite(whichType);
    }

    public void ClickTroopType(int typeInt)
    {

        if (!IsTroopUnlocked(typeInt)) return;

        whichType = typeInt;
        currentIndex = typeInt;
        type = (UnitType)typeInt;

        ChangeText();
        selectionHighlighter.ChangeButtonColor(Buttons[whichType]);
        spriteSwitcher.SetSprite(whichType);
        
    }

    public void ClickTerritoryType(int typeint)
    {
        whichType = typeint;
        currentIndex = typeint;
        territoryType = (TerritoryType)typeint;

        ChangeText();
        selectionHighlighter.ChangeButtonColor(Buttons[whichType]);
        spriteSwitcher.SetSprite(whichType);
    }

    bool isTerritoryUnlocked(int index)
    {
        return true;
    }
    bool IsTroopUnlocked(int index)
    {
        if (index == 1) return assignLevelScript.unlockedAssassin;
        if (index == 2) return assignLevelScript.unlockedDwarfs;
        if (index == 3) return assignLevelScript.unlockedMage;
        if (index == 4) return assignLevelScript.unlockedRanger;
        return true;
    }

    public void ClickUpgrade(int Upgrade)// 1 is Attack Power, 2 is move speed and 3 is Health
    {
        if (troopUpgrade)
        {
            if (assignLevelScript.TryUpgradeTroop(Upgrade, type))
            {
                costtext[Upgrade].text = "Cost: " + assignLevelScript.troopUpgrades[type].cost[Upgrade].ToString();
                Menu.Instance.SetCoinText();
                ChangeText();
            }
            else { Debug.Log("not enough coins"); }
        }
        else
        {
            if (assignLevelScript.TryUpgradeTerritory(Upgrade, territoryType))
            {
                costtext[Upgrade].text = "Cost: " + assignLevelScript.territoryUpgrades[territoryType].cost[Upgrade].ToString();
                Menu.Instance.SetCoinText();
                ChangeText();
            }
            else { Debug.Log("not enough coins"); }
        }
    }

    void ChangeText()
    {
        if (troopUpgrade)
        {
            UnitStats currentTroopStat = assignLevelScript.GetCurrentStats(type);

            for (int i = 0; i < costtext.Length; i++)
            {
                costtext[i].text = "Cost: " + assignLevelScript.troopUpgrades[type].cost[i].ToString();
            }
            increaseAmountText[0].text = currentTroopStat.attackPower + " increase by: 5% ".ToString();
            increaseAmountText[1].text = currentTroopStat.moveSpeed + " increase by: 5% ".ToString();
            increaseAmountText[2].text = currentTroopStat.health + " increase by: 5% ".ToString();
        }
        else
        {
            TerretoryData currentTerStat = assignLevelScript.GetCurrentTerStat(territoryType);

            for (int i = 0; i < costtext.Length; i++)
            {
                costtext[i].text = "Cost: " + assignLevelScript.territoryUpgrades[territoryType].cost[i].ToString();
            }
            increaseAmountText[0].text = currentTerStat.productionRate + "sec decrease time by: 5% ".ToString();
            increaseAmountText[1].text = currentTerStat.maxCapacity + " increase capacity by: 5% ".ToString();
            increaseAmountText[2].text = currentTerStat.buffPercentage + " increase by: 5% ".ToString(); // will need to add buff later
        }
    }
}
