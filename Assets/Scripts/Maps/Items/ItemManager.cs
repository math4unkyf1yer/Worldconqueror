using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemManager : MonoBehaviour
{
    [SerializeField] GameObject hazard;
    [SerializeField] GameObject fireStorm;
    [SerializeField] GameObject Dupli;
    GameObject duplicateCl;
    GameObject fireCl;

    float radiusZone;
    Vector3 positionZone;
    HazardController hazardController;

    public int speedAmount { get; private set; } = 3;
    public int slowAmount { get; private set; } = 3;
    public int fireAmount { get; private set; } = 3;
    public int duplicationAmount { get; private set; } = 3;

    public void PlaceItem(ItemType type,float radius,Vector3 position)
    {
        positionZone = position;
        radiusZone = radius;

        switch (type)
        {
            case ItemType.Slow:
                ZonePlace(HazardType.Slow);
                slowAmount--;
                break;
            case ItemType.Speed:
                ZonePlace(HazardType.Speed);
                speedAmount--;
                break;
            case ItemType.FireStorm:

                FireStormS();
                fireAmount--;
                break;
            case ItemType.Duplicate:
                Duplicate();
                duplicationAmount--;
                break;
            
        }
    }


    void Duplicate()
    {
        if(duplicateCl == null)
        {
            duplicateCl = Instantiate(Dupli, positionZone, Quaternion.identity);
        }
        else
        {
            duplicateCl.transform.position = positionZone;
            duplicateCl.SetActive(true);
        }

        SetRadius(radiusZone,  duplicateCl);

        StartCoroutine(StopItem(0.5f));
    }
    void FireStormS()
    {
        if(fireCl == null)
        {
            fireCl = Instantiate(fireStorm, positionZone,Quaternion.identity);
        }
        else
        {
            fireCl.transform.position = positionZone;
            fireCl.SetActive(true);
        }

        //fire script do what you do for radius for the damage
        FireStorm fire = fireCl.GetComponent<FireStorm>();

        SetRadius(radiusZone,fireCl);

        StartCoroutine(StopItem(0.5f));
    }
    IEnumerator StopItem(float timer)
    {
        //hide it back 
        yield return new WaitForSeconds(timer);
        if(fireCl && fireCl.activeInHierarchy)
            fireCl.SetActive(false);

        if(duplicateCl && duplicateCl.activeInHierarchy)
            duplicateCl.SetActive(false);
    }

    void ZonePlace(HazardType type)
    {
        GameObject hazardCl = Instantiate(hazard, positionZone, Quaternion.identity);

        hazardController = hazardCl.GetComponent<HazardController>();

        // Create a unique zone for this hazard
        HazardZone newZone = new HazardZone();

        newZone.Position = positionZone;
        newZone.intensity = radiusZone;
        newZone.Type = type;

        hazardController.SetUp(newZone);
    }

    public void SetRadius(float radius,GameObject Object)
    {
        Object.transform.localScale = Vector3.one * Mathf.Lerp(0.3f, 2f, radius);
    }


    public int GetAmount(ItemType type)
    {
        switch (type)
        {
            case ItemType.Speed: return speedAmount;
            case ItemType.Slow: return slowAmount;
            case ItemType.FireStorm: return fireAmount;
            case ItemType.Duplicate: return duplicationAmount;
        }

        return 0;
    }

    public int GetItemPrice(ItemType type)
    {
        switch (type)
        {
            case ItemType.Speed: return 10;
            case ItemType.Slow: return 10;
            case ItemType.FireStorm: return 10;
            case ItemType.Duplicate: return 10;
        }

        return 0;
    }

    //Setting Items
    public void SetSpeed(int amount)
    {
        speedAmount = amount;
    }
    public void SetSlow(int amount)
    {
        slowAmount = amount;
    }
    public void SetFire(int amount)
    {
        fireAmount = amount;
    }
    public void SetDuplication(int amount)
    {
        duplicationAmount = amount;
    }

    //store adding items 
    public void BuySpeed()
    {
        BuyItem(ItemType.Speed);
    }

    public void BuySlow()
    {
        BuyItem(ItemType.Slow);
    }

    public void BuyFireStorm()
    {
        BuyItem(ItemType.FireStorm);
    }

    public void BuyDuplicate()
    {
        BuyItem(ItemType.Duplicate);
    }
    public void BuyItem(ItemType type)
    {
        int price = GetItemPrice(type);

        if (AssignLevel.Instance.GetCoin() >= price)
        {
            AddItem(type);
        }
        else
        {
            Debug.Log("not enought money");
        }
    }
    public void AddItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.Speed:
                speedAmount ++;
                Debug.Log(speedAmount);
                break;

            case ItemType.Slow:
                slowAmount ++;
                Debug.Log(slowAmount);
                break;

            case ItemType.FireStorm:
                fireAmount ++;
                Debug.Log(fireAmount);
                break;

            case ItemType.Duplicate:
                duplicationAmount ++;
                Debug.Log(duplicationAmount);
                break;
        }
    }
}
