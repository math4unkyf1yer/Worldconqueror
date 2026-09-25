using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangePreview : BasePreview
{
    [SerializeField] PreviewTroop troop;
    [SerializeField] PreviewProjectile projectile;
    [SerializeField] Transform origin;
    [SerializeField] Transform target;

    public override void Show(TerritoryType type, UnitStats troopStats, TerretoryData ter)
    {
        gameObject.SetActive(true);

        Color c = new Color(0.4f, 0.6f, 1f, 1);
        troop.SpriteLook(3, c);

        PreviewProjectileManager.Instance.StartAttackRangePreview(projectile, origin, target);
    }
}
