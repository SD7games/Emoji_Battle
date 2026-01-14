using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class LobbyController : IDisposable
{
    private readonly LobbyService _service;
    private readonly SoundDefinition _emojiSelectSound;
    private readonly SoundDefinition _swapSound;
    private readonly GameRewardService _rewards;

    private bool _rewardedInProgress;
    private bool _rewardedAvailable;
    private bool _noInternetPopupShown;

    private int _uiColorId = -1;

    private int _selectedEmojiId = -1;
    private int _selectedEmojiColorId = -1;

    public EmojiResolver Resolver { get; }

    public event Action<Sprite> PlayerAvatarChanged;

    public event Action<Sprite> AIAvatarChanged;

    public event Action<List<EmojiProgress>> EmojiListChanged;

    public event Action<string> PlayerNameChanged;

    public event Action<string> AINameChanged;

    public event Action<bool> RewardedAvailabilityChanged;

    public LobbyController(
        LobbyService service,
        EmojiResolver resolver,
        GameRewardService rewards,
        SoundDefinition emojiSelectSound,
        SoundDefinition colorChangeSound)
    {
        _service = service;
        Resolver = resolver;
        _rewards = rewards;
        _emojiSelectSound = emojiSelectSound;
        _swapSound = colorChangeSound;

        SettingsService.PlayerNameChanged += OnPlayerNameChanged;
    }

    public void Initialize()
    {
        if (AdsService.I != null)
        {
            AdsService.I.RewardedReady += OnRewardedReady;
            AdsService.I.RewardedFailed += OnRewardedFailed;
        }

        var player = GameDataService.I.Data.Player;

        _selectedEmojiId = player.EmojiIndex;
        _selectedEmojiColorId = player.EmojiColor;

        PlayerNameChanged?.Invoke(player.Name);
    }

    public void Dispose()
    {
        SettingsService.PlayerNameChanged -= OnPlayerNameChanged;

        if (AdsService.I != null)
        {
            AdsService.I.RewardedReady -= OnRewardedReady;
            AdsService.I.RewardedFailed -= OnRewardedFailed;
        }
    }

    public void OnAdsPressed()
    {
        if (_rewardedInProgress)
            return;

        if (!_rewardedAvailable)
        {
            if (!_noInternetPopupShown)
            {
                _noInternetPopupShown = true;
                PopupService.I.Show(PopupId.NoInternet);
            }

            return;
        }

        if (AdsService.I == null)
            return;

        if (!AdsService.I.HasInternet())
        {
            PopupService.I.Show(PopupId.NoInternet);
            return;
        }

        if (!AdsService.I.CanShowRewarded())
            return;

        _rewardedInProgress = true;

        if (!AdsService.I.ShowRewarded(OnRewarded))
        {
            _rewardedInProgress = false;
        }
    }

    public void SetInitialColor(int colorId)
    {
        _uiColorId = colorId;
        UpdateEmojiList();

        var ai = _service.EnsureValidAIEmoji();
        if (ai != null)
            AIAvatarChanged?.Invoke(ai);

        AINameChanged?.Invoke(
            string.IsNullOrEmpty(_service.GetAIName())
                ? _service.GenerateAIName()
                : _service.GetAIName());
    }

    public void OnColorChanged(int colorId)
    {
        if (_uiColorId == colorId)
            return;

        _uiColorId = colorId;
        UpdateEmojiList();
        PlayColorChangeSound();
    }

    public void OnEmojiSelected(int emojiId)
    {
        if (_selectedEmojiId == emojiId &&
            _selectedEmojiColorId == _uiColorId)
            return;

        _selectedEmojiId = emojiId;
        _selectedEmojiColorId = _uiColorId;

        var player = _service.SelectPlayerEmoji(_uiColorId, emojiId);
        if (player != null)
        {
            PlayerAvatarChanged?.Invoke(player);
            PlayEmojiSelectSound();
        }

        var ai = _service.EnsureValidAIEmoji();
        if (ai != null)
            AIAvatarChanged?.Invoke(ai);
    }

    private void OnRewardedReady()
    {
        _rewardedAvailable = true;
        RewardedAvailabilityChanged?.Invoke(true);
    }

    private void OnRewardedFailed()
    {
        _rewardedAvailable = false;
        _rewardedInProgress = false;
        RewardedAvailabilityChanged?.Invoke(false);
        _noInternetPopupShown = false;
    }

    private void OnRewarded()
    {
        _rewardedInProgress = false;

        var result = _rewards.RewardedOpened();
        UpdateEmojiList();

        PopupService.I.Show
        (
            result.EmojiUnlocked ? PopupId.Reward : PopupId.Complete
        );
    }

    private void PlayEmojiSelectSound()
    {
        if (_emojiSelectSound != null)
            AudioService.I.PlaySFX(_emojiSelectSound);
    }

    private void PlayColorChangeSound()
    {
        if (_swapSound != null)
            AudioService.I.PlaySFX(_swapSound);
    }

    private void UpdateEmojiList()
    {
        if (_uiColorId < 0)
            return;

        var progress = GameDataService.I.Data.Progress;
        var data = Resolver.GetData(_uiColorId);

        if (data == null)
            return;

        EmojiListChanged?.Invoke(
            progress.GetSortedForView(
                _uiColorId,
                data.EmojiSprites.Count));
    }

    public void OnAIStrategyChanged(AIStrategyType type)
    {
        var data = GameDataService.I.Data;
        data.AI.Strategy = type;
        GameDataService.I.Save();

        AINameChanged?.Invoke(_service.GenerateAIName());
    }

    public void OnStartPressed()
    {
        PlayColorChangeSound();
        SceneManager.LoadScene("Main");
    }

    private void OnPlayerNameChanged(string name)
    {
        PlayerNameChanged?.Invoke(name);
    }
}