using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class TopUiMenu : MonoBehaviour
{
    [SerializeField] private GameObject levelInfoPage;
    [SerializeField] private Button levelInfoButton;
    [SerializeField] private Button levelInfoCloseButton;
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

        levelInfoButton.onClick.AddListener(OpenInfoPage);
        levelInfoCloseButton.onClick.AddListener(CloseInfoPage);
    }

    public void OpenInfoButton()
    {
        if (!levelInfoButton.gameObject.activeInHierarchy)
        {
            levelInfoButton.gameObject.SetActive(true);
        }
    }
    public void CloseInfoButton()
    {
        if (levelInfoButton.gameObject.activeInHierarchy)
        {
            levelInfoButton.gameObject.SetActive(false);
        }
    }

    public void OpenInfoPage()
    {
        if (!levelInfoPage.activeInHierarchy)
        {
            levelInfoPage.SetActive(true);
            CloseInfoButton();
        }
    }
    public void CloseInfoPage()
    {
        if (levelInfoPage.activeInHierarchy)
        {
            levelInfoPage.SetActive(false);
            OpenInfoButton();
        }
    }

    public GameObject GetLevelInfo()
    {
        return levelInfoPage;
    }
}
