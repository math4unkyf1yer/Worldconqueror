using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SturdyPreviewUI : BasePreview
{
    [SerializeField] PreviewTerritory t1;
    [SerializeField] PreviewTerritory t2;
    [SerializeField] GameObject troop2;

    public override void Show(TerritoryType type, UnitStats troop, TerretoryData ter)
    {
        gameObject.SetActive(true);

        t1.SetLook(type);
        t2.SetLook(type);

        t1.VigorPreview(t2.transform, troop.moveSpeed, 1, 0.9f, troop2);
        t2.VigorPreview(t1.transform, troop.moveSpeed, 0.9f, 1, null);
    }
}
