using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHolder : MonoBehaviour
{
    private List<TerritoryType> territoryTypes = new List<TerritoryType>();

    //types of buff the troops can received 
    public float attackBuff;
    public float healthBuff;
    public float moveSpeedBuff;

    public static BuffHolder Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnTerritoryOwnerChanged(Owner oldOwner, Owner newOwner, TerritoryType type, float baseBuff)
    {
        baseBuff /= 100f;

        // Only player buffs matter
        if (oldOwner == Owner.Player)
            RemoveTerritoryBuff(type, baseBuff);

        if (newOwner == Owner.Player)
            AddTerritoryBuff(type, baseBuff);
    }
    //for now only for the player 
    public void AddTerritoryBuff(TerritoryType type,float baseBuff)
    {
        bool buffAlreadyActive = false;
        if (territoryTypes.Contains(type))
        {
            Debug.Log("Already have the territory type but in list but no buff upgrade");
            buffAlreadyActive = true;
        }
        territoryTypes.Add(type);
        if(!buffAlreadyActive)
        {
            //buff in here (base on type increase whichever it needs to increase)
            switch (type)
            {
                case TerritoryType.SoldierProd:
                    attackBuff += baseBuff;
                    moveSpeedBuff += baseBuff;
                    break;

                case TerritoryType.DwarfProd:
                    healthBuff += baseBuff;
                    break;

                case TerritoryType.AssassinProd:
                    moveSpeedBuff += baseBuff; // example: assassins buff attack more
                    break;

                case TerritoryType.MageProd:
                    moveSpeedBuff += baseBuff; // example: mages buff speed
                    break;

                case TerritoryType.RangerProd:
                    attackBuff += baseBuff;
                    moveSpeedBuff += baseBuff;
                    break;

                case TerritoryType.Fort:
                    healthBuff += baseBuff; // forts buff health strongly
                    break;

                case TerritoryType.Fog:
                    // maybe no buff?
                    break;
            }
        }
    }
    public void RemoveTerritoryBuff(TerritoryType type, float baseBuff)
    {
        // Count how many territories of this type exist
        int count = 0;
        foreach (var t in territoryTypes)
        {
            if (t == type)
                count++;
        }

        // If none exist, nothing to remove
        if (count == 0)
            return;

        // Remove ONE instance of the type
        territoryTypes.Remove(type);

        // If more still exist, do NOT remove buff
        if (count > 1)
            return;

        // If this was the LAST one → remove buff
        switch (type)
        {
            case TerritoryType.SoldierProd:
                attackBuff -= baseBuff;
                moveSpeedBuff -= baseBuff;
                break;

            case TerritoryType.DwarfProd:
                healthBuff -= baseBuff;
                break;

            case TerritoryType.AssassinProd:
                moveSpeedBuff -= baseBuff;
                break;

            case TerritoryType.MageProd:
                moveSpeedBuff -= baseBuff;
                break;

            case TerritoryType.RangerProd:
                attackBuff -= baseBuff;
                moveSpeedBuff -= baseBuff;
                break;

            case TerritoryType.Fort:
                healthBuff -= baseBuff;
                break;

            case TerritoryType.Fog:
                break;
        }
    }
        
    
}
