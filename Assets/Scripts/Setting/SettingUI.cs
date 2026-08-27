using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] Button musicBut;
    [SerializeField] Button sfxBut;
    public Color clickColor;
    private Color OldColor;

    private bool musicOn = true;
    private bool sfxOn = true;
    public void MusicClick()
    {
        Debug.Log("Click");
        if (musicOn)
        {
            ColorBlock cb = musicBut.colors;
            OldColor = cb.normalColor;
            cb.normalColor = clickColor;
            cb.selectedColor = clickColor;// your new color
            musicBut.colors = cb;
            musicOn = false;
        }
        else
        {
            ColorBlock cb = musicBut.colors;
            cb.normalColor = OldColor;
            cb.selectedColor = OldColor;// your new color
            musicBut.colors = cb;
            musicOn = true;
        }
    }
    public void SFXClick()
    {
        if (sfxOn)
        {
            ColorBlock cb = sfxBut.colors;
            OldColor = cb.normalColor;
            cb.normalColor = clickColor;
            cb.selectedColor = clickColor;// your new color
            sfxBut.colors = cb;
            sfxOn = false;
        }
        else
        {
            ColorBlock cb = sfxBut.colors;
            cb.normalColor = OldColor;
            cb.selectedColor = OldColor;// your new color
            sfxBut.colors = cb;
            sfxOn = true;
        }
    }
}
