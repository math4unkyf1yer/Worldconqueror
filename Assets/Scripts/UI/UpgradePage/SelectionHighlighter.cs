using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionHighlighter : MonoBehaviour
{

    public Color activeColor;
    public Color inactiveColor;

    private Button oldActivatedButton;
    public void ChangeButtonColor(Button btn)
    {
        if (oldActivatedButton != null)
        {
            ColorBlock cbOld = oldActivatedButton.colors;
            cbOld.normalColor = inactiveColor;
            cbOld.selectedColor = inactiveColor;
            oldActivatedButton.colors = cbOld;
        }

        ColorBlock cb = btn.colors;
        cb.normalColor = activeColor;
        cb.selectedColor = activeColor;
        btn.colors = cb;

        oldActivatedButton = btn;
    }
}
