using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Level_01", menuName = "ProjectTerritory/Level Data")]
public class LevelData : ScriptableObject
{

    [System.Serializable]
    public class VegetationData
    {
        public Sprite sprite;
        public float radius = 2f;      // Poisson disk min distance for this prefab
        public int maxCount = 100;      // cap for this prefab type
        public float scaleX;
        public float scaleY;

        [System.NonSerialized] public Material runtimeMaterial;
        [System.NonSerialized] public List<Matrix4x4> matrices = new List<Matrix4x4>();
        [System.NonSerialized] public List<Matrix4x4[]> batches = new List<Matrix4x4[]>();
    }


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

    public List<VegetationData> populatedItem;

    public int coinReward = 10;

    public int Cost = 1;


}
