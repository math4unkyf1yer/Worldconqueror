using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPreviewUI : BasePreview
{
    [SerializeField] PreviewTerritory t1;
    [SerializeField] PreviewTerritory t2;

    public override void Show(TerritoryType type, UnitStats troop, TerretoryData ter)
    {
        gameObject.SetActive(true);

        t1.SetLook(type);
        t1.SpeedPreview(t2.transform, troop.moveSpeed);
    }
}
