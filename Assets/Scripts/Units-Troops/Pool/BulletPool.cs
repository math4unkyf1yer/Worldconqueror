using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> troopsList = new List<GameObject>();
    [SerializeField] private GameObject troops;
    [SerializeField] private int amountOfBullet = 75;

    // need to remoce this is testing
    public GameObject extraTroopHolder;

    public static BulletPool Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        for (int i = 0; i < amountOfBullet; i++)
        {
            GameObject troopClone = Instantiate(troops, this.transform);
            troopClone.SetActive(false);
            troopsList.Add(troopClone);
        }

    }

    public void RemoveTroop(GameObject bullet)
    {
        troopsList.Remove(bullet);
        bullet.SetActive(true);
    }
    public void ReceiveTroop(GameObject bullet)
    {
        bullet.SetActive(false);
        troopsList.Add(bullet);
    }

    public GameObject GetTroop()//get troops from the lest
    {
        if(troopsList.Count > 0)
        {
            GameObject troop = troopsList[0];
            RemoveTroop(troop);
            return troop;
        }
        Debug.LogWarning($"Pool exhausted! Growing... consider increasing pool size.");
        return Instantiate(troops,extraTroopHolder.transform);
    }
}
