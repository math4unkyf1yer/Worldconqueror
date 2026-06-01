using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerritoryType
{
    Production,
    Fort,
    scoutProd,
    HeavyProd,
    Fog
}
public enum FactoryType
{
    Balanced,
    Scout,
    Heavy
}

public enum HazardType
{
    Slow,
    Speed,
    Damage,
    Fog
}

public enum Owner
{
    Neutral,
    Player,
    AI1,
    AI2,
    AI3
}

public enum Difficulty
{
    easy,
    Normal,
    Hard,
    custom
}

public enum UnitType
{
    Basic,
    Scout,
    Heavy
}
