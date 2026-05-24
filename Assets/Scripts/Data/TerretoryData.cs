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

    public FactoryType factoryType = FactoryType.Balanced;

    [Tooltip("Tier of units this territory produces. Driven by player upgrades and difficulty.")]
    [Range(1, 5)]
    public int unitTier = 1;
    [Tooltip("Position on the map in world space. Set by MapGenerator.")]
    public Vector2 position;
}
