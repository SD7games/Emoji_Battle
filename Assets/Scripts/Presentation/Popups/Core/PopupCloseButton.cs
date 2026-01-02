using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class PopupCloseButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private float _delay = 0.8f;

    private void Awake()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        _button.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        _button.interactable = false;
        StartCoroutine(EnableAfterDelay());
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(Close);
    }

    private IEnumerator EnableAfterDelay()
    {
        yield return new WaitForSeconds(_delay);
        _button.interactable = true;
    }

    private void Close()
    {
        if (!_button.interactable)
            return;

        PopupService.I.HideCurrent();
    }
}