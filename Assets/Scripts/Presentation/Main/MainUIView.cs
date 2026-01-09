using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class MainUIView : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _backButton;

    [SerializeField] private Button _adsButton;

    [SerializeField] private Button _settingsButton;

    [Header("Board")]
    [SerializeField] private BoardView _boardView;

    public BoardView BoardView => _boardView;

    public event Action OnBackClicked;

    public event Action OnAdsClicked;

    public event Action OnSettingsClicked;

    public event Action<int> OnCellClicked;

    private void Awake()
    {
        _backButton.onClick.AddListener(OnBackPressed);
        _settingsButton.onClick.AddListener(OnSettingsPressed);
        _adsButton.onClick.AddListener(OnAdsPressed);

        _boardView.OnCellPressed += OnBoardCellPressed;
    }

    private void OnDestroy()
    {
        _backButton.onClick.RemoveListener(OnBackPressed);
        _settingsButton.onClick.RemoveListener(OnSettingsPressed);
        _adsButton.onClick.RemoveListener(OnAdsPressed);

        _boardView.OnCellPressed -= OnBoardCellPressed;
    }

    public void InitBoardSprites(Sprite player, Sprite ai)
    {
        _boardView.AssignSprites(player, ai);
    }

    private void OnAdsPressed()
    {
        OnAdsClicked?.Invoke();
    }

    private void OnBackPressed() => OnBackClicked?.Invoke();

    private void OnSettingsPressed() => OnSettingsClicked?.Invoke();

    private void OnBoardCellPressed(int index) => OnCellClicked?.Invoke(index);
}