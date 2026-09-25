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

    //radius look 
    //Materials 
    [SerializeField] Material assassinAuraMat;
    [SerializeField] Material dwarfAuraMat;
    [SerializeField] Material mageAuraMat;
    [SerializeField] Material rangerAuraMat;
    private LineRenderer lr;

    public void SetLook(TerritoryType terType)
    {
        lr = GetComponent<LineRenderer>();
        switch (terType)
        {
            case TerritoryType.SoldierProd:
                territoryImage.sprite = territorySprites[0];
                troopIndex = 0;
                break;
            case TerritoryType.AssassinProd:
                territoryImage.sprite = territorySprites[1];
                lr.material = assassinAuraMat;
                troopIndex = 1;
                break;
            case TerritoryType.DwarfProd:
                territoryImage.sprite = territorySprites[2];
                lr.material = dwarfAuraMat;
                troopIndex = 2;
                break;
            case TerritoryType.MageProd:
                territoryImage.sprite = territorySprites[3];
                lr.material = mageAuraMat;
                troopIndex = 3;
                break;
            case TerritoryType.RangerProd:
                territoryImage.sprite = territorySprites[4];
                lr.material = rangerAuraMat;
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
        previewTroopRef.SpeedPreview(target, this.transform, speed);
    }

    public void VigorPreview(Transform target, float speed, float vigor, float enemyVigor, GameObject enemyTroop)
    {
        previewTroop.SetActive(true);
        previewTroop.transform.position = this.transform.position;

        PreviewTroop previewTroopRef = previewTroop.GetComponent<PreviewTroop>();
        previewTroopRef.SpriteLook(troopIndex, currentColor);
        previewTroopRef.ImageController(true);

        previewTroopRef.VigorPreview(target, this.transform, speed, vigor, enemyVigor, enemyTroop);
    }


    public void StrenghtPreview(Transform target, float speed, float strenght, PreviewTerritory enemyTerritory)
    {
        previewTroop.SetActive(true);
        previewTroop.transform.position = this.transform.position;

        PreviewTroop previewTroopRef = previewTroop.GetComponent<PreviewTroop>();
        previewTroopRef.SpriteLook(troopIndex, currentColor);
        previewTroopRef.ImageController(true);

        previewTroopRef.StrenghtPreview(target, transform, speed, strenght, enemyTerritory);
    }
    //change the unit 
    public void ChangeUnitNumber(float subtract)
    {
        float currentAmount = 10;
        currentAmount -= subtract;

        UnitShowText.text = currentAmount.ToString();
    }
    public void CapacityPreview(int maxCapacity)
    {
        UnitShowText.text = maxCapacity.ToString();
    }

    public void ProductionPreview(float productionSpeed)
    {
        StartCoroutine(CountUpProduction(productionSpeed));
    }
    private IEnumerator CountUpProduction(float target)
    {
        int amount = 0;
        UnitShowText.text = amount.ToString();

        // Smooth count-up animation
        while (amount < 30)
        {
            amount++;
            UnitShowText.text = amount.ToString();
            yield return new WaitForSeconds(target); // speed of animation
        }
    }


    public void RadiusPreview(float radius)
    {
        int segments = 10;
        lr.positionCount = segments + 1;
        lr.useWorldSpace = false;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            lr.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
        }
    }
}
