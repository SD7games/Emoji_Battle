using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class MainUIView : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _restartButton;

    [SerializeField] private Button _backButton;
    [SerializeField] private Button _settingsButton;

    [Header("Board")]
    [SerializeField] private BoardView _boardView;

    public BoardView BoardView => _boardView;

    public event Action OnRestartClicked;

    public event Action OnBackClicked;

    public event Action OnSettingsClicked;

    public event Action<int> OnCellClicked;

    private void Awake()
    {
        _restartButton.onClick.AddListener(OnRestartPressed);
        _backButton.onClick.AddListener(OnBackPressed);
        _settingsButton.onClick.AddListener(OnSettingsPressed);

        _boardView.OnCellPressed += OnBoardCellPressed;
    }

    private void OnDestroy()
    {
        _restartButton.onClick.RemoveListener(OnRestartPressed);
        _backButton.onClick.RemoveListener(OnBackPressed);
        _settingsButton.onClick.RemoveListener(OnSettingsPressed);

        _boardView.OnCellPressed -= OnBoardCellPressed;
    }

    public void InitBoardSprites(Sprite player, Sprite ai)
    {
        _boardView.AssignSprites(player, ai);
    }

    private void OnRestartPressed() => OnRestartClicked?.Invoke();

    private void OnBackPressed() => OnBackClicked?.Invoke();

    private void OnSettingsPressed() => OnSettingsClicked?.Invoke();

    private void OnBoardCellPressed(int index) => OnCellClicked?.Invoke(index);
}