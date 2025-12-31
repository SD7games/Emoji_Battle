using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowScrollController : MonoBehaviour
{
    [Header("Scroll Components")]
    [SerializeField] private ScrollRect _scrollRect;

    [Header("Buttons")]
    [SerializeField] private Button _upButton;

    [SerializeField] private Button _downButton;

    [Header("Settings")]
    [SerializeField, Range(0.05f, 1f)]
    private float _step = 0.2f;

    [SerializeField, Range(0.05f, 1.5f)]
    private float _smoothTime = 0.25f;

    [Header("Audio")]
    [SerializeField] private SoundDefinition _scrollSound;

    private Coroutine _scrollRoutine;
    private bool _isAnimating;

    private void Start()
    {
        _upButton.onClick.AddListener(() => Scroll(true));
        _downButton.onClick.AddListener(() => Scroll(false));

        _scrollRect.onValueChanged.AddListener(OnScrollChanged);

        UpdateArrowStates();
    }

    private void Scroll(bool up)
    {
        if (_isAnimating)
            return;

        float start = _scrollRect.verticalNormalizedPosition;
        float target = Mathf.Clamp01(start + (up ? _step : -_step));

        if (Mathf.Approximately(start, target))
            return;

        if (_scrollRoutine != null)
            StopCoroutine(_scrollRoutine);

        PlayScrollSound();
        _scrollRoutine = StartCoroutine(SmoothScroll(start, target));
    }

    private IEnumerator SmoothScroll(float start, float target)
    {
        _isAnimating = true;
        SetRaycast(false);

        float time = 0f;

        while (time < _smoothTime)
        {
            time += Time.deltaTime;

            float t = time / _smoothTime;
            t = t * t * (3f - 2f * t);

            _scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, target, t);
            yield return null;
        }

        _scrollRect.verticalNormalizedPosition = target;

        SetRaycast(true);
        _isAnimating = false;

        UpdateArrowStates();
    }

    private void PlayScrollSound()
    {
        if (_scrollSound == null)
            return;

        AudioService.I.PlaySFX(_scrollSound);
    }

    private void OnScrollChanged(Vector2 _)
    {
        if (!_isAnimating)
            UpdateArrowStates();
    }

    private void UpdateArrowStates()
    {
        float pos = _scrollRect.verticalNormalizedPosition;

        _upButton.interactable = pos < 0.98f;
        _downButton.interactable = pos > 0.02f;
    }

    private void SetRaycast(bool value)
    {
        _upButton.image.raycastTarget = value;
        _downButton.image.raycastTarget = value;
    }
}