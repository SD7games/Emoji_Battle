using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class SettingsPopup : PopupBase
{
    public override PopupId Id => PopupId.Settings;

    [Header("Music")]
    [SerializeField] private Button _musicButton;

    [SerializeField] private Image _musicOffIcon;
    [SerializeField] private Slider _musicSlider;

    [Header("SFX")]
    [SerializeField] private Button _sfxButton;

    [SerializeField] private Image _sfxOffIcon;
    [SerializeField] private Slider _sfxSlider;

    [Header("Vibration")]
    [SerializeField] private Button _vibrationButton;

    [SerializeField] private Image _vibrationOffIcon;

    [Header("Player")]
    [SerializeField] private TMP_InputField _playerNameInput;

    [Header("Buttons")]
    [SerializeField] private Button _closeButton;

    private SettingsService _settings;
    private bool _bound;

    private void Awake()
    {
        _settings = SettingsService.I;

        _closeButton.onClick.AddListener(() => PopupService.I.HideCurrent());
    }

    public override void Show()
    {
        base.Show();
        Refresh();
        Bind();
    }

    public override void Hide()
    {
        Unbind();
        base.Hide();
    }

    private void Bind()
    {
        if (_bound)
            return;

        _bound = true;

        _musicButton.onClick.AddListener(MusicChanger);
        _musicSlider.onValueChanged.AddListener(_settings.SetMusicVolume);

        _sfxButton.onClick.AddListener(SfxChanger);
        _sfxSlider.onValueChanged.AddListener(_settings.SetSfxVolume);

        _vibrationButton.onClick.AddListener(VibrationChanger);
        _playerNameInput.onEndEdit.AddListener(_settings.SetPlayerName);
    }

    private void Unbind()
    {
        if (!_bound)
            return;

        _bound = false;

        _musicButton.onClick.RemoveListener(MusicChanger);
        _musicSlider.onValueChanged.RemoveListener(_settings.SetMusicVolume);

        _sfxButton.onClick.RemoveListener(SfxChanger);
        _sfxSlider.onValueChanged.RemoveListener(_settings.SetSfxVolume);

        _vibrationButton.onClick.RemoveListener(VibrationChanger);
        _playerNameInput.onEndEdit.RemoveListener(_settings.SetPlayerName);
    }

    private void MusicChanger()
    {
        _settings.SetMusicEnabled(!_settings.Data.MusicEnabled);
        RefreshVisuals();
    }

    private void SfxChanger()
    {
        _settings.SetSfxEnabled(!_settings.Data.SfxEnabled);
        RefreshVisuals();
    }

    private void VibrationChanger()
    {
        _settings.SetVibration(!_settings.Data.VibrationEnabled);
        RefreshVisuals();
    }

    private void Refresh()
    {
        var data = _settings.Data;
        var player = GameDataService.I.Data.Player;

        _musicSlider.SetValueWithoutNotify(data.MusicVolume);
        _sfxSlider.SetValueWithoutNotify(data.SfxVolume);

        _playerNameInput.SetTextWithoutNotify(player.Name);

        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        var data = _settings.Data;

        _musicOffIcon.enabled = !data.MusicEnabled;
        _musicSlider.interactable = data.MusicEnabled;

        _sfxOffIcon.enabled = !data.SfxEnabled;
        _sfxSlider.interactable = data.SfxEnabled;

        _vibrationOffIcon.enabled = !data.VibrationEnabled;
    }
}