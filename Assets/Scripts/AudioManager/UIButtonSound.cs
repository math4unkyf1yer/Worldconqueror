using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    private ButtonLock lockScript;
    void Start()
    {
        lockScript = GetComponent<ButtonLock>();

        //need to check if lock is access 
        GetComponent<Button>().onClick.AddListener(() =>
        {
            // Only play sound if button is NOT locked
            if (lockScript == null || lockScript.isUnlocked)
            {
                AssignLevel.Instance.audioManager.PlayButtonSound();
            }
        });
    }
}
