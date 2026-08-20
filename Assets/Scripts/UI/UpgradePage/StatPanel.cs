using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatPanel : MonoBehaviour
{
    [SerializeField] GameObject statObject;
    [SerializeField] GameObject explanationObject;
    [SerializeField] TextMeshProUGUI[] statsAmount;
    [SerializeField] TextMeshProUGUI explanationText;

    public string[] statsExplenation;

    private Cost upgradePageScript;
    private UnitStats currentTroopStat;
    private TerretoryData currentTerrainData;

    private void Start()
    {
        upgradePageScript = GetComponent<Cost>();
    }
    public void ShowStats()
    {
        statObject.SetActive(true);

        if(upgradePageScript != null)
        {
            if (upgradePageScript.troopUpgrade)
            {
                currentTroopStat = upgradePageScript.GetCurrentTroopStats();
                //reveal the stats to the world 
                ChangeTroopStatText();
            }
            else
            {
                currentTerrainData = upgradePageScript.GetCurrentTerritoryData();
                ChangeTerritoryStatsText();
            }
        }

    }

    void ChangeTroopStatText()
    {
        float critPercentage = currentTroopStat.critChances *= 100;
        float noDeathPercentage = currentTroopStat.noDeathChances *= 100;

        statsAmount[0].text = currentTroopStat.moveSpeed.ToString();
        statsAmount[1].text = currentTroopStat.vigor.ToString();
        statsAmount[2].text = currentTroopStat.strenght.ToString();
        statsAmount[3].text = currentTroopStat.attackRange.ToString();
        statsAmount[4].text = currentTroopStat.fireRate + "sec".ToString();
        statsAmount[5].text = critPercentage + "%".ToString();
        statsAmount[6].text = noDeathPercentage + "%".ToString();
    }
    void ChangeTerritoryStatsText()
    {
        statsAmount[0].text = currentTerrainData.productionRate.ToString();
        statsAmount[1].text = currentTerrainData.maxCapacity.ToString();
        statsAmount[2].text = currentTerrainData.radiusSize.ToString();
        statsAmount[3].text = currentTerrainData.radiusEffect.ToString();
    }

    public void HideStats()
    {
        explanationObject.SetActive(false);
        statObject.SetActive(false);
    }

    public void DetailStat(int whichStats)
    {
        explanationObject.SetActive(true);
        for(int i = 0; i < statsExplenation.Length; i++)
        {
            if(whichStats == i)
            {
                explanationText.text = statsExplenation[i].ToString();
                break;
            }
        }
    }
}
