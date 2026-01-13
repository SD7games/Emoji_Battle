using System;
using UnityEngine;
using UnityEngine.Advertisements;

public sealed class AdsService :
    MonoBehaviour,
    IUnityAdsInitializationListener,
    IUnityAdsLoadListener,
    IUnityAdsShowListener
{
    public static AdsService I { get; private set; }

    [Header("Unity Ads")]
    [SerializeField] private string _androidGameId = "6020853";

    [SerializeField] private string _rewardedPlacementId = "Rewarded_Android";
    [SerializeField] private string _interstitialPlacementId = "Interstitial_Android";
    [SerializeField] private bool _testMode = false;

    [Header("Auto Ads")]
    [SerializeField] private int _matchesPerAdMin = 4;

    [SerializeField] private int _matchesPerAdMax = 5;

    private int _matchesSinceLastAd;
    private int _currentMatchesThreshold;

    private bool _rewardedReady;
    private bool _interstitialReady;

    private Action _rewardCallback;

    private void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        ResetMatchCounter();
        Advertisement.Initialize(_androidGameId, _testMode, this);
    }

    public void NotifyMatchFinished()
    {
        _matchesSinceLastAd++;

        if (_matchesSinceLastAd < _currentMatchesThreshold)
            return;

        TryShowInterstitial();
    }

    public bool CanShowRewarded()
        => IsOnlineAndReady(_rewardedReady);

    public void ShowRewarded(Action onReward)
    {
        if (onReward == null)
            return;

        if (!CanShowRewarded())
            return;

        _rewardCallback = onReward;
        Advertisement.Show(_rewardedPlacementId, this);
    }

    private void TryShowInterstitial()
    {
        if (!IsOnlineAndReady(_interstitialReady))
            return;

        ResetMatchCounter();
        Advertisement.Show(_interstitialPlacementId, this);
    }

    public void OnInitializationComplete()
    {
        LoadRewarded();
        LoadInterstitial();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        _rewardedReady = false;
        _interstitialReady = false;
    }

    private void LoadRewarded()
    {
        _rewardedReady = false;
        Advertisement.Load(_rewardedPlacementId, this);
    }

    private void LoadInterstitial()
    {
        _interstitialReady = false;
        Advertisement.Load(_interstitialPlacementId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == _rewardedPlacementId)
            _rewardedReady = true;

        if (placementId == _interstitialPlacementId)
            _interstitialReady = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        if (placementId == _rewardedPlacementId)
            _rewardedReady = false;

        if (placementId == _interstitialPlacementId)
            _interstitialReady = false;
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state)
    {
        if (placementId == _rewardedPlacementId)
        {
            if (state == UnityAdsShowCompletionState.COMPLETED)
                _rewardCallback?.Invoke();

            _rewardCallback = null;
            LoadRewarded();
        }

        if (placementId == _interstitialPlacementId)
        {
            LoadInterstitial();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        if (placementId == _rewardedPlacementId)
        {
            _rewardCallback = null;
            LoadRewarded();
        }

        if (placementId == _interstitialPlacementId)
        {
            LoadInterstitial();
        }
    }

    public void OnUnityAdsShowStart(string placementId)
    { }

    public void OnUnityAdsShowClick(string placementId)
    { }

    private bool IsOnlineAndReady(bool ready)
    {
        if (!InternetService.IsOnline)
            return false;

        if (!Advertisement.isInitialized)
            return false;

        if (Advertisement.isShowing)
            return false;

        return ready;
    }

    private void ResetMatchCounter()
    {
        _matchesSinceLastAd = 0;

        int min = Mathf.Min(_matchesPerAdMin, _matchesPerAdMax);
        int max = Mathf.Max(_matchesPerAdMin, _matchesPerAdMax);

        _currentMatchesThreshold = UnityEngine.Random.Range(min, max + 1);
    }
}