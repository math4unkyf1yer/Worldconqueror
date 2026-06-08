using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using Unity.Mathematics;

public class TerretoryController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("TerretoryStats")]
    [SerializeField] public TerretoryData TerretoryData;
    [SerializeField] private DifficultyConfiguration Difficulty;
    public float amountOfTroops;
    private float StandardProductionRate = 3;
    public int terretoryIndex;
    public Owner owner;
    Owner previousOwner;
   [SerializeField] private int goldRecieved;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private UnitStats troopsStatsPlayer;
    [SerializeField] private UnitStats troopsStatsEnemy;
    [SerializeField] private UnitStats neutralStats;

    [Header("Troops")]
    //Looks
    [SerializeField] SpriteRenderer troopsMiddleSprite;
    [SerializeField] SpriteRenderer terretoryImage;
    private SpriteRenderer middleOuterSprite;
    [SerializeField] GameObject[] troopsUnit;
    private int troopIndex;
    UnitType unitType = UnitType.Basic;

    [Header("UI/Dragging")]
    //Dragging
    bool isDragging;
    [SerializeField] private GameObject arrowPrefab;
    GameObject activeArrow;
    private Camera mainCamera;
    [SerializeField] private Transform circleTransform;
    [SerializeField] private GameObject[] sprites;

    //MapGenerator
    MapGenerator mapGenerator;
    public Dictionary<Owner, AIController> aiControllers;
    [SerializeField]private List<TerritorySlider> sliderList = new List<TerritorySlider>();

    void Awake()
    {
        mainCamera = Camera.main;
    }

    //start of it and it will keep increasing-- getting the data that the territory will need(should have got a territory manager)
    public void GetData(TerretoryData data, DifficultyConfiguration difficulty, MapGenerator gameMode, List<TerritorySlider> sliders)
    {
        transform.localScale = new Vector3(data.scale, data.scale, data.scale);

        TerretoryData = data;
        Difficulty = difficulty;
        terretoryIndex = TerretoryData.TerretoryID;
        owner = TerretoryData.Owner;
        amountOfTroops = TerretoryData.StartingUnits;
        mapGenerator = gameMode;
        sliderList = sliders;

        SetTerretoryLook();
        StandardProductionRate = ProductionRate();
       //Invoke reapeate the function for the production rate 
        InvokeRepeating("OnUnitCountUp", 0, StandardProductionRate);
    }
    void OnUnitCountUp()
    {
        int cap = TerretoryData.Owner == Owner.Player
        ? troopsStatsPlayer.maxCapacity
        : troopsStatsEnemy.maxCapacity;

        if (amountOfTroops >= cap) return;
        //increase the number of troops
        amountOfTroops += 1;
        UpdateTroopDisplay();
    }

    public void TakeDamage(float damage,Owner ownercl)
    {

        //reduce the amounts of trrops
        if (TerretoryData.Type == TerritoryType.Fort)
        {
            float amount = damage / 3;
            amountOfTroops -= amount;
        }
        else if(TerretoryData.Type == TerritoryType.HeavyProd)
        {
            float amount = damage / 2;
            amountOfTroops -= amount;
        }
        else
        {
            amountOfTroops -= damage;
        }

        if (amountOfTroops < 0)
        {
            previousOwner = owner;
            if (previousOwner == Owner.AI1 || previousOwner == Owner.AI2 || previousOwner == Owner.AI3)
            {
                aiControllers[previousOwner].OnTerritoryLost(this, previousOwner);
            }

            owner = ownercl;
            if (owner == Owner.AI1 || owner == Owner.AI2 || owner == Owner.AI3)
            {
                aiControllers[owner].OnTerritoryGain(this, owner);
            }

            //Slider Update
            foreach(TerritorySlider data in sliderList)
            {
                data.SetOwnerTerritories(owner, previousOwner);
            }
            CancelInvoke("OnUnitCountUp");
            StandardProductionRate = ProductionRate();
            InvokeRepeating("OnUnitCountUp", 0, StandardProductionRate);
            mapGenerator.CheckIfGameOver();
        }
        amountText.text = amountOfTroops.ToString();
    }
    public void ReceiveTroops(float received)
    {
        if (TerretoryData.Type == TerritoryType.Fort)
        {
            float amount = received / 3;
            amountOfTroops += amount;
        }
        else if (TerretoryData.Type == TerritoryType.HeavyProd)
        {
            float amount = received / 2;
            amountOfTroops += amount;
        }
        else
        {
            amountOfTroops += received;
        }
        amountText.text = amountOfTroops.ToString();
    }

    void UpdateTroopDisplay()
    {
        amountOfTroops = Mathf.Floor(amountOfTroops);
        amountText.text = amountOfTroops.ToString();
    }

    float ProductionRate()
    {
        ChangeOwnershipColor();
        if (owner== Owner.Player)
        {
            //give its production rate == something different
            troopsStatsPlayer = troopsStatsPlayer.WithTier(AssignLevel.Instance.GetUnitStrenght(0), AssignLevel.Instance.GetUnitStrenght(2), AssignLevel.Instance.GetUnitStrenght(1), unitType);
            StandardProductionRate = troopsStatsPlayer.productionRate;
        }
        else if (owner != Owner.Neutral)
        {
       
            EnemyTierSet enemyTier = Difficulty.GetEnemyTier(AssignLevel.Instance.GetUnitStrenght(0), AssignLevel.Instance.GetUnitStrenght(2), AssignLevel.Instance.GetUnitStrenght(1));
            troopsStatsEnemy = troopsStatsEnemy.WithTier(enemyTier.productionTier, enemyTier.capacityTier, enemyTier.productionTier, unitType);
            StandardProductionRate = troopsStatsEnemy.productionRate;

        }else
        {
            neutralStats = neutralStats.WithTier(-10,-10,-10, unitType);
            StandardProductionRate = 3;
        }

        if (TerretoryData.Type == TerritoryType.Fort)
        {
            float newStandard = StandardProductionRate * 1.6f;
            return newStandard;
        }
        return StandardProductionRate;
    }

    void ChangeOwnershipColor()
    {
        switch (owner)
        {
            case Owner.Player:
                troopsMiddleSprite.color = Color.blue;
                terretoryImage.color = new Color(0f, 0f, 1f, 0.3f);
                middleOuterSprite.color = new Color(0f, 0f, 1f, 0.3f);
                break;
            case Owner.AI1:
                troopsMiddleSprite.color = Color.red;
                terretoryImage.color = new Color(1f, 0f, 0f, 0.3f);
                middleOuterSprite.color = new Color(1f, 0f, 0f, 0.3f);
                break;
            case Owner.AI2:
                troopsMiddleSprite.color = Color.yellow;
                terretoryImage.color = new Color(1f, 1f, 0f, 0.3f);
                middleOuterSprite.color = new Color(1f, 1f, 0f, 0.3f);
                break;
            case Owner.AI3:
                troopsMiddleSprite.color = Color.green;
                terretoryImage.color = new Color(0f, 1f, 0f, 0.3f);
                middleOuterSprite.color = new Color(0f, 1f, 0f, 0.3f);
                break;
        }
    }
    void SetTerretoryLook()
    {
        switch (TerretoryData.Type)
        {
            case TerritoryType.Production:
                sprites[0].SetActive(true);
                troopIndex = 0;
                troopsMiddleSprite = sprites[0].GetComponent<SpriteRenderer>();
                break;

            case TerritoryType.Fort:
                sprites[1].SetActive(true);
                troopIndex = 0;
                troopsMiddleSprite = sprites[1].GetComponent<SpriteRenderer>();
                break;

            case TerritoryType.scoutProd:
                sprites[2].SetActive(true);
                troopIndex = 2;
                troopsMiddleSprite = sprites[2].GetComponent<SpriteRenderer>();
                unitType = UnitType.Scout;
                break;

            case TerritoryType.HeavyProd:
                sprites[3].SetActive(true);
                troopIndex = 1;
                troopsMiddleSprite = sprites[3].GetComponent<SpriteRenderer>();
                unitType = UnitType.Heavy;
                break;
        }
        middleOuterSprite = troopsMiddleSprite.GetComponentInChildren<SpriteRenderer>();
    }

    //on mouse down 
    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject hit = eventData.pointerCurrentRaycast.gameObject;
        
        if(hit != null && hit.tag == "Terretory")
        {
            if(owner == Owner.Player && amountOfTroops > 0)
            {
                //is dragging
                isDragging = true;
                activeArrow = Instantiate(arrowPrefab,transform.position, Quaternion.identity);

                SpriteRenderer arrowRenderer = activeArrow.GetComponentInChildren<SpriteRenderer>();
                if (arrowRenderer != null)
                    arrowRenderer.sortingOrder = 997;
            }
        }
    }

    //dragging the mouse 
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(eventData.position);
        mouseWorld.z = 0;

        Vector3 startPos = transform.position;
        Vector3 direction = mouseWorld - startPos;

        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the root so the arrow points toward the mouse
        activeArrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Keep the base locked at the start
        activeArrow.transform.position = startPos;

        // Stretch the arrow forward only
        activeArrow.transform.localScale = new Vector3(distance, 1f, 1f);
    }

    //on mouse release
    public void OnPointerUp(PointerEventData eventData)
    {
        if(isDragging)
        {
            isDragging = false;
            Destroy(activeArrow);
            GameObject hit = eventData.pointerCurrentRaycast.gameObject;
            if (hit != null && hit.transform != circleTransform && hit.tag == "Terretory")
            {
                TerretoryController terretoryScript = hit.GetComponentInParent< TerretoryController>();
                if( terretoryScript == null) { return; }

                SpawnTroops(hit.transform, terretoryScript.terretoryIndex);
            }
        }
    }
    public void SpawnTroops(Transform targetPosition, int targetIndex)
    {
        float spawnRadius = 0.3f;
        float troopsSave = amountOfTroops;
        //spawn all the troops and remove the amount of troops 
        for (int i = 0; i < troopsSave; i++)
        {
            // Spread evenly in a circle + small random offset
            float angle = (360f / troopsSave) * i * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
            // small random jitter so they don't look too rigid
            offset += UnityEngine.Random.insideUnitCircle * 0.1f;

            Vector3 spawnPos = this.transform.position + new Vector3(offset.x, offset.y, 0);

            GameObject spawnedTroops = Instantiate(troopsUnit[troopIndex], spawnPos, Quaternion.identity);
            UnitTroop unitScript = spawnedTroops.GetComponent<UnitTroop>();

            if (unitScript != null)
            {
                if(owner == Owner.Player) { unitScript.SetUp(troopsStatsPlayer, targetPosition, targetIndex, owner); }
                else { unitScript.SetUp(troopsStatsEnemy, targetPosition, targetIndex, owner); }
            }
        }

        amountOfTroops = 0;
        amountText.text = amountOfTroops.ToString();
    }
}
