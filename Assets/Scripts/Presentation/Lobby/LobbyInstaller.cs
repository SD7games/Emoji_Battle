using System.Collections.Generic;
using UnityEngine;

public sealed class LobbyInstaller : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private LobbyView _view;

    [SerializeField] private AIComplexityView _aiComplexityView;

    [Header("Data")]
    [SerializeField] private List<EmojiData> _emojiSets;

    [Header("Audio")]
    [SerializeField] private SoundDefinition _emojiChooseSound;

    [SerializeField] private SoundDefinition _colorSelectSound;

    [Header("Popups")]
    [SerializeField] private PopupCanvasController _popupCanvas;

    [SerializeField] private PopupBase[] _scenePopups;

    private LobbyController _controller;
    private AIComplexityController _aiComplexityController;

    private void Start()
    {
        InitProgress();

        var context = CreateContext();
        _controller = context.Controller;

        InitPopups(context.Resolver);
        InitAIComplexity();
        InitView(context);
        InitPlayerAvatar(context.Resolver);

        _controller.Initialize();
        _controller.SetInitialColor(context.SavedColor);
    }

    private void OnDestroy()
    {
        _controller?.Dispose();

        if (_aiComplexityController != null)
            _aiComplexityController.OnDifficultyChanged -= _controller.OnAIStrategyChanged;
    }

    private LobbyContext CreateContext()
    {
        var dataService = GameDataService.I;
        var data = dataService.Data;

        var resolver = new EmojiResolver(_emojiSets);

        var emojiService = new EmojiSelectionService(dataService, resolver);
        var aiService = new AISelectionService(dataService, _emojiSets);
        var lobbyService = new LobbyService(emojiService, aiService);

        var rewards = new GameRewardService(_emojiSets);

        var controller = new LobbyController(
            lobbyService,
            resolver,
            rewards,
            _emojiChooseSound,
            _colorSelectSound
        );

        return new LobbyContext(
            controller,
            resolver,
            data.Player.EmojiColor
        );
    }

    private void InitProgress()
    {
        var dataService = GameDataService.I;
        var data = dataService.Data;

        if (data.IsFirstLaunchDone)
            return;

        const int firstN = 4;
        Dictionary<int, int> total = new();

        foreach (var set in _emojiSets)
        {
            if (set == null)
                continue;

            total[set.ColorId] = set.EmojiSprites?.Count ?? 0;
        }

        data.Progress.UnlockFirstNAllColors(total, firstN);
        data.IsFirstLaunchDone = true;
        dataService.Save();
    }

    private void InitPopups(EmojiResolver resolver)
    {
        PopupService.I.SetContext(_popupCanvas, _scenePopups);

        foreach (var popup in _scenePopups)
        {
            if (popup is IEmojiResolverConsumer consumer)
                consumer.Construct(resolver);
        }
    }

    private void InitAIComplexity()
    {
        _aiComplexityController =
            _aiComplexityView.gameObject.AddComponent<AIComplexityController>();

        _aiComplexityController.Initialize(_aiComplexityView);
        _aiComplexityController.OnDifficultyChanged += _controller.OnAIStrategyChanged;
    }

    private void InitView(LobbyContext context)
    {
        _view.Construct(context.Controller);
    }

    private void InitPlayerAvatar(EmojiResolver resolver)
    {
        var data = GameDataService.I.Data;

        Sprite sprite = resolver.Get(
            data.Player.EmojiColor,
            data.Player.EmojiIndex
        );

        if (sprite != null)
            _view.ForceSetPlayerAvatar(sprite);
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