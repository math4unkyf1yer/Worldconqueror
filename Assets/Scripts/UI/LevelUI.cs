using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static LevelData;

public class LevelUI : MonoBehaviour
{


    [SerializeField] private GameObject levelImage;
    [SerializeField] private GameObject lineImage;
    [SerializeField] private GameObject hazardImage;

    public int howManyLevelHolder = 5;
    public int mapCount;
    public int mapCurrentCount;

    private float startPositionX = -5.3f;
    private float positionx;

    private bool changePage;

    //boarders and padding from the other 
    Transform newPos;
    Transform oldPos;

    float positionY;
    public List<float> levelPositionY = new List<float>();
    public List<GameObject> holdLevelObj = new List<GameObject>();
    public List<GameObject> holdHazard = new List<GameObject>();

    int levelIndex;
    int localLevelIndex;
    AssignLevel levelScript;
    private UIRoads roadScript;
    public PopulateTerritory populationScript;
    public List<VegetationData> populatedItem;
    [SerializeField] GameObject parentToObjects;
    public static LevelUI Instance { get; private set; }

    //Color for the map levels
    public Color finishLevel;
    public Color onLevel;

    private void Awake()
    {
        // 1. If an instance already exists and it isn't this one, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        roadScript = GetComponent<UIRoads>();
    }


    public void RefreshMap(bool firsttime)
    {
        if(levelScript == null)
        {
            levelScript = AssignLevel.Instance;
            mapCount = levelScript.LevelData.Length / howManyLevelHolder;
        }
        levelIndex = levelScript.levelCount;
        localLevelIndex = levelIndex;
        positionx = startPositionX;

        int subtract = mapCurrentCount * howManyLevelHolder;


        localLevelIndex -= subtract;

        for (int i = 0; i < howManyLevelHolder; i++)
        {
            if (mapCurrentCount == mapCount) { break; }


            if (firsttime || changePage)
            {
                GameObject level = Instantiate(levelImage, parentToObjects.transform);
                oldPos = newPos;
                newPos = level.transform;
                Vector3 pos = level.transform.position;
                pos.x = positionx;
                positionx += 2.5f;
                if (i < levelPositionY.Count)
                {
                    positionY = levelPositionY[i];
                }
                else
                {
                    positionY = Random.Range(-2.3f, 2.6f);
                    levelPositionY.Add(positionY);
                }
                pos.y = positionY;

                level.transform.position = pos;
                holdLevelObj.Add(level);

                if (levelScript.LevelData[i].hasHazard)
                {
                   /* GameObject hazard = Instantiate(hazardImage, parentToObjects.transform);
                    hazard.transform.position = level.transform.position;
                    holdHazard.Add(hazard);*/
                }

            }

            if (i < localLevelIndex)
            {
                Transform child = holdLevelObj[i].transform.Find("Flag");
                Transform selected = holdLevelObj[i].transform.Find("CircleSelect");
                Transform outline = holdLevelObj[i].transform.Find("Outline");
                selected.gameObject.SetActive(false);
                child.GetComponent<SpriteRenderer>().color = Color.blue;
                outline.GetComponent<SpriteRenderer>().color = finishLevel;
            }
            else if (i == localLevelIndex)
            {
                Transform child = holdLevelObj[i].transform.Find("Flag");
                Transform selected = holdLevelObj[i].transform.Find("CircleSelect");
                Transform outline = holdLevelObj[i].transform.Find("Outline");
                selected.gameObject.SetActive(true);
                child.GetComponent<SpriteRenderer>().color = Color.white;
                outline.GetComponent<SpriteRenderer>().color = onLevel;
            }

                RefreshLine(oldPos, imageNew: newPos, i, firsttime);
        }
        foreach (GameObject level in holdLevelObj)
        {
            level.transform.SetAsLastSibling();
        }

        if (firsttime) { populationScript.Setup(populatedItem); }
        if (changePage) { populationScript.ClearTerritory(); populationScript.Setup(populatedItem); }
        if (changePage) { changePage = false; }
    }
    public void RefreshLine(Transform imageOld, Transform imageNew, int lineIndex, bool firsttime)
    {
        if(imageOld == null || imageNew == null) { return; }

        lineIndex -= 1;
        Vector3 posA = imageOld.position;
        Vector3 posB = imageNew.position;
        if (firsttime)
        {
            roadScript.BuildRoad(posA, posB, parentToObjects.transform);
        }
        else if (changePage)
        {
            roadScript.ReshapeRoad(posA, posB, lineIndex);
        }

        if(lineIndex + 1 <= localLevelIndex)
        {
            if(lineIndex  < 0) { return; }
            //change color blue 
            roadScript.ChangeRoadTexture(lineIndex);
        }
    }

    public void ClearMap()
    {

        // Destroy level icons
        foreach (GameObject level in holdLevelObj)
            Destroy(level);

        // Destroy hazard icons
        foreach (GameObject hazard in holdHazard)
            Destroy(hazard);

        //clear positions y
        levelPositionY.Clear();

        // Clear lists
        holdLevelObj.Clear();
        holdHazard.Clear();
    }


    public void UpdateMap()
    {
        int subtract = mapCurrentCount * howManyLevelHolder;
        localLevelIndex = levelScript.levelCount;
        localLevelIndex -= subtract;
        if (localLevelIndex == howManyLevelHolder)
        {
            ClearMap();
            mapCurrentCount++;
            changePage = true;
        }
    }
}
