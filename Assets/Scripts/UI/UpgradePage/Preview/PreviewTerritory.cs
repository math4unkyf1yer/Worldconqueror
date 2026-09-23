using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewTerritory : MonoBehaviour
{
    public Owner terOwner;
    public Transform target;
    public GameObject previewTroop;

    //look territory
    [SerializeField] Sprite[] territorySprites;
    private int troopIndex;
    private Sprite currentTroopSprite;
    [SerializeField] Image territoryImage;
    [SerializeField] Image outerImage;
    [SerializeField] TextMeshProUGUI UnitShowText;
 
    [SerializeField] Color playerColor;
    [SerializeField] Color AIColor;
    [SerializeField] Color neutralColor;
    private Color currentColor;

    public void SetLook(TerritoryType terType)
    {
        switch (terType)
        {
            case TerritoryType.SoldierProd:
                territoryImage.sprite = territorySprites[0];
                troopIndex = 0;
                break;
            case TerritoryType.AssassinProd:
                territoryImage.sprite = territorySprites[1];
                troopIndex = 1;
                break;
            case TerritoryType.DwarfProd:
                territoryImage.sprite = territorySprites[2];
                troopIndex = 2;
                break;
            case TerritoryType.MageProd:
                territoryImage.sprite = territorySprites[3];
                troopIndex = 3;
                break;
            case TerritoryType.RangerProd:
                territoryImage.sprite = territorySprites[4];
                troopIndex = 4;
                break;
        }
        switch (terOwner)
        {
            case Owner.Player:
                outerImage.color = playerColor;
                break;
            case Owner.AI1:
                outerImage.color = AIColor;
                break;
            case Owner.Neutral:
                outerImage.color = neutralColor;
                break;
        }
        currentColor = outerImage.color;
    }
   public void SpeedPreview(Transform target, float speed)
   {
        previewTroop.SetActive(true);
        PreviewTroop previewTroopRef = previewTroop.GetComponent<PreviewTroop>();
        previewTroopRef.ImageController(true);
        previewTroopRef.SpriteLook(troopIndex, currentColor);
        previewTroopRef.SpeedPreview(target,this.transform, speed);
   }

    public void VigorPreview(Transform target, float speed, float vigor, float enemyVigor, GameObject enemyTroop)
    {
        previewTroop.SetActive(true);
        previewTroop.transform.position = this.transform.position;

        PreviewTroop previewTroopRef = previewTroop.GetComponent<PreviewTroop>();
        previewTroopRef.SpriteLook(troopIndex, currentColor);
        previewTroopRef.ImageController(true);

        previewTroopRef.VigorPreview(target, this.transform, speed, vigor,enemyVigor,enemyTroop);
    }


    public void StrenghtPreview(Transform target, float speed, float strenght,PreviewTerritory enemyTerritory)
    {
        previewTroop.SetActive(true);
        previewTroop.transform.position = this.transform.position;

        PreviewTroop previewTroopRef = previewTroop.GetComponent<PreviewTroop>();
        previewTroopRef.SpriteLook(troopIndex, currentColor);
        previewTroopRef.ImageController(true);

        previewTroopRef.StrenghtPreview(target,transform,speed,strenght,enemyTerritory);
    }
    //change the unit 
    public void ChangeUnitNumber(float subtract)
    {
        float currentAmount = 10;
        currentAmount -= subtract;

        UnitShowText.text = currentAmount.ToString();
    }
}
