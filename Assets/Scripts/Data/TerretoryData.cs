using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class TerretoryData
{
    public int TerretoryID;
    public TerritoryType Type;

    public Owner Owner;

    public int StartingUnits = 0;

    public float scale;

    [Tooltip("Position on the map in world space. Set by MapGenerator.")]
    public Vector2 position;
}
