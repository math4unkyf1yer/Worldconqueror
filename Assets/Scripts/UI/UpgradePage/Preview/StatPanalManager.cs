using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPanalManager : MonoBehaviour
{
    public static StatPanalManager Instance;

    [SerializeField] private GameObject statObject;
    [SerializeField] private PreviewManager previewManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Show()
    {
        statObject.SetActive(true);
    }

    public void Hide()
    {
        statObject.SetActive(false);
        previewManager.CloseAll();
    }

    public PreviewManager Preview => previewManager;
}
