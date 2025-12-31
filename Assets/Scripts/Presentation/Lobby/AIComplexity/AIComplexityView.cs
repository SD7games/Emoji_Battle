using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class AIComplexityView : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _mainButton;

    [SerializeField] private Button _optionButton1;
    [SerializeField] private Button _optionButton2;

    [Header("Audio")]
    [SerializeField] private SoundDefinition _openListSound;

    [SerializeField] private SoundDefinition _closeListSound;

    [Header("Animation")]
    [SerializeField] private float _animDuration = 0.25f;

    [SerializeField] private float _appearOffset = 20f;

    [SerializeField] private float _pulseDelay = 3f;
    [SerializeField] private float _pulseScale = 1.15f;
    [SerializeField] private float _pulseTime = 0.6f;

    public event Action OnMainClick;

    public event Action<Button> OnOptionClick;

    private RectTransform _mainRT;
    private RectTransform _opt1RT;
    private RectTransform _opt2RT;

    private CanvasGroup _opt1Group;
    private CanvasGroup _opt2Group;

    private Vector2 _opt1BasePos;
    private Vector2 _opt2BasePos;

    private Sequence _expandSeq;
    private Sequence _collapseSeq;
    private Tween _pulseTween;

    private bool _pulseAllowed = true;

    private void Awake()
    {
        _mainRT = _mainButton.GetComponent<RectTransform>();
        _opt1RT = _optionButton1.GetComponent<RectTransform>();
        _opt2RT = _optionButton2.GetComponent<RectTransform>();

        if (!_optionButton1.TryGetComponent(out _opt1Group))
            _opt1Group = _optionButton1.gameObject.AddComponent<CanvasGroup>();

        if (!_optionButton2.TryGetComponent(out _opt2Group))
            _opt2Group = _optionButton2.gameObject.AddComponent<CanvasGroup>();

        _opt1BasePos = _opt1RT.anchoredPosition;
        _opt2BasePos = _opt2RT.anchoredPosition;

        _mainButton.onClick.AddListener(() => OnMainClick?.Invoke());
        _optionButton1.onClick.AddListener(() => OnOptionClick?.Invoke(_optionButton1));
        _optionButton2.onClick.AddListener(() => OnOptionClick?.Invoke(_optionButton2));
    }

    public void PlayOpenListSound()
    {
        if (_openListSound == null)
            return;

        AudioService.I.PlaySFX(_openListSound);
    }

    public void PlayCloseListSound()
    {
        if (_closeListSound == null)
            return;

        AudioService.I.PlaySFX(_closeListSound);
    }

    public void SetMain(AIStrategyType type)
    {
        _mainButton.GetComponentInChildren<TMP_Text>().text = type.ToString();
        _mainButton.image.color = AIComplexityColors.Get(type);
    }

    public void SetOption1(AIStrategyType type)
    {
        _optionButton1.GetComponentInChildren<TMP_Text>().text = type.ToString();
        _optionButton1.image.color = AIComplexityColors.Get(type);
    }

    public void SetOption2(AIStrategyType type)
    {
        _optionButton2.GetComponentInChildren<TMP_Text>().text = type.ToString();
        _optionButton2.image.color = AIComplexityColors.Get(type);
    }

    public void Expand()
    {
        _pulseAllowed = false;
        StopPulse();
        Kill();

        _opt1Group.interactable = true;
        _opt2Group.interactable = true;
        _opt1Group.blocksRaycasts = true;
        _opt2Group.blocksRaycasts = true;

        _opt1Group.alpha = 0;
        _opt2Group.alpha = 0;

        _opt1RT.anchoredPosition = _opt1BasePos - new Vector2(0, _appearOffset);
        _opt2RT.anchoredPosition = _opt2BasePos - new Vector2(0, _appearOffset);

        _expandSeq = DOTween.Sequence()
            .Append(_opt1Group.DOFade(1f, _animDuration))
            .Join(_opt1RT.DOAnchorPos(_opt1BasePos, _animDuration))
            .Join(_opt2Group.DOFade(1f, _animDuration * 1.2f))
            .Join(_opt2RT.DOAnchorPos(_opt2BasePos, _animDuration * 1.2f))
            .SetEase(Ease.OutQuad);
    }

    public void Collapse()
    {
        Kill();

        _collapseSeq = DOTween.Sequence()
            .Append(_opt1Group.DOFade(0f, _animDuration))
            .Join(_opt2Group.DOFade(0f, _animDuration))
            .Join(_opt1RT.DOAnchorPos(_opt1BasePos - new Vector2(0, _appearOffset), _animDuration))
            .Join(_opt2RT.DOAnchorPos(_opt2BasePos - new Vector2(0, _appearOffset), _animDuration))
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                EnableOptions(false);
                _pulseAllowed = true;
                StartPulse();
            });
    }

    public void HideInstant()
    {
        EnableOptions(false);
        _opt1Group.alpha = 0;
        _opt2Group.alpha = 0;
        _opt1RT.anchoredPosition = _opt1BasePos;
        _opt2RT.anchoredPosition = _opt2BasePos;
        _pulseAllowed = true;
    }

    private void EnableOptions(bool enable)
    {
        _opt1Group.interactable = enable;
        _opt2Group.interactable = enable;
        _opt1Group.blocksRaycasts = enable;
        _opt2Group.blocksRaycasts = enable;
    }

    public void StartPulse()
    {
        if (!_pulseAllowed) return;

        StopPulse();

        _pulseTween = _mainRT
            .DOScale(_pulseScale, _pulseTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(_pulseDelay);
    }

    private void StopPulse()
    {
        _pulseTween?.Kill();
        _mainRT.localScale = Vector3.one;
    }

    private void Kill()
    {
        _expandSeq?.Kill();
        _collapseSeq?.Kill();
    }

    private void OnDestroy()
    {
        Kill();
        StopPulse();
    }
}