using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
public class AdsManager : MonoBehaviour , IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string androidGameId = "6191215";
    [SerializeField] string androidAdUnitID = "Interstitial_Android";

    string adUnitId;

    private System.Action _onFinish;

    void Start()
    {
        adUnitId = androidAdUnitID;
#if UNITY_ANDROID
        adUnitId = androidAdUnitID;
#endif

        Advertisement.Initialize(androidGameId, true,this);
    }

    // Called when Unity Ads has finished initializing
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialized successfully.");

        LoadAd();
    }

    // Called if Unity Ads initialization fails
    public void OnInitializationFailed(
        UnityAdsInitializationError error,
        string message)
    {
        Debug.LogError(
            $"Unity Ads initialization failed: {error} - {message}");
    }

    // Load an ad
    public void LoadAd()
    {
        Debug.Log("Loading ad...");

        Advertisement.Load(adUnitId, this);
    }

    // Called when the ad successfully loads
    public void OnUnityAdsAdLoaded(string loadedAdUnitId)
    {
        Debug.Log("Ad loaded: " + loadedAdUnitId);

    }

    // Called if the ad fails to load
    public void OnUnityAdsFailedToLoad(
        string failedAdUnitId,
        UnityAdsLoadError error,
        string message)
    {
        Debug.LogError(
            $"Failed to load ad: {failedAdUnitId} - {error} - {message}");
    }

    // Show an ad
    public void ShowAd(System.Action onFinish)
    {
        _onFinish = onFinish;

        Debug.Log("Showing ad...");

        Advertisement.Show(adUnitId, this);
    }

    // Called when the ad fails to show
    public void OnUnityAdsShowFailure(
        string failedAdUnitId,
        UnityAdsShowError error,
        string message)
    {
        Debug.LogError(
            $"Failed to show ad: {failedAdUnitId} - {error} - {message}");

        _onFinish?.Invoke();
        _onFinish = null;

        LoadAd();
    }

    // Called when the ad starts
    public void OnUnityAdsShowStart(string startedAdUnitId)
    {
        Debug.Log("Ad started: " + startedAdUnitId);
    }

    // Called when the player clicks the ad
    public void OnUnityAdsShowClick(string clickedAdUnitId)
    {
        Debug.Log("Ad clicked: " + clickedAdUnitId);
    }

    // Called when the ad finishes
    public void OnUnityAdsShowComplete(
        string completedAdUnitId,
        UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Ad completed: " + completedAdUnitId);

        _onFinish?.Invoke();
        _onFinish = null;

        // Load the next ad
        LoadAd();
    }

}
