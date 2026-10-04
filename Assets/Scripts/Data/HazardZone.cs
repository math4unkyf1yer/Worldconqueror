using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HazardZone
{
    public HazardType Type;

    [Tooltip("position of the terretory for the hazard ")]
    public Vector2 Position;

    public float intensity = 1f;


    public void Damage(UnitTroop troop)
    {
        troop.TakeDamage(2,true);
    }

    public float speedChange()
    {
        switch(Type)
        {
            case HazardType.Slow: return Mathf.Lerp(1f, 0.3f, intensity); // slows to 20% at full intensity
            case HazardType.Speed: return Mathf.Lerp(1f, 2f, intensity); // speeds up to 2x
            default: return 1f;
        }
    }

}
