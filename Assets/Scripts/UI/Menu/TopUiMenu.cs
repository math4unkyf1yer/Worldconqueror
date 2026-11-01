using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUiMenu : MonoBehaviour
{
    [SerializeField] private GameObject levelInfoPage;
    public static TopUiMenu Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CloseInfoPage()
    {
        levelInfoPage.SetActive(false);
    }
    public void OpenInfoPage()
    {
        levelInfoPage.SetActive(true);
    }
}
