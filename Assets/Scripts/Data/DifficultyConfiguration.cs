using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct EnemyTierSet
{
    public int capacityTier;
    public int moveSpeedTier;
    public int productionTier;
}
[System.Serializable]
public class DifficultyConfiguration
{
    public Difficulty setting = Difficulty.Normal;
    // Tier offsets per difficulty
    private const int EasyOffset = -1;
    private const int NormalOffset = 0;
    private const int HardOffset = 1;

    public int levelDifficulty;


    public EnemyTierSet GetEnemyTier(int playerProduction,int playerCapacity,int playerMoveSpeed)
    {
        int offset = setting == Difficulty.easy ? EasyOffset :
                     setting == Difficulty.Hard ? HardOffset :
                                                   NormalOffset;
        if (setting == Difficulty.custom)
        {
            offset = levelDifficulty;
            playerCapacity = 0;
            playerMoveSpeed = 0;
            playerProduction = 0;
        }

        return new EnemyTierSet
        {
            capacityTier = Mathf.Clamp(playerCapacity + offset, -1, 10),
            moveSpeedTier = Mathf.Clamp(playerMoveSpeed + offset, -1, 10),
            productionTier = Mathf.Clamp(playerProduction + offset, -1, 10)
        };
    }

    //get specific tier for each 
    public int GetEnemyTierCapacity(int playerTier)
    {
        return 0;
    }
    public int GetEnemyTierMoveSpeed(int playerTier)
    {
        return 0;
    }
}
