using System.Collections.Generic;
using UnityEngine;

public sealed class PopupService : MonoBehaviour
{
    public static PopupService I { get; private set; }

    private readonly Dictionary<PopupId, PopupBase> _popups = new();
    private PopupBase _current;
    private PopupCanvasController _canvas;

    private void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetContext(PopupCanvasController canvas, IEnumerable<PopupBase> popups)
    {
        if (_canvas != null)
            _canvas.OverlayClicked -= OnOverlayClicked;

        _canvas = canvas;

        if (_canvas != null)
        {
            _canvas.OverlayClicked += OnOverlayClicked;
            _canvas.SetOverlayVisible(false);
        }

        _current = null;
        _popups.Clear();

        if (popups == null)
            return;

        foreach (var popup in popups)
        {
            if (popup == null)
                continue;

            _popups[popup.Id] = popup;
            popup.Hide();
        }
    }

    public bool CanShow(PopupId id) => _popups.ContainsKey(id);

    public void Show(PopupId id)
    {
        if (!_popups.TryGetValue(id, out var popup) || popup == null)
        {
            Debug.LogWarning($"Popup '{id}' not registered in current scene context.");
            return;
        }

        if (_current != null && _current != popup)
            _current.Hide();

        _current = popup;
        _current.Show();

        _canvas?.SetOverlayVisible(true);
    }

    public void HideCurrent()
    {
        if (_current == null)
            return;

        _current.Hide();
        _current = null;

        _canvas?.SetOverlayVisible(false);
    }

    private void OnOverlayClicked()
    {
        if (_current == null)
            return;

        if (!_current.CanCloseByBackground)
            return;

        HideCurrent();
    }
}