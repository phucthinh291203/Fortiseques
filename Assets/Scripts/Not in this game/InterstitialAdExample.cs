using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class InterstitialAdExample : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    string _adUnitId_Interstitial, _adUnitId_Rewarded,_adUnitId_Banner, _gameId;
    [SerializeField] string _iOsAdUnitId_Interstitial = "Interstitial_iOS";
    [SerializeField] string _iOSAdUnitId_Rewarded = "Rewarded_iOS";
    [SerializeField] string _androidAdUnitId_Interstitial = "Interstitial_Android";
    [SerializeField] string _androidAdUnitId_Rewarded = "Rewarded_Android";
    Action<bool> callback_Rewarded, callback_Interstitial;
    public bool isAdsAvailable = false;

    public static InterstitialAdExample instance;
    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
                _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
                _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }

        // Get the Ad Unit ID for the current platform:
        _adUnitId_Interstitial = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOsAdUnitId_Interstitial : _androidAdUnitId_Interstitial;
        _adUnitId_Rewarded = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSAdUnitId_Rewarded : _androidAdUnitId_Rewarded;
    }
    #region Interstitial ( Ads Full screend and have button to close)
    // Load content to the Ad Unit:
    public void LoadAd_Interstitial(Action<bool> callback_Interstitial)
    {
        this.callback_Interstitial = callback_Interstitial;
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId_Interstitial);
        Advertisement.Load(_adUnitId_Interstitial, this);
    }

    // Show the loaded content in the Ad Unit:
    public void ShowAd_Interstitial()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: " + _adUnitId_Interstitial);
        ShowOptions showOptions = new ShowOptions();

        Advertisement.Show(_adUnitId_Interstitial, showOptions, this);
    }
    #endregion

    #region Rewarded ( Seen Ads and have present and if user close => don't have present)

    // Call this public method when you want to get an ad ready to show.
    public void LoadAd_Rewarded(Action<bool> callback_Rewarded)
    {
        this.callback_Rewarded = callback_Rewarded;
        //IMPORTANT! Only load content AFTER initialization(in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId_Rewarded);
        Advertisement.Load(_adUnitId_Rewarded, this);
    }
    // Implement a method to execute when the user clicks the button:
    public void ShowAd_Rewarded()
    {
        //Disable the button:
        isAdsAvailable = false;
        //Then show the ad:
        Advertisement.Show(_adUnitId_Rewarded, this);
    }
    #endregion
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId_Rewarded))
        {
            // Configure the button to call the ShowAd() method when clicked:
            ShowAd_Rewarded();
            // Enable the button for users to click:
            isAdsAvailable = true;
        }

        if (adUnitId.Equals(_adUnitId_Interstitial))
        {
            // Configure the button to call the ShowAd() method when clicked:
            ShowAd_Interstitial();
            // Enable the button for users to click:
            isAdsAvailable = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        callback_Rewarded?.Invoke(false);
        callback_Rewarded = null;

        callback_Interstitial?.Invoke(false);
        callback_Interstitial = null;
    }
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        callback_Rewarded?.Invoke(false);
        callback_Rewarded = null;

        callback_Interstitial?.Invoke(false);
        callback_Interstitial = null;
    }

    public void OnUnityAdsShowStart(string placementId)
    {

    }

    public void OnUnityAdsShowClick(string placementId)
    {

    }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(_adUnitId_Rewarded))
        {
            Debug.LogError("adUnitId_Rewarded");
            callback_Rewarded(true);
            callback_Rewarded = null;
        }
        if (placementId.Equals(_adUnitId_Interstitial))
        {
            Debug.LogError("adUnitId_Interstitial");
            callback_Interstitial(true);
            callback_Interstitial = null;
        }
    }

    #region banner

    [SerializeField] Button _loadBannerButton;
    [SerializeField] Button _showBannerButton;
    [SerializeField] Button _hideBannerButton;
    public void LoadBanner()
    {
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitId_Banner, options);
    }

    // Implement code to execute when the loadCallback event triggers:
    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");

        // Configure the Show Banner button to call the ShowBannerAd() method when clicked:
        _showBannerButton.onClick.AddListener(ShowBannerAd);
        // Configure the Hide Banner button to call the HideBannerAd() method when clicked:
        _hideBannerButton.onClick.AddListener(HideBannerAd);

        // Enable both buttons:
        _showBannerButton.interactable = true;
        _hideBannerButton.interactable = true;
    }

    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }

    void ShowBannerAd()
    {
        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        // Show the loaded Banner Ad Unit:
        Advertisement.Banner.Show(_adUnitId_Banner, options);
    }

    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }
    // Implement a method to call when the Hide Banner button is clicked:
    void HideBannerAd()
    {
        // Hide the banner:
        Advertisement.Banner.Hide();
    }
    #endregion
}