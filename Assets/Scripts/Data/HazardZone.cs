using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HazardZone
{
    public HazardType Type;

    public Vector2 Position;

    public float intensity = 1f;

    [Tooltip("position of the terretory")]
    public int terretory;

    public void Damage(UnitTroop troop)
    {
        troop.TakeDamage(2);
    }

    public float speedChange()
    {
        switch(Type)
        {
            case HazardType.Slow: return Mathf.Lerp(1f, 0.7f, intensity); // slows to 30% at full intensity
            case HazardType.Speed: return Mathf.Lerp(1f, 2f, intensity); // speeds up to 2x
            default: return 1f;
        }
    }

    public bool HidesUnits => Type == HazardType.Fog;

}
