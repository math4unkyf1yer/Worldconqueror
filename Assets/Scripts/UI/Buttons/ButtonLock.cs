using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonLock : MonoBehaviour
{
    [SerializeField] GameObject lockImage;
    [SerializeField] ButtonLockController controller;
    [SerializeField] int unlockedLevel;
    [SerializeField] string whichButton;

    [SerializeField] ButtonHovering butHovering;

    private bool isUnlocked;

    //make it pop

    private void Start()
    {
        if(butHovering == null) { butHovering = GetComponent<ButtonHovering>(); }
    }

    public void CanUnlockedButton(int level) // need to add the tutorial button looks 
    {
        if (!isUnlocked)
        {
            if (unlockedLevel <= level)
            {
                lockImage.SetActive(false);
                isUnlocked = true;
                controller.CanUnlockedButton(whichButton);
                butHovering.canHover = false;
            }
        }
    }

    public int GetLevel()
    {
        return unlockedLevel;
    }
}
