using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionPreview : BasePreview
{
    [SerializeField] PreviewTerritory territory;

    public override void Show(TerritoryType type, UnitStats troop, TerretoryData ter)
    {
        gameObject.SetActive(true);
        territory.SetLook(type);


        territory.ProductionPreview(ter.productionRate);
    }

}
