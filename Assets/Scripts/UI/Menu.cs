
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI[] coinText;

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

    private GameObject behindSettingPage;

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
        selectedButton = buttonsInHolder[0];
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

        SetCoinText();
        levelUI.RefreshMap(false);

        EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
    }

    public void SetCoinText()
    {
        if (gameManager == null)
        {
            gameManager = AssignLevel.Instance;
        }
        int currentLevel = gameManager.levelCount + 1;
        int coin = gameManager.GetCoin();

        for(int i = 0 ; i < coinText.Length; i++)
        {
            coinText[i].text = "Coin: " + coin;
        }
        levelText.text = "Level " + currentLevel;
    }

    // ---------------------------------------------------------
    // PAGE SWITCHING (CENTRALIZED)
    // ---------------------------------------------------------

    private void ShowPage(GameObject targetPage)
    {
        playPage.SetActive(false);
        storePage.SetActive(false);
        upgradePage.SetActive(false);
        customGamePage.SetActive(false);
        terUpgradePage.SetActive(false);

        holderTer.SetActive(false);
        playButton.SetActive(true);

        behindSettingPage = targetPage;
        targetPage.SetActive(true);
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
        ShowPage(playPage);
        holderTer.SetActive(true);

        selectedButton = buttonsInHolder[0];
    }

    public void StorePage()
    {
        if (!buttonLock.unlockShop)
        {
            PreventSelection();
            return;
        }

        buttonLock.CloseTutorial();
        ShowPage(storePage);
        selectedButton = buttonsInHolder[3];
    }

    public void UpgradePage()
    {
        if (!buttonLock.unlockTroopUpgrades)
        {
            PreventSelection();
            return;
        }

        buttonLock.CloseTutorial();
        ShowPage(upgradePage);

        selectedButton = buttonsInHolder[1];
    }

    public void TerUpgradePage()
    {
        if (!buttonLock.unlockTerritoryUpgrades)
        {
            PreventSelection();
            return;
        }
        buttonLock.CloseTutorial();
        ShowPage(terUpgradePage);

        selectedButton = buttonsInHolder[2];
    }

    public void CustomGamePageOpen()
    {
        ShowPage(customGamePage);
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
            PlayPageOpen();
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
