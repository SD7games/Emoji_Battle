using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private GameRewardService _rewardService;
    private GameResultController _resultController;

    private void Awake()
    {
        _resolver = new EmojiResolver(_emojiSets);

        InitPopups();
        InitHeaderSigns();
        InitGameFlow();
        InjectPopupDependencies();
        BindUI();
        BindResultPopups();
        SubscribeNameUpdates();
    }

    private void InjectPopupDependencies()
    {
        foreach (var popup in _scenePopups)
        {
            if (popup is IEmojiResolverConsumer consumer)
                consumer.Construct(_resolver);
        }
    }

    private void Start()
    {
        _signView.PlayIntroDissolve();
    }

    public void OpenLootBox()
    {
        var result = _rewardService.OnLootBoxOpened();

        if (result.AllEmojisUnlocked)
            PopupService.I.Show(PopupId.Complete);
        else if (result.EmojiUnlocked)
            PopupService.I.Show(PopupId.LootBox);
        else
            PopupService.I.Show(PopupId.Complete);
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.lKey.wasPressedThisFrame)
            OpenLootBox();
    }

#endif

    private void OnDestroy()
    {
        UnsubscribeNameUpdates();
        UnbindResultPopups();

        if (_uiView != null)
        {
            _uiView.OnBackClicked -= OnBackToLobby;
            _uiView.OnSettingsClicked -= OnOpenSettings;
        }

        if (_input != null)
            _input.OnCellClicked -= OnCellClicked;

        if (_flow != null)
            _flow.OnGameOver -= OnGameOver;
    }

    private void InitPopups()
    {
        if (PopupService.I == null)
        {
            Debug.LogError("PopupService not initialized");
            return;
        }

        PopupService.I.SetContext(_popupCanvas, _scenePopups);
    }

    private void InitHeaderSigns()
    {
        var save = GameDataService.I.Data;

        Sprite player = _resolver.Get(save.Player.EmojiColor, save.Player.EmojiIndex);
        Sprite ai = _resolver.Get(save.AI.EmojiColor, save.AI.EmojiIndex);

        _signView.SetPlayer(player, save.Player.Name);
        _signView.SetAI(ai, save.AI.Name);
    }

    private void InitGameFlow()
    {
        PlayerPrefs.DeleteAll();
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

        _rewardService = new GameRewardService(_emojiSets);
        _resultController = new GameResultController(_winLines, _rewardService, this, _input);

        _flow.OnGameOver += OnGameOver;

        _session = new GameSession(_flow, _turnState, _winLines, _uiView.BoardView);
    }

    private void BindUI()
    {
        _uiView.OnBackClicked += OnBackToLobby;
        _uiView.OnSettingsClicked += OnOpenSettings;
    }

    private void BindResultPopups()
    {
        foreach (var popup in _scenePopups)
        {
            if (popup is ResultPopup resultPopup)
            {
                resultPopup.Closed += OnResultPopupClosed;
            }
        }
    }

    private void UnbindResultPopups()
    {
        foreach (var popup in _scenePopups)
        {
            if (popup is ResultPopup resultPopup)
            {
                resultPopup.Closed -= OnResultPopupClosed;
            }
        }
    }

    private void OnCellClicked(int index)
    {
        _flow.ProcessMove(index);
    }

    private void OnGameOver(
        CellState winner,
        WinLineView.WinLineType? line,
        CellState[,] finalBoard
    )
    {
        _uiView.BoardView.DisableAfterGameOver(finalBoard);
        _resultController.HandleGameOver(winner, line);
    }

    private void OnResultPopupClosed()
    {
        _session.Restart();
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