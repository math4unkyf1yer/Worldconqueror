using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalTerritoryBar : MonoBehaviour
{
    public GameObject segmentPrefab;
    private int totalTerritories;
    private int playerTerritory;
    private int neutralTerrritory;
    private int ai1Territory;
    private int ai2Territory;
    private int ai3Territory;

    public void InitializedBar(LevelData level)
    {
        totalTerritories = level.terretories.Count;

        neutralTerrritory = totalTerritories;
    }

    public void CalculateTerritoriesOwner(Owner owner, Owner PreviousOwner)
    {
        if(owner == Owner.Player) { playerTerritory += 1; }
        if(owner == Owner.AI1) { ai1Territory += 1; }
        if(owner == Owner.AI2) { ai2Territory += 1; }
        if(owner == Owner.AI3) { ai3Territory += 1; }
        if(owner == Owner.Neutral) {neutralTerrritory += 1; } 
        
        if(PreviousOwner != owner)
        {
            if (PreviousOwner == Owner.Player) { playerTerritory -= 1; }
            if (PreviousOwner == Owner.AI1) { ai1Territory -= 1; }
            if (PreviousOwner == Owner.AI2) { ai2Territory -= 1; }
            if (PreviousOwner == Owner.AI3) { ai3Territory -= 1; }
            if (PreviousOwner == Owner.Neutral) { neutralTerrritory -= 1; }
        }
            
    }
    public void UpdateBar()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        AddSegements(playerTerritory, Color.blue);
        AddSegements(neutralTerrritory, Color.white);
        AddSegements(ai1Territory, Color.red);
        AddSegements(ai2Territory, Color.yellow);
        AddSegements(ai3Territory, Color.green);
    }


    void AddSegements(int count, Color color)
    {
        for (int i = 0; i < count; i++)
        {
            var seg = Instantiate(segmentPrefab, transform);
            seg.GetComponent<Image>().color = color;
        }
    }
}
