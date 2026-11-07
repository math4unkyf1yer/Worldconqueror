using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfoUI : MonoBehaviour
{
    public TextMeshProUGUI territoriesText;
    public TextMeshProUGUI hazardText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI levelText;

    //horizontal box
    [SerializeField] private Transform territoryHolder;
    [SerializeField] private Transform hazardHolder;

    [SerializeField] private GameObject uiLevelTerritory;
    [SerializeField] private GameObject hazardLevelUi;
    [SerializeField] private List<GameObject> objectList = new List<GameObject>();
    [SerializeField] private Sprite[] levelSprites;

    [SerializeField] private Color[] colorsHolder;
    public void ShowData(LevelData levelData)
    {
        if(objectList.Count > 0)
        {
            foreach(GameObject obj in objectList)
            {
                Destroy(obj);
            }
        }

        Dictionary<TerritoryType, int> typeDictionary = new Dictionary<TerritoryType, int>();

        HashSet<HazardType> typeHazard = new HashSet<HazardType>();

        if (levelData != null)
        {
            foreach (TerretoryData data in levelData.terretories)
            {
                if (typeDictionary.ContainsKey(data.Type))
                {
                    typeDictionary[data.Type]++;
                }
                else
                {
                    typeDictionary.Add(data.Type, 1);
                }
            }
        }
        if (levelData.hasHazard)
        {
            foreach (HazardZone data in levelData.Zones)
            {
                typeHazard.Add(data.Type);
            }
        }

        //showcasing our data 
        StringBuilder builderTerritory = new StringBuilder();
        StringBuilder builderZone = new StringBuilder();

        builderTerritory.AppendLine("Territories:");

        foreach (KeyValuePair<TerritoryType,int> data in typeDictionary)
        {
            GameObject childTerritory = Instantiate(uiLevelTerritory,territoryHolder);
            objectList.Add(childTerritory);

            TextMeshProUGUI text = childTerritory.GetComponentInChildren<TextMeshProUGUI>();
            Image[] images = childTerritory.GetComponentsInChildren<Image>();
            Image image = images[1];
            text.text = data.Value + "X";
            ChangeSprite(data.Key, image);
        }
        if (levelData.hasHazard)
        {

            foreach (HazardType hazard in typeHazard)
            {
                GameObject childHazard = Instantiate(hazardLevelUi, hazardHolder);
                objectList.Add(childHazard);
                Image image = childHazard.GetComponent<Image>();
                Color c = ChangeColor(hazard);

                image.color = c;
            }
        }
        else
        {
            builderZone.AppendLine("Hazard:");
            builderZone.AppendLine("None");
            hazardText.text = builderZone.ToString();
        }

            int showLevel = levelData.levelIndex + 1;

        territoriesText.text = builderTerritory.ToString();
        coinText.text = "Coin: " + levelData.coinReward.ToString();
        levelText.text = "Level: " + showLevel.ToString();
        enemiesText.text =  "Enemies: " + levelData.enemyCount.ToString();
    }

    void ChangeSprite(TerritoryType type, Image image)
    {
        switch (type)
        {
            case TerritoryType.AssassinProd:
                image.sprite = levelSprites[0];
                break;
            case TerritoryType.DwarfProd:
                break;
            case TerritoryType.MageProd:
                break;
            case TerritoryType.RangerProd:
                break;
        }
    }

    Color ChangeColor(HazardType type)
    {
        Color colors = Color.white;
        switch (type)
        {
            case HazardType.Speed:
                colors = colorsHolder[0];
                break;
            case HazardType.Slow:
                colors = colorsHolder[1];
                break;
            case HazardType.Damage:
                colors = colorsHolder[2];
                break;
        }
        return colors;
    }
}
