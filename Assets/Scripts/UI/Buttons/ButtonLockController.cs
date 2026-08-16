using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonLockController : MonoBehaviour
{
    //controls if you can press button 
    public bool unlockTroopUpgrades { get; private set; } = false;
    public bool unlockTerritoryUpgrades { get; private set; } = false;
    public bool unlockShop { get; private set; } = false;

    public bool unlockAssassinUpgrades {  get; private set; } = false;
    public bool unlockDwarfUpgrades { get; private set; } = false;
    public bool unlockMageUpgrades { get; private set; } = false;
    public bool unlockRangerUpgrades { get; private set; } = false;
    public bool unlockFortUpgrades { get; private set; } = false;

    [SerializeField] private List<GameObject> allButtons = new List<GameObject>();
    private List<ButtonLock> buttonsLocks = new List<ButtonLock>();

    public static ButtonLockController Instance { get; private set; }

    private void Awake()
    {
        // 1. If an instance already exists and it isn't this one, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CheckAllButtons(int levelCount)
    {
        if(buttonsLocks.Count == 0)
        {
            foreach (GameObject button in allButtons)
            {
                if (button != null)
                {
                    ButtonLock buttonLock = button.GetComponent<ButtonLock>();
                    buttonsLocks.Add(buttonLock);
                }
            }
        }

        foreach(ButtonLock locks  in buttonsLocks)
        {
            locks.CanUnlockedButton(levelCount);
        }
    }

    public void CanUnlockedButton(string whichButton)
    {
        switch (whichButton)
        {
            case "troopUpgrade": 
                SetTroopUpgradesPage(); break;
            case "territoryUpgrade":
                SetTerritoryUpgradesPage(); break;
            case "AssassinTroops":
                SetAssassinPage(); break;
            case "DwarfTroops":
                SetDwarfPage(); break;
            case "MageTroops":
                SetMagePage(); break;
            case "RangerTroops":
                SetRangerPage(); break;
            case "Fort":
                SetFort(); break;
            case "Shop":
                SetShop(); break;
                
        }
    }
    private void SetTroopUpgradesPage()
    {
        unlockTroopUpgrades = true;
    }
    private void SetTerritoryUpgradesPage()
    {
        unlockTerritoryUpgrades = true;
    }
    private void SetAssassinPage()
    {
        unlockAssassinUpgrades = true;
    }
    private void SetDwarfPage()
    {
        unlockDwarfUpgrades = true;
    }
    private void SetMagePage()
    {
        unlockMageUpgrades = true;
    }
    private void SetRangerPage()
    {
        unlockRangerUpgrades = true;
    }

    private void SetFort()
    {
        unlockFortUpgrades = true;
    }

    private void SetShop()
    {
        unlockShop = true;
    }

}
