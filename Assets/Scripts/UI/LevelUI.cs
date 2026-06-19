using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{


    [SerializeField] private GameObject levelImage;
    [SerializeField] private GameObject lineImage;
    [SerializeField] private Texture Xmark;

    public int howManyLevelHolder = 5;

    private float startPositionX = -28f;
    private float positionx;

    //boarders and padding from the other 
    RectTransform newPos;
    RectTransform oldPos;

    int levelIndex;

    private void Start()
    {
        levelIndex = AssignLevel.Instance.levelCount;
        positionx = startPositionX;
        for(int i = 0; i < howManyLevelHolder; i++)
        {
            GameObject level = Instantiate(levelImage, this.transform);
            oldPos = newPos;
            newPos = level.GetComponent<RectTransform>();
            Vector3 pos = level.transform.localPosition;
            pos.x = positionx;
            positionx += 13;
            float positionY = Random.Range(-18, 20);
            pos.y = positionY;
            level.transform.localPosition = pos;

            if(i < levelIndex)
            {
                level.GetComponent<RawImage>().color = Color.blue;
            }else if(i == levelIndex)
            {
                level.GetComponent<RawImage>().texture = Xmark;
            }

            RefreshLine(oldPos, newPos, i);
            level.transform.SetAsLastSibling();
        }
    }
    public void RefreshLine(RectTransform imageOld, RectTransform imageNew, int lineIndex)
    {
        if(imageOld == null || imageNew == null) { return; }

        GameObject lineObject = Instantiate(lineImage, this.transform);
        lineObject.transform.SetAsFirstSibling();
        RectTransform line = lineObject.GetComponent<RectTransform>();
        Vector3 posA = imageOld.anchoredPosition;
        Vector3 posB = imageNew.anchoredPosition;

        // Midpoint
        line.anchoredPosition = (posA + posB) / 2f;

        // Direction
        Vector3 dir = posB - posA;

        // Length
        float distance = dir.magnitude;
        line.sizeDelta = new Vector2(distance, line.sizeDelta.y);

        // Rotation
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        line.localRotation = Quaternion.Euler(0, 0, angle);

        if(lineIndex <= levelIndex)
        {
            //change color blue 
            lineObject.GetComponent<Image>().color = Color.blue;
        }
    }
}
