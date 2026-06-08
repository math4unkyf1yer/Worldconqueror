using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerritorySlider : MonoBehaviour
{
    public Owner owner;
    private int totalTerritories;
    private int ownerTerritories;

    public Slider territorySlider;
    public Image sliderColor;

    [SerializeField] private TextMeshProUGUI sliderText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image nameImage;

    public void InitializedBar(LevelData level)
    {
        totalTerritories = level.terretories.Count;
    }

    public void SetSliderOwner(Owner ownerGiven)
    {
        owner = ownerGiven;
        switch (owner)
        {
            case Owner.Player:
                sliderColor.color = Color.blue;
                nameImage.color = Color.blue;
                nameText.text = "Player";
                break;
            case Owner.AI1:
                sliderColor.color = Color.red;
                nameImage.color = Color.red;
                nameText.text = "CPU";
                break;
            case Owner.AI2:
                sliderColor.color = Color.yellow;
                nameImage.color = Color.yellow;
                nameText.text = "CPU";
                break;
            case Owner.AI3:
                sliderColor.color = Color.green;
                nameImage.color = Color.green;
                nameText.text = "CPU";
                break;
        }
        SetOwnerTerritories(owner,Owner.Neutral);
    }
    public void SetOwnerTerritories(Owner NewOwner,Owner OldOwner)
    {
        if(owner == NewOwner)
        {
            ownerTerritories++;
            UpdateSlider();
        }else if(owner == OldOwner)
        {
            ownerTerritories--;
            UpdateSlider();
        }
    }

    private void UpdateSlider()
    {
        if (totalTerritories == 0) return;

        float progress = (float)ownerTerritories / totalTerritories;
        territorySlider.value = progress;

        sliderText.text = "Territories " + ownerTerritories + "/" + totalTerritories;
    }

}
