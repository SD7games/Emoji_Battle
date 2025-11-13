using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerLobbyController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image _playerSign;
    [SerializeField] private ContentScrollController _contentScroll;
    [SerializeField] private EmojiDataSetter _emojiSetter;

    private EmojiData _currentEmojiData;

    public event Action OnCheckMatchAISign;

    private void Start()
    {
        LoadFromGD();
    }

    private void LoadFromGD()
    {
        string savedColor = GD.Player.EmojiColor;
        int savedIndex = GD.Player.EmojiIndex;

        _currentEmojiData = _emojiSetter.AllData.Find(d => d.ColorName == savedColor);

        if (_currentEmojiData == null && _emojiSetter.AllData.Count > 0)
        {
            _currentEmojiData = _emojiSetter.AllData[0];
            savedIndex = 0;
        }

        _emojiSetter.SetEmojiData(_currentEmojiData);

        _contentScroll.SetEmojiData(_currentEmojiData);

        savedIndex = Mathf.Clamp(savedIndex, 0, _currentEmojiData.EmojiSprites.Count - 1);
        _playerSign.sprite = _currentEmojiData.EmojiSprites[savedIndex];
    }

    public void SetEmojiData(EmojiData newData)
    {
        if (newData == null) return;

        _currentEmojiData = newData;

        _emojiSetter.SetEmojiData(newData);

        GD.Player.EmojiColor = newData.ColorName;
        GD.Player.EmojiIndex = Mathf.Clamp(GD.Player.EmojiIndex, 0, newData.EmojiSprites.Count - 1);
        GD.Save();

        _contentScroll.SetEmojiData(newData);
        _playerSign.sprite = newData.EmojiSprites[GD.Player.EmojiIndex];

        OnCheckMatchAISign?.Invoke();
    }
}
