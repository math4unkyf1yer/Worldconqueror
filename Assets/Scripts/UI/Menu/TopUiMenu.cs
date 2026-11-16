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

        levelInfoButton.onClick.AddListener(PageClick);
        levelInfoCloseButton.onClick.AddListener(ButtonClick);
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

    public void CloseBoth()
    {
        CloseInfoButton();
        CloseInfoPage(false);
    }
    public void OpenBoth()
    {
        OpenInfoPage(false);
        OpenInfoButton();
    }

    public void OpenInfoPage(bool keepButton)
    {
        if (!levelInfoPage.activeInHierarchy)
        {
            levelInfoPage.SetActive(true);
            if(keepButton)
                CloseInfoButton();
        }
    }
    public void CloseInfoPage(bool keepButton)
    {
        if (levelInfoPage.activeInHierarchy)
        {
            levelInfoPage.SetActive(false);
            if (keepButton)
                OpenInfoButton();
        }
    }

    void ButtonClick()
    {
        CloseInfoPage(true);
    } 
    void PageClick()
    {
        OpenInfoPage(true);
    }
    public GameObject GetLevelInfo()
    {
        return levelInfoPage;
    }
}
