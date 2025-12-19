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

    protected CanvasGroup CanvasGroup { get; private set; }
    protected RectTransform Rect { get; private set; }

    private Tween _tween;

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

    public virtual void Show()
    {
        _tween?.Kill();

        gameObject.SetActive(true);

        Rect.localScale = Vector3.one * 0.85f;
        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;

        _tween = DOTween.Sequence()
            .Append(Rect.DOScale(1f, _showDuration).SetEase(_showEase))
            .Join(CanvasGroup.DOFade(1f, _showDuration))
            .OnComplete(() =>
            {
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;
            });
    }

    public virtual void Hide()
    {
        _tween?.Kill();

        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;

        _tween = DOTween.Sequence()
            .Append(Rect.DOScale(0.85f, _hideDuration).SetEase(_hideEase))
            .Join(CanvasGroup.DOFade(0f, _hideDuration))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}