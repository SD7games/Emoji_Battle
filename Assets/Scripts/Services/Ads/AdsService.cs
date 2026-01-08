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
    [SerializeField] private bool _testMode = true;

    [Header("Auto Ads")]
    [SerializeField] private int _matchesPerAdMin = 2;

    [SerializeField] private int _matchesPerAdMax = 3;

    private int _matchesSinceLastAd;
    private int _currentMatchesThreshold;

    private bool _rewardedReady;
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

        TryShowAutoAd();
    }

    public bool CanShowRewardedForLootbox()
    {
        return IsRewardedReady();
    }

    public void ShowRewardedForLootbox(Action onReward)
    {
        if (!IsRewardedReady())
            return;

        _rewardCallback = onReward;
        Advertisement.Show(_rewardedPlacementId, this);
    }

    private void TryShowAutoAd()
    {
        if (!IsRewardedReady())
        {
            ResetMatchCounter();
            return;
        }

        _rewardCallback = null;
        ResetMatchCounter();

        Advertisement.Show(_rewardedPlacementId, this);
    }

    private bool IsRewardedReady()
    {
        if (!InternetService.IsOnline)
            return false;

        if (!Advertisement.isInitialized)
            return false;

        return _rewardedReady;
    }

    private void ResetMatchCounter()
    {
        _matchesSinceLastAd = 0;
        _currentMatchesThreshold = UnityEngine.Random.Range(
            _matchesPerAdMin,
            _matchesPerAdMax + 1
        );
    }

    public void OnInitializationComplete()
    {
        LoadRewarded();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        _rewardedReady = false;
    }

    private void LoadRewarded()
    {
        _rewardedReady = false;
        Advertisement.Load(_rewardedPlacementId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId == _rewardedPlacementId)
            _rewardedReady = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        _rewardedReady = false;
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state)
    {
        if (placementId != _rewardedPlacementId)
            return;

        if (state == UnityAdsShowCompletionState.COMPLETED)
            _rewardCallback?.Invoke();

        _rewardCallback = null;
        LoadRewarded();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        _rewardCallback = null;
        LoadRewarded();
    }

    public void OnUnityAdsShowStart(string placementId)
    { }

    public void OnUnityAdsShowClick(string placementId)
    { }
}