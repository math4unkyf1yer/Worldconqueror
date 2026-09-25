using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PreviewManager : MonoBehaviour
{
    [Header("Previews")]
    public BasePreview speed;
    public BasePreview vigor;
    public BasePreview sturdy;
    public BasePreview critical;
    public BasePreview strength;
    public BasePreview attackRange;
    public BasePreview fireRate;
    public BasePreview production;
    public BasePreview capacity;
    public BasePreview radius;

    private UnitStats troop;
    private TerretoryData ter;

    public void SetData(UnitStats t, TerretoryData td)
    {
        troop = t;
        ter = td;
    }

    public void Show(BasePreview preview, TerritoryType type)
    {
        preview.Show(type, troop, ter);
    }

    public void CloseAll()
    {
        speed.Hide();
        vigor.Hide();
        sturdy.Hide();
        critical.Hide();
        strength.Hide();
        attackRange.Hide();
        fireRate.Hide();
        production.Hide();
        capacity.Hide();
        radius.Hide();

        PreviewProjectileManager.Instance.StopPreview();
    }
}