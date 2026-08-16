using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomLevelConfig : MonoBehaviour
{
    public int enemyCount = 0;
    public bool hasHazard;
    public bool[] territoryTypes = new bool[3];
    public MapSize mapSize; // NEW
    public DifficultyConfiguration difficulty;

    [SerializeField] private GameObject menuObj;
    [SerializeField] private TMP_Dropdown sizeMapDropdown;
    [SerializeField] private TMP_Dropdown enemyDropdown;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private Toggle hazardToggle;
    [SerializeField] private Toggle assassinTerToggle;
    [SerializeField] private Toggle dwarfTerToggle;
    [SerializeField] private Toggle mageTerToggle;
    [SerializeField] private Toggle rangerTerToggle;

    [SerializeField] private GameObject[] blockade;

    private AssignLevel levelScript;

    private void Start()
    {
        levelScript = AssignLevel.Instance;
        CheckIfUnlocked();
    }
    public void CheckIfUnlocked()
    {
        if (levelScript == null) { levelScript = AssignLevel.Instance; }
        if (levelScript.unlockedAssassin)
        {
            blockade[0].SetActive(false);
        }
        if(levelScript.unlockedDwarfs)
        {
            blockade[1].SetActive(false);
        }
        if(levelScript.unlockedMage)
        {
            blockade[2].SetActive(false);
        }
        if (levelScript.unlockedRanger)
        {
            blockade[3].SetActive(false);
        }
    }

    public void SetEnemyCount()
    {
        int value = enemyDropdown.value;
        enemyCount = value;
        Debug.Log(value);
    }

    public void SetHasHazard()
    {
        bool value = hazardToggle.isOn;
        Debug.Log(value);
        hasHazard = value;
    }

    public void SetMapSize()
    {
        int index = sizeMapDropdown.value;

        mapSize = (MapSize)index;
        Debug.Log(mapSize);
    }

    public void SetDifficulty()
    {
        int index = difficultyDropdown.value;
        difficulty.setting = (Difficulty)index;
    }

    public void SetAssassin()
    {
        bool value = assassinTerToggle.isOn;
        Debug.Log(value);
        territoryTypes[1] = value;
    }
    public void SetDwarf()
    {
        bool value = dwarfTerToggle.isOn;
        territoryTypes[2] = value;
        Debug.Log(value);
    }
    public void SetMage()
    {
        bool value = mageTerToggle.isOn;
        territoryTypes[3] = value;
        Debug.Log(value);
    }
    public void SetRanger()
    {
        bool value = rangerTerToggle.isOn;
        territoryTypes[4] = value;
        Debug.Log(value);
    }

    public void CustomPlay()
    {
        levelScript.SetupLevel(enemyCount,hasHazard,mapSize,difficulty,territoryTypes);
        gameObject.SetActive(false);
        menuObj.SetActive(false);
        LoadScreen.Instance.LoadScene(1);
    }
}
