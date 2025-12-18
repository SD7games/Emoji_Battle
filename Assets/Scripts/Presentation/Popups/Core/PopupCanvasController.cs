using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class PopupCanvasController : MonoBehaviour
{
    [SerializeField] private Button _overlayButton;

    public event Action OverlayClicked;

    private void Awake()
    {
        if (_overlayButton != null)
            _overlayButton.onClick.AddListener(() => OverlayClicked?.Invoke());
    }

    private void OnDestroy()
    {
        if (_overlayButton != null)
            _overlayButton.onClick.RemoveAllListeners();
    }

    public void SetOverlayVisible(bool visible)
    {
        if (_overlayButton != null)
            _overlayButton.gameObject.SetActive(visible);
    }
}