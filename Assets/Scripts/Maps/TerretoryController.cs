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
    [SerializeField] public TerretoryData terretoryData;
    [SerializeField] private DifficultyConfiguration Difficulty;
    public float amountOfTroops;
    private float StandardProductionRate = 3;
    public int terretoryIndex;
    public Owner owner;
    private Owner CombatOwner;
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
    UnitType unitType = UnitType.Soldier;
    [SerializeField] GameObject mageProjectile;

    AssignLevel troopTiersScript;
    BuffHolder buffScript;

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
    TroopConter countingTroopScript;
    public Dictionary<Owner, AIController> aiControllers;
    [SerializeField]private List<TerritorySlider> sliderList = new List<TerritorySlider>();

    TerBuildRoad buildRoad;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    //start of it and it will keep increasing-- getting the data that the territory will need(should have got a territory manager) 
    //add the roads here do a chek to see if any territory are open 
    public void GetData(TerretoryData data, DifficultyConfiguration difficulty, MapGenerator gameMode, List<TerritorySlider> sliders)
    {
        troopTiersScript = AssignLevel.Instance;
        buffScript = BuffHolder.Instance;
        buildRoad = GetComponent<TerBuildRoad>();
        buildRoad.SetUp();
        transform.localScale = new Vector3(data.scale, data.scale, data.scale);

        terretoryData = data;
        Difficulty = difficulty;
        terretoryIndex = terretoryData.TerretoryID;
        owner = terretoryData.Owner;
        amountOfTroops = terretoryData.StartingUnits;
        mapGenerator = gameMode;
        GameObject map = gameMode.gameObject;
        countingTroopScript = map.GetComponent<TroopConter>();
        sliderList = sliders;

        buffScript.OnTerritoryOwnerChanged(Owner.Neutral, owner, terretoryData.Type, terretoryData.buffPercentage);
        SetTerretoryLook();
        StandardProductionRate = ProductionRate();
       //Invoke reapeate the function for the production rate 
        InvokeRepeating("OnUnitCountUp", 0, StandardProductionRate);
    }
    void OnUnitCountUp()
    {
        int cap = terretoryData.maxCapacity;

        if (amountOfTroops >= cap) return;
        //increase the number of troops
        amountOfTroops += 1;
        UpdateTroopDisplay();
    }

    public void TakeDamage(float damage,Owner ownercl)
    {
        amountOfTroops -= CalculateTroops(damage);

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
            buffScript.OnTerritoryOwnerChanged(previousOwner, owner, terretoryData.Type, terretoryData.buffPercentage);
            CancelInvoke("OnUnitCountUp");
            StandardProductionRate = ProductionRate();
            InvokeRepeating("OnUnitCountUp", 0, StandardProductionRate);
            mapGenerator.CheckIfGameOver();
        }
        amountText.text = amountOfTroops.ToString();
    }
    public void ReceiveTroops(float received)
    {
        amountOfTroops += CalculateTroops(received);
        amountText.text = amountOfTroops.ToString();
    }

    float CalculateTroops(float amountGiven)
    {
        float amount = amountGiven;
        //reduce the amounts of trrops
        if (terretoryData.Type == TerritoryType.Fort)
        {
             amount = amountGiven / 3;
        }
        else if (terretoryData.Type == TerritoryType.DwarfProd)
        {
             amount = amountGiven / 2;
        }
        else if (terretoryData.Type == TerritoryType.AssassinProd)
        {
             amount = amountGiven * 2;
        }
        return amount;
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
            //give its production rate == something different and its buff
            troopsStatsPlayer = troopsStatsPlayer.WithTier(troopTiersScript.GetMoveSpeed(unitType), troopTiersScript.GetAttack(unitType), troopTiersScript.GetHealth(unitType), unitType);
            terretoryData = terretoryData.TerritoryTier(troopTiersScript.GetProduction(terretoryData.Type), troopTiersScript.GetCapacity(terretoryData.Type), troopTiersScript.GetBuff(terretoryData.Type), terretoryData.Type);
            StandardProductionRate = terretoryData.productionRate;
        }
        else if (owner != Owner.Neutral)
        {
       
            EnemyTierSet enemyTier = Difficulty.GetEnemyTier(troopTiersScript.GetProduction(terretoryData.Type), troopTiersScript.GetCapacity(terretoryData.Type), troopTiersScript.GetBuff(terretoryData.Type), troopTiersScript.GetMoveSpeed(unitType),troopTiersScript.GetAttack(unitType), troopTiersScript.GetHealth(unitType));
            troopsStatsEnemy = troopsStatsEnemy.WithTier( enemyTier.moveSpeedTier, enemyTier.AttackPowerTier, enemyTier.healthTier, unitType);
            terretoryData = terretoryData.TerritoryTier(enemyTier.productionTier, enemyTier.capacityTier,enemyTier.buffTier, terretoryData.Type);
            StandardProductionRate = terretoryData.productionRate;
        }
        else
        {
            //needs a few fix take production rate ofneutral and different for each territory
            neutralStats = neutralStats.WithTier(-10,-10,-10, unitType);
            terretoryData = terretoryData.TerritoryTier(-10, -8,-8, terretoryData.Type);
            StandardProductionRate = terretoryData.productionRate;
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
        switch (terretoryData.Type)
        {
            case TerritoryType.SoldierProd:
                sprites[0].SetActive(true);
                troopsMiddleSprite = sprites[0].GetComponent<SpriteRenderer>();
                break;

            case TerritoryType.Fort:
                sprites[1].SetActive(true);
                troopsMiddleSprite = sprites[1].GetComponent<SpriteRenderer>();
                break;

            case TerritoryType.AssassinProd:
                sprites[2].SetActive(true);
                troopsMiddleSprite = sprites[2].GetComponent<SpriteRenderer>();
                unitType = UnitType.Assassin;
                troopTiersScript.UnlockedAssassin();
                break;

            case TerritoryType.DwarfProd:
                sprites[3].SetActive(true);
                troopsMiddleSprite = sprites[3].GetComponent<SpriteRenderer>();
                unitType = UnitType.Dwarf;
                troopTiersScript.UnlockedDwarf();
                break;
            case TerritoryType.MageProd:
                sprites[4].SetActive(true);
                troopsMiddleSprite = sprites[4].GetComponent<SpriteRenderer>();
                unitType = UnitType.Mage;
                troopTiersScript.UnlockedMage();
                break;
            case TerritoryType.RangerProd:
                sprites[5].SetActive(true);
                troopsMiddleSprite = sprites[5].GetComponent<SpriteRenderer>();
                unitType = UnitType.Ranger;
                troopTiersScript.UnlockedRanger();
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

                StartSpawn(hit.transform, terretoryScript.terretoryIndex);
            }
        }
    }

    public void StartSpawn(Transform targetPosition, int targetIndex)
    {
        CombatOwner = owner;
        StartCoroutine(SpawnTroops(targetPosition, targetIndex));
    }
     IEnumerator SpawnTroops(Transform targetPosition, int targetIndex)
    {
        float troopsSave = amountOfTroops;
        amountOfTroops = 0;
        amountText.text = amountOfTroops.ToString();
        int spawned = 0;
        while (spawned < troopsSave)
        {
            float canSpawnNb = Mathf.Min(5,troopsSave-spawned);
            Vector2 dir = ((Vector2)targetPosition.position - (Vector2)transform.position).normalized;
            Vector2 perpendicular = new Vector2(-dir.y, dir.x);
            //spawn all the troops and remove the amount of troops 
            for (int i = 0; i < canSpawnNb; i++)
            {
                float offsetX = (i - (canSpawnNb - 1) / 2f) * 0.3f;
                Vector3 spawnPos = this.transform.position + (Vector3)(perpendicular * offsetX);

                GameObject troopClone = BulletPool.Instance.GetTroop();
                UnitTroop unitScript = troopClone.GetComponent<UnitTroop>();
                troopClone.transform.position = spawnPos;

                if (unitScript != null)
                {
                    if (CombatOwner == Owner.Player) 
                    { 
                        countingTroopScript.RegisterTroop(true);
                        unitScript.SetUp(troopsStatsPlayer, targetPosition, targetIndex, CombatOwner);                    
                    }
                    else 
                    {
                        countingTroopScript.RegisterTroop(false);
                        unitScript.SetUp(troopsStatsEnemy, targetPosition, targetIndex, CombatOwner);
                    }
                }
                spawned++;
            }
            yield return new WaitForSeconds(0.3f);
        }     
    }
}
