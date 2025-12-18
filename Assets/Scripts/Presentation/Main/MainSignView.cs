using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MainSignView : MonoBehaviour
{
    [Header("PLAYER UI")]
    [SerializeField] private Image _playerEmojiImage;

    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private DissolveMain _playerDissolve;

    [Header("AI UI")]
    [SerializeField] private Image _aiEmojiImage;

    [SerializeField] private TMP_Text _aiNameText;
    [SerializeField] private DissolveMain _aiDissolve;

    public void SetPlayer(Sprite emoji, string name)
    {
        _playerEmojiImage.sprite = emoji;
        _playerNameText.text = name;
    }

    public void SetPlayerName(string name)
    {
        _playerNameText.text = name;
    }

    public void SetAI(Sprite emoji, string name)
    {
        _aiEmojiImage.sprite = emoji;
        _aiNameText.text = name;
    }

    public void PlayIntroDissolve()
    {
        _playerDissolve?.PlayDissolve();
        _aiDissolve?.PlayDissolve();
    }
}