using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField] Button[] storeButtons;

    [SerializeField] GameObject coinPage;
    [SerializeField] GameObject diamondPage;

    [SerializeField] SelectionHighlighter SelectionOutline;

    private AssignLevel gameManager;
    private Menu menu;

    private int[] packCoinCost = new int[4];
    private int[] packDiamondCost = new int[4];
    private int[] goldGain = new int[4];
    private int[] diamondGain = new int[4];

    private void Start()
    {
        SelectionOutline.ChangeButtonColor(storeButtons[0]);
        gameManager = AssignLevel.Instance;
        menu = Menu.Instance;
        packCoinCost[0] = 10;
        packCoinCost[1] = 20;
        packCoinCost[2] = 30;
        packCoinCost[3] = 50;
        goldGain[0] = 25;
        goldGain[1] = 50;
        goldGain[2] = 100;
        goldGain[3] = 250;
        diamondGain[0] = 25;
        diamondGain[1] = 50;
        diamondGain[2] = 100;
        diamondGain[3] = 250;
    }
    //bools
    public void ShowRewardedAd(string rewardType)
    {
        Debug.Log("Ads");
    }
    public void BuyCoin(int whichPack)
    {
        if(gameManager.GetDiamond() >= packCoinCost[whichPack])
        {
            Debug.Log("enough diamound");
            int newDiamond = gameManager.GetDiamond() - packCoinCost[whichPack];
            gameManager.SetDiamonds(newDiamond);
            //gain coin
            int newCoinAmount = gameManager.GetCoin() + goldGain[whichPack];
            gameManager.SetCoin(newCoinAmount);
            menu.SetText();
        }
        else
        {
            DiamondStore();
        }
    }

    public void BuyDiamond(int whichPack)
    {
        int newDiamondAmount = gameManager.GetDiamond() + diamondGain[whichPack];
        gameManager.SetDiamonds(newDiamondAmount);
        menu.SetText();
    }

    public void OpenStore(int whichStore)
    {
        switch (whichStore)
        {
            case 0:CoinStore(); break;
            case 1:DiamondStore(); break;
        }
        SelectionOutline.ChangeButtonColor(storeButtons[whichStore]);
    }

    
    public void CoinStore()
    {
        coinPage.SetActive(true);
        diamondPage.SetActive(false);
    }
    public void DiamondStore()
    {
        diamondPage.SetActive(true);
        coinPage.SetActive(false);
    }

}

