using System;
using UnityEngine;

public class EntryPointBootstrap : MonoBehaviour
{
    [SerializeField]
    private BootstrapView _bootstrapView;

    [SerializeField]
    private PopupService _popupServicePrefab;

    private BootstrapController _controller;

    private void Awake()
    {
        if (PopupService.I == null)
            Instantiate(_popupServicePrefab);

        _controller = new BootstrapController(_bootstrapView);
    }

    private async void Start()
    {
        try
        {
            await _controller.StartAsynk();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}