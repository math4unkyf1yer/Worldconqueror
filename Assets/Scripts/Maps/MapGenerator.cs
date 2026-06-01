using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] LevelData levelData;
    [SerializeField] GameObject terretoryPrefab;
    [SerializeField] GameObject[] AiControllers;

    GameObject aiClone;
    [SerializeField] private GameObject hazardPrefab;
    [SerializeField] private float hazardSpreadRadius = 0.7f;
    List<TerretoryController> spawnedTerretories = new List<TerretoryController>();
    Owner firstOwner;

    Dictionary<Owner, TerretoryController> aiStartTerritories = new Dictionary<Owner, TerretoryController>();
    Dictionary<Owner, AIController> aiControllers = new Dictionary<Owner, AIController>();


    // Start is called before the first frame update
    void Start()
    {
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
            terCloneController.GetData(data,levelData.DifficultyConfiguration, this);
            spawnedTerretories.Add(terCloneController);

            if(data.Owner == Owner.AI1 || data.Owner == Owner.AI2 || data.Owner == Owner.AI3)
            {
                aiStartTerritories[data.Owner] = terCloneController;
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
                case Owner.AI1 :
                    aiClone = Instantiate(AiControllers[0], this.transform);
                    break;
                case Owner.AI2 :
                    aiClone = Instantiate(AiControllers[1], this.transform);
                    break;
                case Owner.AI3 :
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
    }

    void PlaceHazards(List<TerretoryController> territories)
    {
        List<TerretoryController> shuffled = new List<TerretoryController>(territories);

        /*for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        */
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

    bool IsLevelOver()
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

    public void CheckIfGameOver()
    {
        if (IsLevelOver())
        {
            if(firstOwner == Owner.Player)
            {
                Win();
            }
            else
            {
                Lose();
            }
        }
    }

    void Win()
    {
        Debug.Log("Win");
    }
    void Lose()
    {
        Debug.Log("Lose");
    }

}
