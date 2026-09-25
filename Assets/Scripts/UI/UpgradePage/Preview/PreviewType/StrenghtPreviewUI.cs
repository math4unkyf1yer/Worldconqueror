using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrenghtPreviewUI : BasePreview
{
    [SerializeField] PreviewTerritory t1;
    [SerializeField] PreviewTerritory t2;

    public override void Show(TerritoryType type, UnitStats troop, TerretoryData ter)
    {
        gameObject.SetActive(true);

        t1.SetLook(type);
        t2.ChangeUnitNumber(0);

        t1.StrenghtPreview(t2.transform, troop.moveSpeed, troop.strenght, t2);
    }
}
