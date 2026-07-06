using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerritoryType
{
    SoldierProd,
    Fort,
    AssassinProd,
    DwarfProd,
    RangerProd,
    MageProd,
    Fog
}
public enum TroopState
{
    Objective,
    Chasing,
    Attacking
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
    Damage
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
    Soldier,
    Assassin,
    Dwarf,
    Ranger,
    Mage
}
public enum MapSize
{
    small,
    medium,
    large,
    super
}
