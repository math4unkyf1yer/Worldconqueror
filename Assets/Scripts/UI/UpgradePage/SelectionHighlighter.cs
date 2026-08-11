using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionHighlighter : MonoBehaviour
{

    public Color activeColor;
    public Color inactiveColor;

    private Button oldActivatedButton;
    private GameObject oldOverlay;

    //keep this to add the gold animation arround the button 
    public void ChangeButtonColor(Button btn)
    {
        if(oldActivatedButton != null)
        {
            oldOverlay.SetActive(false);
        }
        oldActivatedButton = btn;

        // Loop through all children using Transform
        foreach (Transform child in oldActivatedButton.transform)
        {
            if (child.CompareTag("Overlay"))
            {
                child.gameObject.SetActive(true);
                oldOverlay = child.gameObject;
            }
        }
    }
}
