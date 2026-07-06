using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] LevelData levelData;
    [SerializeField] GameObject terretoryPrefab;
    [SerializeField] Transform projectileParent;
    [SerializeField] GameObject fireBallObject;
    int mageProjectileMax = 30;
    [SerializeField] GameObject[] AiControllers;
    public BulletPool bulletPool;

    GameObject aiClone;
    [SerializeField] private GameObject hazardPrefab;
    [SerializeField] private float hazardSpreadRadius = 0.7f;
    List<TerretoryController> spawnedTerretories = new List<TerretoryController>();
    Owner firstOwner;

    Dictionary<Owner, TerretoryController> aiStartTerritories = new Dictionary<Owner, TerretoryController>();
    Dictionary<Owner, AIController> aiControllers = new Dictionary<Owner, AIController>();

    [Header("UI")]
    [SerializeField] GameObject terBar;
    [SerializeField] Transform barParent;
    private List<TerritorySlider> sliders = new List<TerritorySlider>();

    //Winnning
    [SerializeField] GameObject winPage;
    [SerializeField] GameObject losePage;
    private bool playerWin;


    void Start()
    {
        if(AssignLevel.Instance != null)
        {
            levelData = AssignLevel.Instance.WhichLevel();
            SetUp();
        }
        
    }

    void SetUp()
    {
        //Slider ui look for better eye
        foreach (TerretoryData data in levelData.terretories)
        {
            if (data.Owner != Owner.Neutral)
            {
                GameObject slider = Instantiate(terBar, barParent);
                TerritorySlider sliderScript = slider.GetComponent<TerritorySlider>();
                sliderScript.InitializedBar(levelData);
                sliderScript.SetSliderOwner(data.Owner);
                sliders.Add(sliderScript);
            }
        }
        
        foreach (TerretoryData data in levelData.terretories)
        {
            if (data == null)
            {
                continue;
            }
            Vector3 position = data.position;
            GameObject terClone = Instantiate(terretoryPrefab, this.transform);
            terClone.transform.position = position;
            TerretoryController terCloneController = terClone.GetComponent<TerretoryController>();
            terCloneController.GetData(data, levelData.DifficultyConfiguration, this,sliders);
            spawnedTerretories.Add(terCloneController);

            if (data.Owner == Owner.AI1 || data.Owner == Owner.AI2 || data.Owner == Owner.AI3)
            {
                aiStartTerritories[data.Owner] = terCloneController;
            }
            if(data.Type == TerritoryType.MageProd)
            {
                for(int i = 0; i < mageProjectileMax; i++)
                {
                    GameObject fireBallProjectile = Instantiate(fireBallObject, projectileParent);
                    FireBallPool.Instance.AddFireBall(fireBallProjectile);
                }
            }
        }

        //need to place hazards 
        if (levelData.hasHazard)
        {
            PlaceHazards(spawnedTerretories);
        }


        foreach (var kvp in aiStartTerritories)
        {
            Owner owner = kvp.Key;
            TerretoryController startTerritory = kvp.Value;

            switch (owner)
            {
                case Owner.AI1:
                    aiClone = Instantiate(AiControllers[0], this.transform);
                    break;
                case Owner.AI2:
                    aiClone = Instantiate(AiControllers[1], this.transform);
                    break;
                case Owner.AI3:
                    aiClone = Instantiate(AiControllers[2], this.transform);
                    break;
            }

            AIController aiController = aiClone.GetComponent<AIController>();

            aiController.SetUp(owner);
            aiController.OnTerritoryGain(startTerritory, owner);

            aiControllers[owner] = aiController;
            // Assign AI controller to all territories
            foreach (TerretoryController t in spawnedTerretories)
                t.aiControllers = aiControllers;
        }

        if (AssignLevel.Instance.customGame)
        {
            levelData.terretories.Clear();
            levelData.Zones.Clear();
        }
    }
    void PlaceHazards(List<TerretoryController> territories)
    {
        List<TerretoryController> shuffled = new List<TerretoryController>(territories);

        int hazardCount = Mathf.Min(levelData.Zones.Count, shuffled.Count);

        for(int i = 0; i < hazardCount; i++)
        {
            HazardZone zone = levelData.Zones[i];
            SpawnHazardInTerretories(shuffled[zone.terretory], levelData.Zones[i]);
        }

    }

    void SpawnHazardInTerretories(TerretoryController territory, HazardZone zone)
    {
        Vector2 randomOffset = Random.insideUnitCircle * hazardSpreadRadius;
        Vector3 hazardPosition = territory.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

        GameObject hazardClone = Instantiate(hazardPrefab, hazardPosition, Quaternion.identity, territory.transform);

        HazardController hazardCtrl = hazardClone.GetComponent<HazardController>();

        hazardCtrl.SetUp(zone);
    }

    bool PlayerAsAll()
    {
        if (spawnedTerretories.Count == 0)
            return false;

        firstOwner = spawnedTerretories[0].owner;

        foreach (TerretoryController controller in spawnedTerretories)
        {
            if (controller.owner != firstOwner)
            {
                return false; // Found a different owner → not over
            }
        }

        return true; // All owners match → level over
    }

    bool PlayerIsGone()
    {
        foreach (TerretoryController controller in spawnedTerretories)
        {
            if (controller.owner == Owner.Player)
            {
                return false; // Found a different owner → not over
            }
        }

        return true;
    }

    public void CheckIfGameOver()
    {
        if (PlayerAsAll())
        {
            if(firstOwner == Owner.Player)
            {
                Win();
            }
        }
        if (PlayerIsGone())
        {
            Lose();
        }

    }

    void Win()
    {
        Debug.Log("Win");
        // win page
        winPage.SetActive(true);
        playerWin = true;
    }
    void Lose()
    {
        Debug.Log("Lose");
        //lose page 
        losePage.SetActive(true);
        playerWin = false;
    }

    public void Continue()
    {
        if (playerWin)
        {
            //gives coin

            //change level
            AssignLevel.Instance.NewLevel(levelData.coinReward);
        }

        SceneManager.LoadScene(0);

    }

}
