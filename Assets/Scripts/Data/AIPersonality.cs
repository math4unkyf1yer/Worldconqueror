using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIPersonality 
{
    public float neutralBonus;   // how much it prefers nearby targets
    public float Range;
    public float inRangeButFar;// how cautious it is
    public float playerTargetBonus; // how aggressive toward the player
    public float decisionInterval; // how fast it thinks
    public float minimumToAdvantage;
    public float rivalAiBonus;// how agressive towards ai bonus

    [Header("Defensive AI")]
    public float defensiveTroopThreshold = 5f;   // if owned territory has fewer troops than this, reinforce it
    public float threatProximityRange = 3f;

    [Header("Aggressive AI")]
    public float desperationThreshold = 5f;
}

// Examples:
// Aggressive AI  → low troopWeight, high playerTargetBonus, fast interval
// Defensive AI   → high troopWeight, prefers nearby, slow interval
// Expansionist   → low distanceWeight, grabs neutrals first