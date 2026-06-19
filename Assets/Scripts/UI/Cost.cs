using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cost : MonoBehaviour
{
  private AssignLevel assignLevelScript;
    [SerializeField] private TextMeshProUGUI costtext;

    [SerializeField] private int whichUpgrade;
    // Start is called before the first frame update
    void Start()
    {
        assignLevelScript = AssignLevel.Instance;

        costtext.text = "Cost:  " + assignLevelScript.cost[whichUpgrade].ToString();
    }

    public void ClickUpgrade()// 1 is production, 2 is move speed and 3 is capacity
    {
        if (assignLevelScript.TryUpgradeTroop(whichUpgrade))
        {
            costtext.text = "Cost:  " + assignLevelScript.cost[whichUpgrade].ToString();
            Menu.Instance.SetCoinText();
        }
        else { Debug.Log("not enough coins"); }
    }

    public void IncreaseAmountTextChange()
    {
        Debug.Log("This will Increase by this much");
    }
}
