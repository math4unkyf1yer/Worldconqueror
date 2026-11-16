using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Slow,
    Speed,
    FireStorm,
    Duplicate
}

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData:ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;

    public float radius;
    public int price;
}
