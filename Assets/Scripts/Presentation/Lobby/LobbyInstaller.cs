using System.Collections.Generic;
using UnityEngine;

public sealed class LobbyInstaller : MonoBehaviour
{
    [SerializeField] private LobbyView _view;
    [SerializeField] private AIComplexityView _aiComplexityView;
    [SerializeField] private List<EmojiData> _emojiSets;
    [SerializeField] private PopupCanvasController _popupCanvas;
    [SerializeField] private PopupBase[] _scenePopups;

    private LobbyController _controller;
    private AIComplexityController _aiComplexityController;

    private void Start()
    {
        InitProgress();

        var context = CreateLobbyContext();
        _controller = context.Controller;

        InitPopups();
        InitAIComplexity();
        InitView();

        _controller.Initialize();
        _controller.SetInitialColor(context.SavedColor);

        InitPlayerAvatar(context.Resolver);
    }

    private void OnDestroy()
    {
        _controller?.Dispose();

        if (_aiComplexityController != null)
            _aiComplexityController.OnDifficultyChanged -= _controller.OnAIStrategyChanged;
    }

    private void InitProgress()
    {
        var dataService = GameDataService.I;
        var data = dataService.Data;

        if (data.Progress.HasAnyProgress())
            return;

        const int firstN = 4;
        Dictionary<int, int> total = new();

        foreach (var set in _emojiSets)
        {
            if (set == null)
                continue;

            int count = set.EmojiSprites?.Count ?? 0;
            total[set.ColorId] = count;
        }

        data.Progress.UnlockFirstNAllColors(total, firstN);
        dataService.Save();
    }

    private LobbyContext CreateLobbyContext()
    {
        var dataService = GameDataService.I;
        var data = dataService.Data;

        var resolver = new EmojiResolver(_emojiSets);
        var emojiService = new EmojiSelectionService(dataService, resolver);
        var aiService = new AISelectionService(dataService, _emojiSets);
        var lobbyService = new LobbyService(emojiService, aiService);

        var controller = new LobbyController(lobbyService);

        return new LobbyContext(
            controller,
            resolver,
            data.Player.EmojiColor
        );
    }

    private void InitPopups()
    {
        PopupService.I.SetContext(_popupCanvas, _scenePopups);
    }

    private void InitAIComplexity()
    {
        _aiComplexityController =
            _aiComplexityView.gameObject.AddComponent<AIComplexityController>();

        _aiComplexityController.Initialize(_aiComplexityView);
        _aiComplexityController.OnDifficultyChanged += _controller.OnAIStrategyChanged;
    }

    private void InitView()
    {
        _view.Construct(_controller);
    }

    private void InitPlayerAvatar(EmojiResolver resolver)
    {
        var data = GameDataService.I.Data;

        Sprite savedPlayer =
            resolver.Get(data.Player.EmojiColor, data.Player.EmojiIndex);

        if (savedPlayer != null)
            _view.ForceSetPlayerAvatar(savedPlayer);
    }

    private readonly struct LobbyContext
    {
        public readonly LobbyController Controller;
        public readonly EmojiResolver Resolver;
        public readonly int SavedColor;

        public LobbyContext(
            LobbyController controller,
            EmojiResolver resolver,
            int savedColor)
        {
            Controller = controller;
            Resolver = resolver;
            SavedColor = savedColor;
        }
    }
}