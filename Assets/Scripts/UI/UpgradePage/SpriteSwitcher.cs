using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    public Image target;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI infoNameText;
    public Sprite[] sprites;
    [TextArea]
    public string[] info;
    public string[] infoName;


    
    public void ChangeInfo(int index)
    {
        SetSprite(index);
        SetInfo(index);
    }
    public void SetSprite(int index)
    {
        target.sprite = sprites[index];
    }
    public void SetInfo(int index)
    {
        if(infoText == null || infoNameText == null) { return; }

        infoText.text = info[index].ToString();
        infoNameText.text = infoName[index].ToString();
    }
}
