using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PreviewManager : MonoBehaviour
{
    [Header("Speed Preview")]
    [SerializeField] GameObject speedPreview;
    public GameObject speedTerritory;
    public GameObject speedTerritory2;

    [Header("Vigor Preview")]
    [SerializeField] GameObject vigorPreview;
    [SerializeField] GameObject vigorTerritory;
    [SerializeField] GameObject vigorTerritory2;
    [SerializeField] GameObject vigorTroop;
    [SerializeField] GameObject vigorTroop2;

    [Header("Strenght Preview")]
    [SerializeField] GameObject strenghtPreview;
    [SerializeField] GameObject strenghtTerritory;
    [SerializeField] GameObject strenghtTerritory2;
    [SerializeField] GameObject strenghtTroop;

    [Header("Attack Range Preview")]
    [SerializeField] GameObject attackRangePreview;
    [SerializeField] GameObject attackRangeTroop;
    [SerializeField] GameObject attackRangeTroop2;
    [SerializeField] GameObject projectileAttackRange;

    [Header("Fire Rate Preview")]
    [SerializeField] GameObject fireRatePreview;
    [SerializeField] GameObject fireRateTroop;
    [SerializeField] GameObject fireRateTroop2;
    [SerializeField] GameObject fireRateTroop3;
    [SerializeField] GameObject fireRateTroop4;
    [SerializeField] PreviewProjectile[] projectilesFireRate;

    [Header("Sturdy Preview")]
    [SerializeField] GameObject sturdyPreview;
    [SerializeField] GameObject sturdyTerritory;
    [SerializeField] GameObject sturdyTerritory2;
    [SerializeField] GameObject sturdyTroop;
    [SerializeField] GameObject sturdyTroop2;

    [Header("Critical Preview")]
    [SerializeField] GameObject critPreview;
    [SerializeField] GameObject critTerritory;
    [SerializeField] GameObject critTerritory2;
    [SerializeField] GameObject critTroop;
    [SerializeField] GameObject critTroop2;

    [Header("Territory Preview")]
    [SerializeField] GameObject terPreview;

    private UnitStats statsTroop;
    private TerretoryData statsTer;

    public void SetData(UnitStats Troop,TerretoryData ter)
    {
        statsTroop = Troop;
        statsTer = ter;
    }
    public void SpeedPreview(TerritoryType territoryType)
    {
        speedPreview.SetActive(true);
        PreviewTerritory previewTer1 = speedTerritory.GetComponent<PreviewTerritory>();

        previewTer1.SetLook(territoryType);
        previewTer1.SpeedPreview(speedTerritory2.transform, statsTroop.moveSpeed);
    }
    public void VigorPreview(TerritoryType territoryType)
    {
        vigorPreview.SetActive(true);

        PreviewTerritory previewTer1 = vigorTerritory.GetComponent<PreviewTerritory>();
        PreviewTerritory previewTer2 = vigorTerritory2.GetComponent<PreviewTerritory>();

        previewTer1.SetLook(territoryType);
        previewTer2.SetLook(territoryType);

        previewTer1.VigorPreview(vigorTerritory2.transform, statsTroop.moveSpeed,1,0.9f,vigorTroop2);
        previewTer2.VigorPreview(vigorTerritory.transform, statsTroop.moveSpeed, 0.9f,1, null);
    }
    public void SturdyPreview(TerritoryType territoryType)
    {
        sturdyPreview.SetActive(true);

        PreviewTerritory previewTer1 = sturdyTerritory.GetComponent<PreviewTerritory>();
        PreviewTerritory previewTer2 = sturdyTerritory2.GetComponent<PreviewTerritory>();

        previewTer1.SetLook(territoryType);
        previewTer2.SetLook (territoryType);

        previewTer1.VigorPreview(sturdyTerritory2.transform, statsTroop.moveSpeed, 1, 0.9f, sturdyTroop2);
        previewTer2.VigorPreview(sturdyTerritory.transform, statsTroop.moveSpeed, 0.9f, 1, null);
    }
    public void CriticalPreview(TerritoryType territoryType)
    {
        critPreview.SetActive(true);

        PreviewTerritory previewTer1 = critTerritory.GetComponent<PreviewTerritory>();
        PreviewTerritory previewTer2 = critTerritory2.GetComponent<PreviewTerritory>();

        previewTer1.SetLook(territoryType);
        previewTer2.SetLook(territoryType);

        previewTer1.VigorPreview(critTerritory2.transform, statsTroop.moveSpeed, 1, 0.9f, critTroop2);
        previewTer2.VigorPreview(critTerritory.transform, statsTroop.moveSpeed, 0.9f, 1, null);
    }
    public void StrengthPreview(TerritoryType territoryType)
    {
        strenghtPreview.SetActive(true);
        //show 
        PreviewTerritory previewTer1 = strenghtTerritory.GetComponent<PreviewTerritory>();
        PreviewTerritory previewTer2 = strenghtTerritory2.GetComponent<PreviewTerritory>();

        previewTer1.SetLook(territoryType);

        previewTer2.ChangeUnitNumber(0);
        previewTer1.StrenghtPreview(vigorTerritory2.transform, statsTroop.moveSpeed, statsTroop.strenght, previewTer2);
    }
    public void AttackRangePreview()
    {
        attackRangePreview.SetActive(true);

        PreviewTroop previewTroop = attackRangeTroop.GetComponent<PreviewTroop>();
        PreviewProjectile previewProjectile = projectileAttackRange.GetComponent<PreviewProjectile>();
        Color c = new Color(0.4f, 0.6f, 1f, 1);
        previewTroop.SpriteLook(3, c);

        PreviewProjectileManager.Instance.StartAttackRangePreview(previewProjectile,attackRangeTroop.transform,attackRangeTroop2.transform);
    }
    public void FireRatePreview()
    {
        fireRatePreview.SetActive(true);

        Color c = new Color(0.4f, 0.6f, 1f, 1);
        fireRateTroop.GetComponent<PreviewTroop>().SpriteLook(3, c);

        Transform[] targets = new Transform[]
        {
           fireRateTroop2.transform,
           fireRateTroop3.transform,
           fireRateTroop4.transform
        };

        foreach (var item in targets)
        {
            Image look = item.GetComponent<Image>();
            look.enabled = true;
        }

        PreviewProjectileManager.Instance.StartFireRatePreview( projectilesFireRate, fireRateTroop.transform,targets,statsTroop.fireRate );
    }

    public void TerritoryPreview(TerritoryType territoryType)
    {
        //call whichever we want right now 
    }
    void ProductionSpeedPreview() 
    {
        // preview territory 
        // production speed preview set amount to 0 
        //as you start start the unit count (need the production speed)

    }
     void CapacityPreview()
    {
        //preview territory 
        //max capacity right away and just shows on the set up 
    }
     void RadiusPreview()
    {
        //territory preview 
        //troop preview

        //set look for them

        //troop speed preview target territory and start the start 

        //territory set up do the effect base on type 
    }
    public void ClosePreview()
    {
        speedPreview.SetActive(false);
        vigorPreview.SetActive(false);
        strenghtPreview.SetActive(false);
        attackRangePreview.SetActive(false);
        sturdyPreview.SetActive(false);
        critPreview.SetActive(false);

        PreviewProjectileManager.Instance.StopPreview();
        fireRatePreview.SetActive(false);
    }
}