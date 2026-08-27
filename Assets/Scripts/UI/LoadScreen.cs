using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadScreen : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Image loadingBarFill;
    public Slider loadingBarSlider;

    [SerializeField] PopulateTerritory originalMenuTerritory;

    public static LoadScreen Instance { get; private set; }

    private void Awake()
    {
        // 1. If an instance already exists and it isn't this one, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        originalMenuTerritory = FindObjectOfType<PopulateTerritory>();
        loadingBarSlider.minValue = 0;
        loadingBarSlider.maxValue = 2;

    }
    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAssync(sceneId));
    }

    IEnumerator LoadSceneAssync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            yield return null;
        }
        loadingBarSlider.value += 1;
        PopulateTerritory territory = FindObjectOfType<PopulateTerritory>();
        if (territory != null && sceneId != 0)
        {
            while (!territory.IsReady)
                yield return null;

            loadingBarSlider.value += 1;
            yield return new WaitForSeconds(0.1f);
            LoadingScreen.SetActive(false);
            loadingBarSlider.value = 0;
        }
        else
        {
            Menu.Instance.BackToMenu();
            while (!originalMenuTerritory.IsReady)
                yield return null;

            loadingBarSlider.value += 1;
            yield return new WaitForSeconds(0.1f);
            LoadingScreen.SetActive(false);
            loadingBarSlider.value = 0;
            //uniy adds plays

        }
    }
}
