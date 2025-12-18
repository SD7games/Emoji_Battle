using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MainInstaller : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private MainUIView _uiView;

    [SerializeField] private MainSignView _signView;
    [SerializeField] private TurnState _turnState;
    [SerializeField] private AIMoveController _aiController;
    [SerializeField] private WinLineView _winLines;

    [Header("Popups")]
    [SerializeField] private PopupCanvasController _popupCanvas;

    [SerializeField] private PopupBase[] _scenePopups;

    [Header("Emoji Sets")]
    [SerializeField] private List<EmojiData> _emojiSets;

    private InputController _input;
    private EmojiResolver _resolver;
    private BoardState _board;
    private WinChecker _checker;
    private GameFlow _flow;
    private GameSession _session;

    private GameResultUI _resultUI;
    private GameRewardService _rewardService;

    private void Awake()
    {
        InitPopups();
        InitHeaderSigns();
        InitGameFlow();
        BindUI();
        SubscribeNameUpdates();
    }

    private void Start()
    {
        _signView.PlayIntroDissolve();
    }

    private void OnDestroy()
    {
        UnsubscribeNameUpdates();

        if (_uiView != null)
        {
            _uiView.OnRestartClicked -= _session.Restart;
            _uiView.OnBackClicked -= OnBackToLobby;
            _uiView.OnSettingsClicked -= OnOpenSettings;
        }

        if (_input != null)
            _input.OnCellClicked -= OnCellClicked;
    }

    private void InitPopups()
    {
        if (PopupService.I == null)
        {
            Debug.LogError("PopupService not initialized");
            return;
        }

        if (_popupCanvas == null)
        {
            Debug.LogError("PopupCanvas is NULL");
            return;
        }

        if (_scenePopups == null || _scenePopups.Length == 0)
        {
            Debug.LogWarning("No scene popups assigned");
        }

        PopupService.I.SetContext(_popupCanvas, _scenePopups);
    }

    private void InitHeaderSigns()
    {
        var save = GameDataService.I.Data;

        _resolver = new EmojiResolver(_emojiSets);

        Sprite player = _resolver.Get(save.Player.EmojiColor, save.Player.EmojiIndex);
        Sprite ai = _resolver.Get(save.AI.EmojiColor, save.AI.EmojiIndex);

        _signView.SetPlayer(player, save.Player.Name);
        _signView.SetAI(ai, save.AI.Name);
    }

    private void InitGameFlow()
    {
        _board = new BoardState();
        _checker = new WinChecker();

        _flow = new GameFlow(_board, _turnState, _checker);

        _input = new InputController(_uiView.BoardView.Buttons);
        _input.OnCellClicked += OnCellClicked;

        var save = GameDataService.I.Data;

        Sprite player = _resolver.Get(save.Player.EmojiColor, save.Player.EmojiIndex);
        Sprite ai = _resolver.Get(save.AI.EmojiColor, save.AI.EmojiIndex);

        _uiView.InitBoardSprites(player, ai);
        _uiView.BoardView.ResetView();
        _uiView.BoardView.SetInteractable(true);

        _flow.OnMoveApplied += _uiView.BoardView.OnMoveApplied;

        _aiController.Init(_flow);

        _flow.OnTurnChanged += isPlayerTurn =>
        {
            _uiView.BoardView.SetInteractable(isPlayerTurn);

            if (!isPlayerTurn)
                _aiController.MakeMove(_board.AsIntArray());
        };

        _resultUI = new GameResultUI(_winLines, _uiView.BoardView);
        _rewardService = new GameRewardService(_emojiSets);

        _flow.OnGameOver += (winner, line, finalBoard) =>
        {
            _resultUI.Show(winner, line, finalBoard);
            _rewardService.OnWin(winner);
            _uiView.BoardView.DisableAfterGameOver(finalBoard);
        };

        _session = new GameSession(_flow, _turnState, _winLines, _uiView.BoardView);
    }

    private void BindUI()
    {
        _uiView.OnRestartClicked += _session.Restart;
        _uiView.OnBackClicked += OnBackToLobby;
        _uiView.OnSettingsClicked += OnOpenSettings;
    }

    private void OnCellClicked(int index)
    {
        _flow.ProcessMove(index);
    }

    private void OnBackToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    private void OnOpenSettings()
    {
        PopupService.I.Show(PopupId.Settings);
    }

    private void SubscribeNameUpdates()
    {
        SettingsService.PlayerNameChanged += OnPlayerNameChanged;
    }

    private void UnsubscribeNameUpdates()
    {
        SettingsService.PlayerNameChanged -= OnPlayerNameChanged;
    }

    private void OnPlayerNameChanged(string name)
    {
        _signView.SetPlayerName(name);
    }
}