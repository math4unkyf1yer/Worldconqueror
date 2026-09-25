using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireRatePreviewUI : BasePreview
{
    [SerializeField] PreviewTroop troop;
    [SerializeField] PreviewProjectile[] projectiles;
    [SerializeField] Transform origin;
    [SerializeField] Transform[] targets;

    public override void Show(TerritoryType type, UnitStats troopStats, TerretoryData ter)
    {
        gameObject.SetActive(true);

        Color c = new Color(0.4f, 0.6f, 1f, 1);
        troop.SpriteLook(3, c);

        foreach (var t in targets)
        {
            t.GetComponent<Image>().enabled = true;
        }

        PreviewProjectileManager.Instance.StartFireRatePreview(projectiles, origin, targets, troopStats.fireRate);
    }
}
