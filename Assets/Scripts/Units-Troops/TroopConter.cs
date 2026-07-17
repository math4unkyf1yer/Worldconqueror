using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopConter : MonoBehaviour
{
    public  int PlayerTroopsAlive = 0;
    public  int EnemyTroopsAlive = 0;

    private MapGenerator mapGeneratorScript;
    public static TroopConter Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        mapGeneratorScript = gameObject.GetComponent<MapGenerator>();
    }
    public  void RegisterTroop(bool isPlayer)
    {
        if (isPlayer) PlayerTroopsAlive++;
        else EnemyTroopsAlive++;
    }

    public  void UnregisterTroop(bool isPlayer)
    {
        if (isPlayer) PlayerTroopsAlive--;
        else EnemyTroopsAlive--;

        if(PlayerTroopsAlive == 0 && EnemyTroopsAlive == 0)
        {
            mapGeneratorScript.CheckIfGameOver();
        }
    }
}
