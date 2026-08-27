using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public Dictionary<string, bool> languages = new Dictionary<string, bool>();
    public List<string> languageOrder = new List<string>();

    [SerializeField] TextMeshProUGUI languageChangerTm;
    [SerializeField] GameObject ChangeLnButton;

    private int currentIndex;
    string chosen;

    public void Start()
    {
        languages["English"] = true;
        languages["Français"] = false;
        languages["Español"] = false;

        languageOrder.Add("English");
        languageOrder.Add("Français");
        languageOrder.Add("Español");
    }

    public void LeftArrow()
    {
        currentIndex--;

        if(currentIndex < 0) { currentIndex = languageOrder.Count - 1; }

        ChangeTextUI();
    }
    public void RightArrow()
    {
        currentIndex++;

        if(currentIndex >= languageOrder.Count) { currentIndex = 0; }

        ChangeTextUI();
    }
    void ChangeTextUI()
    {
        chosen = languageOrder[currentIndex]; 
        languageChangerTm.text = chosen;

        //once press button change all of them 
        if (languages.ContainsKey(chosen) && languages[chosen] == true)
        {
            ChangeLnButton.SetActive(false);   // already selected → hide
        }
        else
        {
            ChangeLnButton.SetActive(true);    // different → show
        }
    }

    public void AgreeToChangeText()
    {
        List<string> keys = new List<string>(languages.Keys);

        foreach (string key in keys)
            languages[key] = false;

        languages[chosen] = true;

        ChangeLnButton.SetActive(false);

        //Just need to change all text possible to the language chosen that are active -- for each text that appears will need to check their language and change on spawn 
    }
}
