using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct EnemyTierSet
{
    public int capacityTier;
    public int buffTier;
    public int moveSpeedTier;
    public int productionTier;
    public int AttackPowerTier;
    public int healthTier;
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


    public EnemyTierSet GetEnemyTier(int playerProduction,int playerCapacity, int playerBuff,int playerMoveSpeed, int playerAttackPower, int playerHealth)
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
            playerHealth = 0;
            playerAttackPower = 0;
            playerBuff = 0;
        }

        return new EnemyTierSet
        {
            AttackPowerTier = Mathf.Clamp(playerAttackPower + offset, -1, 10),
            moveSpeedTier = Mathf.Clamp(playerMoveSpeed + offset, -1, 10),
            healthTier = Mathf.Clamp(playerHealth + offset, -1, 10),
            productionTier = Mathf.Clamp(playerProduction + offset, -1, 10),
            capacityTier = Mathf.Clamp(playerCapacity + offset, -1, 10),
            buffTier = Mathf.Clamp(playerBuff + offset,-1,10),

        };
    }

}
