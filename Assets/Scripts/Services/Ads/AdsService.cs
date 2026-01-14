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
    private bool _initializing;

    private Action _rewardCallback;

    public event Action RewardedFailed;

    public event Action RewardedReady;

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

        if (InternetService.IsOnline)
            InitializeAds();
    }

    private void OnEnable()
    {
        InternetService.OnlineStateChanged += OnInternetChanged;
    }

    private void OnDisable()
    {
        InternetService.OnlineStateChanged -= OnInternetChanged;
    }

    public bool HasInternet()
        => InternetService.IsOnline;

    public bool CanShowRewarded()
        => IsOnlineAndReady(_rewardedReady);

    public bool ShowRewarded(Action onReward)
    {
        if (onReward == null)
            return false;

        if (!CanShowRewarded())
            return false;

        _rewardCallback = onReward;
        Advertisement.Show(_rewardedPlacementId, this);
        return true;
    }

    public void NotifyMatchFinished()
    {
        _matchesSinceLastAd++;

        if (_matchesSinceLastAd < _currentMatchesThreshold)
            return;

        TryShowInterstitial();
    }

    private void OnInternetChanged(bool isOnline)
    {
        if (!isOnline)
            return;

        if (!_initializing && !Advertisement.isInitialized)
        {
            InitializeAds();
            return;
        }

        if (Advertisement.isInitialized)
        {
            if (!_rewardedReady)
                LoadRewarded();

            if (!_interstitialReady)
                LoadInterstitial();
        }
    }

    private void InitializeAds()
    {
        _initializing = true;
        Advertisement.Initialize(_androidGameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        _initializing = false;
        LoadRewarded();
        LoadInterstitial();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        _initializing = false;
        _rewardedReady = false;
        _interstitialReady = false;
    }

    private void LoadRewarded()
    {
        if (!InternetService.IsOnline)
            return;

        _rewardedReady = false;
        Advertisement.Load(_rewardedPlacementId, this);
    }

    private void LoadInterstitial()
    {
        if (!InternetService.IsOnline)
            return;

        _interstitialReady = false;
        Advertisement.Load(_interstitialPlacementId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == _rewardedPlacementId)
        {
            _rewardedReady = true;
            RewardedReady?.Invoke();
        }

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
            ResetMatchCounter();
            LoadInterstitial();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        if (placementId == _rewardedPlacementId)
        {
            _rewardCallback = null;
            RewardedFailed?.Invoke();
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

    private void TryShowInterstitial()
    {
        if (!IsOnlineAndReady(_interstitialReady))
            return;

        Advertisement.Show(_interstitialPlacementId, this);
    }

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