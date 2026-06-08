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
    public MapSize mapSize; // NEW
    public DifficultyConfiguration difficulty;

    [SerializeField] private TMP_Dropdown sizeMapDropdown;
    [SerializeField] private TMP_Dropdown enemyDropdown;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private Toggle hazardToggle;

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

    public void CustomPlay()
    {
        AssignLevel.Instance.SetupLevel(enemyCount,hasHazard,mapSize,difficulty);
        SceneManager.LoadScene(1);
    }
}
