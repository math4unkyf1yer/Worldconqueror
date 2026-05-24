using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DifficultyConfiguration
{
    public Difficulty setting = Difficulty.Normal;
    // Tier offsets per difficulty
    private const int EasyOffset = -1;
    private const int NormalOffset = 0;
    private const int HardOffset = 1;

    public int levelDifficulty;


    public int GetEnemyTier(int playerTier)
    {
        int offset = setting == Difficulty.easy ? EasyOffset :
                     setting == Difficulty.Hard ? HardOffset :
                                                   NormalOffset;



        // Clamp between 1 and 5 so enemies are never below minimum or above maximum
        return Mathf.Clamp(playerTier + offset, 1, 5);
    }
}
