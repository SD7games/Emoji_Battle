using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class PopupCanvasController : MonoBehaviour
{
    [SerializeField] private Button _overlayButton;
    [SerializeField] private float _clickDelay = 1f;

    public event Action OverlayClicked;

    private void Awake()
    {
        if (_overlayButton != null)
            _overlayButton.onClick.AddListener(OnOverlayClicked);
    }

    private void OnDestroy()
    {
        if (_overlayButton != null)
            _overlayButton.onClick.RemoveListener(OnOverlayClicked);
    }

    private void OnOverlayClicked()
    {
        if (!_overlayButton.interactable)
            return;

        OverlayClicked?.Invoke();
    }

    public void SetOverlayVisible(bool visible)
    {
        if (_overlayButton == null)
            return;

        _overlayButton.gameObject.SetActive(visible);

        if (visible)
        {
            _overlayButton.interactable = false;
            StartCoroutine(EnableOverlayAfterDelay());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator EnableOverlayAfterDelay()
    {
        yield return new WaitForSeconds(_clickDelay);
        _overlayButton.interactable = true;
    }
}