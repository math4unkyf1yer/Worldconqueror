using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;

public class RadiusPreview : BasePreview
{
    [SerializeField] PreviewTerritory territory;

    public override void Show(TerritoryType type, UnitStats troop, TerretoryData ter)
    {
        //change explanation
        gameObject.SetActive(true);
        territory.SetLook(type);
        territory.RadiusPreview(ter.radiusSize);
    }

    
}
