using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LobbyView : MonoBehaviour
{
    [SerializeField] private EmojiScrollView _emojiView;
    [SerializeField] private ColorSwitcherView _colorSwitcher;
    [SerializeField] private DissolveLobby _playerDissolve;
    [SerializeField] private DissolveLobby _aiDissolve;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _aiNameText;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _adsButton;
    [SerializeField] private Button _comlexityInfoButton;

    private LobbyController _controller;

    private Sprite _lastPlayerSprite;
    private Sprite _lastAISprite;

    public void Construct(LobbyController controller)
    {
        _controller = controller;

        _emojiView.OnEmojiClicked += _controller.OnEmojiSelected;
        _colorSwitcher.OnColorSelected += _controller.OnColorChanged;
        _startButton.onClick.AddListener(OnStartButton);
        _settingsButton.onClick.AddListener(OnSettingsButton);
        _adsButton.onClick.AddListener(OnAdsButton);
        _comlexityInfoButton.onClick.AddListener(OnComplexityInfoButton);

        _controller.PlayerAvatarChanged += UpdatePlayerAvatar;
        _controller.AIAvatarChanged += UpdateAIAvatar;
        _controller.EmojiListChanged += UpdateEmojiList;
        _controller.PlayerNameChanged += UpdatePlayerName;
        _controller.AINameChanged += UpdateAIName;
        _controller.RewardedAvailabilityChanged += SetAdsButtonState;
    }

    private void OnDestroy()
    {
        if (_controller == null) return;

        _emojiView.OnEmojiClicked -= _controller.OnEmojiSelected;
        _colorSwitcher.OnColorSelected -= _controller.OnColorChanged;
        _startButton.onClick.RemoveListener(OnStartButton);
        _settingsButton.onClick.RemoveListener(OnSettingsButton);
        _adsButton.onClick.RemoveListener(OnAdsButton);
        _comlexityInfoButton.onClick.RemoveListener(OnComplexityInfoButton);

        _controller.PlayerAvatarChanged -= UpdatePlayerAvatar;
        _controller.AIAvatarChanged -= UpdateAIAvatar;
        _controller.EmojiListChanged -= UpdateEmojiList;
        _controller.PlayerNameChanged -= UpdatePlayerName;
        _controller.AINameChanged -= UpdateAIName;
        _controller.RewardedAvailabilityChanged -= SetAdsButtonState;

    }

    public void ForceSetPlayerAvatar(Sprite sprite)
    {
        _lastPlayerSprite = sprite;
        _playerDissolve.SetSprite(sprite);
        _playerDissolve.PlayForOwner(AvatarOwner.Player);
    }

    private void OnComplexityInfoButton()
    {
        PopupService.I.Show(PopupId.ComplexityInfo);
    }

    private void SetAdsButtonState(bool available)
    {
        _adsButton.interactable = available;
    }

    private void OnSettingsButton()
    {
        PopupService.I.Show(PopupId.Settings);
    }

    private void OnAdsButton()
    {
        _controller.OnAdsPressed();
    }

    private void UpdatePlayerName(string name)
    {
        _playerNameText.text = name;
    }

    private void UpdateAIName(string value)
    {
        _aiNameText.text = value;
    }

    private void UpdateEmojiList(List<EmojiProgress> items)
    {
        _emojiView.Fill(items, _controller.Resolver);
    }

    private void UpdatePlayerAvatar(Sprite sprite)
    {
        if (sprite == null) return;

        bool changed = _lastPlayerSprite != sprite;
        _lastPlayerSprite = sprite;

        _playerDissolve.SetSprite(sprite);

        if (changed)
            _playerDissolve.PlayForOwner(AvatarOwner.Player);
    }

    private void UpdateAIAvatar(Sprite sprite)
    {
        if (sprite == null) return;

        bool changed = _lastAISprite != sprite;
        _lastAISprite = sprite;

        _aiDissolve.SetSprite(sprite);

        if (changed)
            _aiDissolve.PlayForOwner(AvatarOwner.AI);
    }

    private void OnStartButton()
    {
        _controller.OnStartPressed();
    }
}