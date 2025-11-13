using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FadeScaleAnimator : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool _playOnStart = false;
    [SerializeField] private bool _startHidden = true;
    [SerializeField] private float _delayBeforeAnimate = 0f;

    [Header("Target (Any UI Graphic)")]
    [SerializeField] private Graphic _targetGraphic;

    [Header("Fade Settings")]
    [SerializeField] private bool _useFade = true;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private Ease _fadeEase = Ease.OutQuad;
    [SerializeField] private bool _fadeLoop = false;
    [SerializeField] private LoopType _fadeLoopType = LoopType.Yoyo;

    [Header("Scale Settings")]
    [SerializeField] private bool _useScale = true;
    [SerializeField] private bool _startFromZero = false;
    [SerializeField] private float _scaleDuration = 0.5f;
    [SerializeField] private float _targetScale = 1f;
    [SerializeField] private Ease _scaleEase = Ease.OutBack;
    [SerializeField] private bool _scaleLoop = false;
    [SerializeField] private LoopType _scaleLoopType = LoopType.Yoyo;

    private Vector3 _originalScale;
    private string _fadeTweenId;
    private string _scaleTweenId;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _fadeTweenId = $"{name}_fade";
        _scaleTweenId = $"{name}_scale";
    }

    private void Start()
    {
        if (_startHidden)
        {
            if (_useFade)
                SetAlpha(0f);

            if (_useScale && _startFromZero)
                transform.localScale = Vector3.zero;
        }

        if (_playOnStart)
            Show();
    }

    public void Show()
    {
        if (_delayBeforeAnimate > 0)
            DOVirtual.DelayedCall(_delayBeforeAnimate, AnimateIn);
        else
            AnimateIn();
    }

    public void Hide(Action onComplete = null)
    {
        AnimateOut(onComplete);
    }

    private void AnimateIn()
    {
        if (_useFade)
            PlayFade(1f, _fadeDuration, _fadeEase, _fadeLoop, _fadeLoopType);

        if (_useScale)
        {
            Vector3 start = _startFromZero ? Vector3.zero : _originalScale;
            Vector3 target = _originalScale * _targetScale;
            transform.localScale = start;
            PlayScale(target, _scaleDuration, _scaleEase, _scaleLoop, _scaleLoopType);
        }
    }

    private void AnimateOut(Action onComplete = null)
    {
        int activeTweens = 0;

        if (_useFade)
        {
            activeTweens++;
            PlayFade(0f, _fadeDuration, _fadeEase, false, LoopType.Restart)
                .OnComplete(() =>
                {
                    if (--activeTweens == 0)
                        onComplete?.Invoke();
                });
        }

        if (_useScale)
        {
            activeTweens++;
            Vector3 endScale = _startFromZero ? Vector3.zero : _originalScale;
            PlayScale(endScale, _scaleDuration, _scaleEase, false, LoopType.Restart)
                .OnComplete(() =>
                {
                    if (--activeTweens == 0)
                        onComplete?.Invoke();
                });
        }

        if (activeTweens == 0)
            onComplete?.Invoke();
    }

    private Tween PlayFade(float targetAlpha, float duration, Ease ease, bool loop, LoopType loopType)
    {
        DOTween.Kill(_fadeTweenId);

        if (_targetGraphic == null)
            return null;

        var tween = _targetGraphic.DOFade(targetAlpha, duration)
            .SetEase(ease)
            .SetId(_fadeTweenId)
            .SetAutoKill(true);

        if (loop)
            tween.SetLoops(-1, loopType);

        return tween;
    }

    private Tween PlayScale(Vector3 targetScale, float duration, Ease ease, bool loop, LoopType loopType)
    {
        DOTween.Kill(_scaleTweenId);

        var tween = transform.DOScale(targetScale, duration)
            .SetEase(ease)
            .SetId(_scaleTweenId)
            .SetAutoKill(true);

        if (loop)
            tween.SetLoops(-1, loopType);

        return tween;
    }

    private void SetAlpha(float value)
    {
        if (_targetGraphic == null)
            return;

        var color = _targetGraphic.color;
        color.a = value;
        _targetGraphic.color = color;
    }

    private void OnDestroy()
    {
        DOTween.Kill(_fadeTweenId);
        DOTween.Kill(_scaleTweenId);
    }
}
