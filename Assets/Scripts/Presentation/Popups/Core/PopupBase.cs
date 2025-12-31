using DG.Tweening;
using UnityEngine;

public abstract class PopupBase : MonoBehaviour
{
    public abstract PopupId Id { get; }
    public virtual bool CanCloseByBackground => true;

    [Header("Animation")]
    [SerializeField] private float _showDuration = 0.25f;

    [SerializeField] private float _hideDuration = 0.2f;
    [SerializeField] private Ease _showEase = Ease.OutBack;
    [SerializeField] private Ease _hideEase = Ease.InBack;

    [Header("Audio")]
    [SerializeField] private SoundDefinition _showSound;

    [SerializeField] private SoundDefinition _hideSound;

    protected CanvasGroup CanvasGroup { get; private set; }
    protected RectTransform Rect { get; private set; }

    private Tween _tween;
    private bool _wasShown;

    protected virtual void Awake()
    {
        Rect = transform as RectTransform;

        CanvasGroup = GetComponent<CanvasGroup>();
        if (CanvasGroup == null)
            CanvasGroup = gameObject.AddComponent<CanvasGroup>();

        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;

        gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        DOTween.Kill(this);
        _tween?.Kill();
    }

    public virtual void Show()
    {
        DOTween.Kill(this);
        _tween?.Kill();

        gameObject.SetActive(true);

        Rect.localScale = Vector3.one * 0.85f;
        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;

        _wasShown = true;
        PlayShowSound();

        _tween = DOTween.Sequence()
            .SetTarget(this)
            .Append(Rect.DOScale(1f, _showDuration).SetEase(_showEase))
            .Join(CanvasGroup.DOFade(1f, _showDuration))
            .OnComplete(() =>
            {
                if (!this) return;

                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;
            });
    }

    private void PlayShowSound()
    {
        if (_showSound == null)
            return;

        AudioService.I.PlaySFX(_showSound);
    }

    public virtual void Hide()
    {
        DOTween.Kill(this);
        _tween?.Kill();

        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;

        if (_wasShown)
            PlayHideSound();

        _tween = DOTween.Sequence()
            .SetTarget(this)
            .Append(Rect.DOScale(0.85f, _hideDuration).SetEase(_hideEase))
            .Join(CanvasGroup.DOFade(0f, _hideDuration))
            .OnComplete(() =>
            {
                if (!this) return;
                gameObject.SetActive(false);
            });

        _wasShown = false;
    }

    private void PlayHideSound()
    {
        if (_hideSound == null)
            return;

        AudioService.I.PlaySFX(_hideSound);
    }
}