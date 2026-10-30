using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewHazard : MonoBehaviour
{
    public HazardType type;

    private PreviewTroop troopPreview;
    private RectTransform hazardRect;
    private Image image;
    private bool troopInside;


    //Color 
    public Color speedColor;
    public Color slowColor;
    public Color damageColor;

    private void Awake()
    {
        hazardRect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void SetHazard(PreviewTroop troop, HazardType hazType)
    {
        troopPreview = troop;
        this.type = hazType;

        ChangeColor();
    }

    private void Update()
    {
        if (troopPreview == null)
            return;

        RectTransform troopRect = troopPreview.GetComponent<RectTransform>();

        bool overlapping = RectTransformUtility.RectangleContainsScreenPoint(hazardRect,RectTransformUtility.WorldToScreenPoint(null, troopRect.position));

        if (overlapping && !troopInside)
        {
            troopInside = true;
            EnterHazard();
        }
        else if (!overlapping && troopInside)
        {
            troopInside = false;
            ResetSpeed();
        }
    }

    void EnterHazard()
    {
        if(type == HazardType.Slow)
        {
            troopPreview.speedBuff(0.6f);
        }
        else if(type == HazardType.Speed)
        {
            troopPreview.speedBuff(1.4f);
        }
        else
        {

        }
    }

    void ResetSpeed()
    {
        troopPreview.ResetSpeed();
    }

    void ChangeColor()
    {
        if (type == HazardType.Slow)
        {
            image.color = slowColor;
        }
        else if (type == HazardType.Speed)
        {
            image.color = speedColor;
        }
        else
        {
            image.color = damageColor;
        }
    }

}
