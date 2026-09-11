
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI diamondText;

    [SerializeField] private GameObject playPage;
    [SerializeField] private GameObject storePage;
    [SerializeField] private GameObject upgradePage;
    [SerializeField] private GameObject customGamePage;
    [SerializeField] private GameObject terUpgradePage;
    [SerializeField] private GameObject settingPage;
    [SerializeField] private GameObject playButton;
    [SerializeField] private TextMeshProUGUI levelText; 
    [SerializeField] private LevelUI levelUI;
    [SerializeField] private GameObject holderTer;
    [SerializeField] private GameObject terCamera;
    private Vector3 terCameraPos;

    private GameObject behindSettingPage;
    GameObject currentPage;

    [SerializeField] private Cost costScript;
    [SerializeField] private Cost costTerScript;

    private ButtonLockController buttonLock;
    [SerializeField] private CustomLevelConfig customLevelConfig;
    //for button look
    [SerializeField] private GameObject buttonHolder;
    private List<Button> buttonsInHolder = new List<Button>();
    //selected Button
    private Button selectedButton;
    //game manager 
    AssignLevel gameManager;

    int oldButtonId;
    public static Menu Instance { get; private set; }


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

    private void Start()
    {
        buttonLock = GetComponent<ButtonLockController>();
        CacheButtons();
        terCameraPos = terCamera.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }

    // ---------------------------------------------------------
    // BUTTON INITIALIZATION
    // ---------------------------------------------------------

    private void CacheButtons()
    {
        buttonsInHolder.Clear();

        foreach (Button child in buttonHolder.GetComponentsInChildren<Button>())
            buttonsInHolder.Add(child);

        // Select first button by default
        selectedButton = buttonsInHolder[1];
        EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
    }

    // ---------------------------------------------------------
    // GENERAL SETUP
    // ---------------------------------------------------------

    public void SetUp()
    {
        buttonLock.CheckAllButtons(gameManager.levelCount);
    }

    public void BackToMenu()
    {
        customLevelConfig.CheckIfUnlocked();
        buttonLock.CheckAllButtons(gameManager.levelCount);

        gameObject.SetActive(true);
        holderTer.SetActive(true);

        SetText();
        levelUI.RefreshMap(false);

        EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
    }

    public void SetText()
    {
        if (gameManager == null)
        {
            gameManager = AssignLevel.Instance;
        }
        int currentLevel = gameManager.levelCount + 1;
        levelText.text = "Level " + currentLevel;
        SetCoinTexts();
        SetDiamondText();
    }
    void SetCoinTexts()
    {
        int coin = gameManager.GetCoin();

        coinText.text = " " + coin;
    }
     void SetDiamondText()
    {
        int diam = gameManager.GetDiamond();

        diamondText.text = " " + diam;
    }

    // ---------------------------------------------------------
    // PAGE SWITCHING (CENTRALIZED)
    // ---------------------------------------------------------

    //for the setting and the custom page(we don't want them to transition)
    private void ShowPage(GameObject targetPage)
    {
        playPage.SetActive(false);
        holderTer.SetActive(false);
        targetPage.SetActive(true);
    }
    private void HidePage(GameObject targetPage)
    {
        targetPage.SetActive(false);
        playPage.SetActive(true);
        holderTer.SetActive(true);  
    }
    

    //Only for pages that will transition
    private void ShowPageSlide(GameObject targetPage, int targetButtonIndex)
    {
        if(targetPage == behindSettingPage)
        {
            return;
        }
        currentPage = null;

        if (playPage.activeInHierarchy) currentPage = playPage;
        else if (storePage.activeInHierarchy) currentPage = storePage;
        else if (upgradePage.activeInHierarchy) currentPage = upgradePage;
        else if (customGamePage.activeInHierarchy) currentPage = customGamePage;
        else if (terUpgradePage.activeInHierarchy) currentPage = terUpgradePage;

        behindSettingPage = targetPage;


        // First time opening menu
        if (currentPage == null)
        {
            targetPage.SetActive(true);
            return;
        }

        int currentIndex = buttonsInHolder.IndexOf(selectedButton);
        bool slideRight = targetButtonIndex > currentIndex;

        StartCoroutine(SlideTransition(currentPage, targetPage, slideRight, 0.25f));

        selectedButton = buttonsInHolder[targetButtonIndex];
        EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
    }

    private IEnumerator SlideTransition(GameObject fromPage, GameObject toPage, bool slideRight, float duration)
    {
        RectTransform fromRect = fromPage.GetComponent<RectTransform>();
        RectTransform toRect = toPage.GetComponent<RectTransform>();

        float screenWidth = Screen.width;

        Vector2 fromStart = Vector2.zero;
        Vector2 fromEnd = slideRight ? new Vector2(-screenWidth, 0) : new Vector2(screenWidth, 0);

        Vector2 toStart = slideRight ? new Vector2(screenWidth, 0) : new Vector2(-screenWidth, 0);
        Vector2 toEnd = Vector2.zero;

        toPage.SetActive(true);
        toRect.anchoredPosition = toStart;

        //move the territories sideways
        Transform holderTransform = terCamera.transform;

        Vector3 holderEnd = terCameraPos + (slideRight ? Vector3.right : Vector3.left) * screenWidth * 0.01f;
        Vector3 holderEndPlay = terCameraPos + (slideRight ? Vector3.left : Vector3.right) * screenWidth * 0.01f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            fromRect.anchoredPosition = Vector2.Lerp(fromStart, fromEnd, normalized);
            toRect.anchoredPosition = Vector2.Lerp(toStart, toEnd, normalized);

            if (fromPage == playPage)
            {
                holderTransform.position = Vector3.Lerp(terCameraPos, holderEnd, normalized);
            }

            if (toPage == playPage)
            {
                holderTransform.position = Vector3.Lerp(holderEndPlay, terCameraPos, normalized);
            }

            yield return null;
        }

        fromPage.SetActive(false);
        if (toPage == playPage)
            holderTransform.position = terCameraPos;
        else
            holderTransform.position = holderEnd;
        toRect.anchoredPosition = Vector2.zero;
    }

    private void PreventSelection()
    {
        EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
    }

    // ---------------------------------------------------------
    // PAGE ACTIONS
    // ---------------------------------------------------------

    public void PlayButton()
    {
        //wait for sound to play 
        StartCoroutine(DelayedPlay(gameManager.audioManager.ButtonSound.length));
    }

    IEnumerator DelayedPlay(float delay)
    {
        yield return new WaitForSeconds(delay);
        holderTer.SetActive(false);
        gameManager.customGame = false;

        gameObject.SetActive(false);
        LoadScreen.Instance.LoadScene(1);
    }

    public void PlayPageOpen()
    {
        ShowPageSlide(playPage,1);
        holderTer.SetActive(true);

        selectedButton = buttonsInHolder[1];
    }

    public void StorePage()
    {
        if (!buttonLock.unlockShop)
        {
            PreventSelection();
            return;
        }

        buttonLock.CloseTutorial();
        ShowPageSlide(storePage,0);
        selectedButton = buttonsInHolder[0];
    }

    public void UpgradePage()
    {
        if (!buttonLock.unlockTroopUpgrades)
        {
            PreventSelection();
            return;
        }

        buttonLock.CloseTutorial();
        ShowPageSlide(upgradePage,2);

        selectedButton = buttonsInHolder[2];
    }

    public void TerUpgradePage()
    {
        if (!buttonLock.unlockTerritoryUpgrades)
        {
            PreventSelection();
            return;
        }
        buttonLock.CloseTutorial();
        ShowPageSlide(terUpgradePage, 3);

        selectedButton = buttonsInHolder[3];
    }

    public void CustomGamePageOpen()
    {
        ShowPage(customGamePage);
    }
    public void CustomGameClose()
    {
        HidePage(customGamePage);
    }

    public void SettingPage()
    {
        holderTer.SetActive(false);
        settingPage.SetActive(true);
    }
    public void CloseSettingPage()
    {
        settingPage.SetActive(false);
        if (playPage.activeInHierarchy)
        {
            HidePage(settingPage);
        }
        else
        {
            ShowPage(behindSettingPage);
        }
    }

    // ---------------------------------------------------------
    // QUIT
    // ---------------------------------------------------------

    private void QuitGame()
    {
        Application.Quit();
    }

}
