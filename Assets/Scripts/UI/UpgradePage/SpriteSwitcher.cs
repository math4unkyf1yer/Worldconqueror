using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    public Image target;
    public Sprite[] sprites;

    public void SetSprite(int index)
    {
        target.sprite = sprites[index];
    }
}
