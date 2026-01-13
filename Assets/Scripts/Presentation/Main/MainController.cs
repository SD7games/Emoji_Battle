using System;
using UnityEngine.SceneManagement;

public sealed class MainController : IDisposable
{
    private readonly GameFlow _flow;
    private readonly GameSession _session;
    private readonly BoardState _board;

    private readonly MainUIView _ui;
    private readonly MainSignView _signView;
    private readonly AIMoveController _ai;
    private readonly InputController _input;
    private readonly GameResultController _results;
    private readonly EmojiResolver _resolver;
    private readonly PopupService _popups;
    private readonly AudioService _audio;
    private readonly PopupBase[] _scenePopups;

    private readonly SoundDefinition _playerMoveSfx;
    private readonly SoundDefinition _aiMoveSfx;
    private readonly SoundDefinition _backToLobbySfx;

    private bool _disposed;

    public MainController
     (
     GameFlow flow,
     GameSession session,
     BoardState board,
     MainUIView ui,
     MainSignView signView,
     AIMoveController ai,
     InputController input,
     GameResultController results,
     GameRewardService rewards,
     EmojiResolver resolver,
     PopupService popups,
     AudioService audio,
     PopupBase[] scenePopups,

     SoundDefinition playerMoveSfx,
     SoundDefinition aiMoveSfx,
     SoundDefinition backToLobbySfx
     )
    {
        _flow = flow;
        _session = session;
        _board = board;
        _ui = ui;
        _signView = signView;
        _ai = ai;
        _input = input;
        _results = results;
        _resolver = resolver;
        _popups = popups;
        _audio = audio;
        _scenePopups = scenePopups;

        _playerMoveSfx = playerMoveSfx;
        _aiMoveSfx = aiMoveSfx;
        _backToLobbySfx = backToLobbySfx;
    }

    public void Initialize()
    {
        InitHeaderSigns();

        _ui.OnBackClicked += OnBackToLobby;
        _ui.OnSettingsClicked += OnOpenSettings;

        _input.OnCellClicked += OnCellClicked;

        _flow.OnTurnChanged += OnTurnChanged;
        _flow.OnGameOver += OnGameOver;
        _flow.OnMoveApplied += OnMoveApplied;

        _results.ResultReady += OnResultReady;

        BindResultPopups();
        SettingsService.PlayerNameChanged += OnPlayerNameChanged;
    }

    private void OnMoveApplied(int index, CellState state)
    {
        if (state == CellState.Player)
            _audio.PlaySFX(_playerMoveSfx);
        else if (state == CellState.AI)
            _audio.PlaySFX(_aiMoveSfx);
    }

    public void PlayIntro()
    {
        _signView.PlayIntroDissolve();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        SettingsService.PlayerNameChanged -= OnPlayerNameChanged;
        UnbindResultPopups();

        _results.ResultReady -= OnResultReady;
        _flow.OnGameOver -= OnGameOver;
        _flow.OnTurnChanged -= OnTurnChanged;
        _flow.OnMoveApplied -= OnMoveApplied;

        _input.OnCellClicked -= OnCellClicked;

        _ui.OnBackClicked -= OnBackToLobby;
        _ui.OnSettingsClicked -= OnOpenSettings;
    }

    private void InitHeaderSigns()
    {
        var save = GameDataService.I.Data;

        _signView.SetPlayer(
            _resolver.Get(save.Player.EmojiColor, save.Player.EmojiIndex),
            save.Player.Name
        );

        _signView.SetAI(
            _resolver.Get(save.AI.EmojiColor, save.AI.EmojiIndex),
            save.AI.Name
        );
    }

    private void OnCellClicked(int index)
    {
        _flow.ProcessMove(index);
    }

    private void OnTurnChanged(bool isPlayerTurn)
    {
        _ui.BoardView.SetInteractable(isPlayerTurn);

        if (!isPlayerTurn)
        {
            _ai.MakeMove(_board.AsIntArray());
        }
    }

    private void OnGameOver(
        CellState winner,
        WinLineView.WinLineType? line,
        CellState[,] finalBoard)
    {
        _ui.BoardView.DisableAfterGameOver(finalBoard);
        _results.HandleGameOver(winner, line);
    }

    private void OnResultReady(CellState winner, GameRewardResult reward)
    {
        if (winner == CellState.Player)
        {
            _popups.Show(
                reward.EmojiUnlocked ? PopupId.Victory : PopupId.Complete
            );
            return;
        }

        _popups.Show(
            winner == CellState.AI ? PopupId.Defeat : PopupId.Draw
        );
    }

    private void BindResultPopups()
    {
        foreach (var popup in _scenePopups)
            if (popup is ResultPopup r)
                r.Closed += OnResultPopupClosed;
    }

    private void UnbindResultPopups()
    {
        foreach (var popup in _scenePopups)
            if (popup is ResultPopup r)
                r.Closed -= OnResultPopupClosed;
    }

    private void OnResultPopupClosed()
    {
        AdsService.I?.NotifyMatchFinished();
        _session.Restart();
    }

    private void OnBackToLobby()
    {
        _audio.PlaySFX(_backToLobbySfx);
        SceneManager.LoadScene("Lobby");
    }

    private void OnOpenSettings()
    {
        _popups.Show(PopupId.Settings);
    }

    private void OnPlayerNameChanged(string name)
    {
        _signView.SetPlayerName(name);
    }
}