using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomModeConfiguration
{
    public LevelData levelData;

    public int aiCount = 1;

    public DifficultyConfiguration difficultyConfiguration;

    public float hazardIntensity = 0.5f;

    // Custom mode never awards coins — hardcoded false.
    public bool AwardCoins => false;

    // Custom mode never costs energy.
    public bool CostsEnergy => false;
}
