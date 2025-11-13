using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ColorSwitcher : MonoBehaviour
{
    [SerializeField] private List<Button> _colorButtons;
    [SerializeField] private List<EmojiData> _emojiDataByColor;
    [SerializeField] private PlayerLobbyController _playerLobby;

    private void Start()
    {
        for (int i = 0; i < _colorButtons.Count; i++)
        {
            int index = i;
            _colorButtons[i].onClick.AddListener(() =>
            {
                if (index >= 0 && index < _emojiDataByColor.Count)
                    _playerLobby.SetEmojiData(_emojiDataByColor[index]);
            });
        }
    }
}
