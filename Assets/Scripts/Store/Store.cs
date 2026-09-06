using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField] Button[] storeButtons;

    [SerializeField] SelectionHighlighter SelectionOutline;

    private void Start()
    {
        SelectionOutline.ChangeButtonColor(storeButtons[0]);
    }
    //bools
    public void ShowRewardedAd(string rewardType)
    {
        Debug.Log("Ads");
    }
    public void BuyCoin()
    {
        Debug.Log("Coins");
    }

    public void OpenStore(int whichStore)
    {
        SelectionOutline.ChangeButtonColor(storeButtons[whichStore]);
    }

    
    public void CoinStore()
    {

    }
    public void DiamondStore()
    {

    }
}

