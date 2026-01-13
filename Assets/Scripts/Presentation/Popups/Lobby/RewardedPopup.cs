using UnityEngine;
using UnityEngine.UI;

public sealed class LootBoxPopup : ResultPopup, IEmojiResolverConsumer
{
    public override PopupId Id => PopupId.Reward;

    [SerializeField] private Image _emojiImage;
    [SerializeField] private int _previewColorId = 0;
    [SerializeField] private ParticleSystem _poofFx;

    private EmojiResolver _resolver;

    public void Construct(EmojiResolver resolver)
    {
        _resolver = resolver;
    }

    public override void Show()
    {
        base.Show();
        _poofFx.Clear(true);
        _poofFx.Play(true);
        UpdateEmojiPreview();
    }

    private void UpdateEmojiPreview()
    {
        if (_emojiImage == null || _resolver == null)
            return;

        var progress = GameDataService.I.Data.Progress;
        int index = progress.LastUnlockedGlobalIndex;

        if (index < 0)
        {
            _emojiImage.enabled = false;
            return;
        }

        var sprite = _resolver.Get(_previewColorId, index);
        _emojiImage.sprite = sprite;
        _emojiImage.enabled = sprite != null;
    }
}