using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    [SerializeField] private GameObject playPage;
    [SerializeField] private GameObject storePage;
    [SerializeField] private GameObject upgradePage;
    [SerializeField] private GameObject customGamePage;
    [SerializeField] private GameObject playButton;

    public static Menu Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetCoinText();
    }

    public void SetCoinText()
    {
        int coin = AssignLevel.Instance.GetCoin();
        coinText.text = "Coin:" + coin.ToString();
    }
    public void PlayButton()
    {
        AssignLevel.Instance.customGame = false;
        SceneManager.LoadScene(1);
    }

   
    public void PlayPageOpen()
    {
        playPage.SetActive(true);
        playButton.SetActive(true);
        storePage.SetActive(false);
        upgradePage.SetActive(false);
        customGamePage.SetActive(false);
    }
    public void StorePage()
    {
        storePage.SetActive(true);
        playButton.SetActive(true);
        playPage.SetActive(false);
        upgradePage.SetActive(false);
        customGamePage.SetActive(false);
    }
    public void UpgradePage()
    {
        upgradePage.SetActive(true);
        playButton.SetActive(true);
        playPage.SetActive(false);
        storePage.SetActive(false);
        customGamePage.SetActive(false);
    }

    public void CustomGamePageOpen()
    {
        customGamePage.SetActive(true);
        playButton.SetActive(false);
    }
}
