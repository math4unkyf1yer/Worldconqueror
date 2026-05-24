using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HazardZone
{
    public HazardType Type;

    public Vector2 Position;

    public float intensity = 0.5f;

    [Tooltip("Indices of the map edges this zone covers. Edges are stored as index pairs in LevelDataSO.")]
    public List<int> affectedEdgeIndices = new List<int>();

    public float DamagePerSecound()
    {
        if(Type != HazardType.Damage) { return 0; }
        return Mathf.Lerp(0f, 3f, intensity); // 0–3 units/sec
    }

    public float speedChange()
    {
        switch(Type)
        {
            case HazardType.Slow: return Mathf.Lerp(1f, 0.3f, intensity); // slows to 30% at full intensity
            case HazardType.Speed: return Mathf.Lerp(1f, 2f, intensity); // speeds up to 2x
            default: return 1f;
        }
    }

    public bool HidesUnits => Type == HazardType.Fog;

}
