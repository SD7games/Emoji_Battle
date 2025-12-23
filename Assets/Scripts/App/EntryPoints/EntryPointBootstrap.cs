using System;
using System.Threading.Tasks;
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

    private void Start()
    {
        _ = RunAsync();
    }

    private async Task RunAsync()
    {
        try
        {
            await _controller.StartAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}