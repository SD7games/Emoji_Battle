using System;
using UnityEngine;

public sealed class SettingsService
{
    private static SettingsService _instance;
    public static SettingsService I => _instance ??= new SettingsService();

    public static event Action<string> PlayerNameChanged;

    private SettingsService()
    { }

    public SettingsData Data => GameDataService.I.Data.Settings;

    public void SetMusicEnabled(bool enabled)
    {
        Data.MusicEnabled = enabled;
        ApplyMusic();
        GameDataService.I.Save();
    }

    public void SetMusicVolume(float value)
    {
        Data.MusicVolume = value;
        ApplyMusic();
        GameDataService.I.Save();
    }

    public void SetSfxEnabled(bool enabled)
    {
        Data.SfxEnabled = enabled;
        ApplySfx();
        GameDataService.I.Save();
    }

    public void SetSfxVolume(float value)
    {
        Data.SfxVolume = value;
        ApplySfx();
        GameDataService.I.Save();
    }

    public void SetVibration(bool enabled)
    {
        Data.VibrationEnabled = enabled;
        GameDataService.I.Save();
    }

    public void SetPlayerName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = "Player";

        name = name.Trim();

        if (name.Length > 8)
            name = name.Substring(0, 8);

        GameDataService.I.Data.Player.Name = name;
        GameDataService.I.Save();

        PlayerNameChanged?.Invoke(name);
    }

    private void ApplyMusic()
    {
        AudioListener.volume = Data.MusicEnabled ? Data.MusicVolume : 0f;
    }

    private void ApplySfx()
    {
        // TODO: AudioService
    }
}