using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class LobbyController : IDisposable
{
    private readonly LobbyService _service;
    private int _currentColor;

    public event Action<Sprite> PlayerAvatarChanged;

    public event Action<Sprite> AIAvatarChanged;

    public event Action<EmojiViewData[]> EmojiListChanged;

    public event Action<string> PlayerNameChanged;

    public event Action<string> AINameChanged;

    public LobbyController(LobbyService service)
    {
        _service = service;
        SettingsService.PlayerNameChanged += OnPlayerNameChanged;
    }

    public void Initialize()
    {
        PlayerNameChanged?.Invoke(GetPlayerName());
    }

    public void Dispose()
    {
        SettingsService.PlayerNameChanged -= OnPlayerNameChanged;
    }

    public void OnAIStrategyChanged(AIStrategyType type)
    {
        var data = GameDataService.I.Data;
        data.AI.Strategy = type;
        GameDataService.I.Save();

        string newName = _service.GenerateAIName();
        AINameChanged?.Invoke(newName);
    }

    public void SetInitialColor(int colorId)
    {
        _currentColor = colorId;
        UpdateEmojiList();

        var ai = _service.EnsureValidAIEmoji();
        if (ai != null)
            AIAvatarChanged?.Invoke(ai);

        string aiName = _service.GetAIName();
        if (string.IsNullOrEmpty(aiName))
            aiName = _service.GenerateAIName();

        AINameChanged?.Invoke(aiName);
    }

    public void OnColorChanged(int colorId)
    {
        _currentColor = colorId;
        UpdateEmojiList();
    }

    public void OnEmojiSelected(int index)
    {
        var player = _service.SelectPlayerEmoji(_currentColor, index);
        if (player != null)
            PlayerAvatarChanged?.Invoke(player);

        var ai = _service.EnsureValidAIEmoji();
        if (ai != null)
            AIAvatarChanged?.Invoke(ai);
    }

    public void OnStartPressed()
    {
        SceneManager.LoadScene("Main");
    }

    private void UpdateEmojiList()
    {
        var items = _service.GetEmojiItems(_currentColor);
        EmojiListChanged?.Invoke(items);
    }

    private void OnPlayerNameChanged(string name)
    {
        PlayerNameChanged?.Invoke(name);
    }

    private string GetPlayerName()
    {
        return GameDataService.I.Data.Player.Name;
    }
}