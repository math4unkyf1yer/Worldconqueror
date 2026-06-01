using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Level_01", menuName = "ProjectTerritory/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Which level number this is. Used as the map generation seed.")]
    public int levelIndex;

    [Header("Enemies")]
    [Tooltip("How many AI opponents appear in this level.")]
    [Range(1, 3)]
    public int enemyCount;

    [Header("Hazard")]
    public bool hasHazard;

    public List<HazardZone> Zones = new List<HazardZone>();

    [Header("Map Layout")]
    [Tooltip("All territories on this map. Positions are set by MapGenerator at runtime.")]
    public List<TerretoryData> terretories = new List<TerretoryData>();


    [Header("Difficulty")]
    public DifficultyConfiguration DifficultyConfiguration;


    public int coinReward = 10;

    public int Cost = 1;


}
