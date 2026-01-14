using System;
using System.Threading.Tasks;
using UnityEngine;

public sealed class EntryPointBootstrap : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private BootstrapView _bootstrapView;

    [Header("Service Prefabs")]
    [SerializeField] private VibrationService _vibrationServicePrefab;

    [SerializeField] private PopupService _popupServicePrefab;
    [SerializeField] private AudioService _audioServicePrefab;
    [SerializeField] private AdsService _adsServicePrefab;
    [SerializeField] private InternetService _internetServicePrefab;

    [Header("Background Music")]
    [SerializeField] private MusicDefinition _backgroundMusic;

    private BootstrapController _controller;

    private void Awake()
    {
        EnsureServices();
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
            StartBackgroundMusic();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void EnsureServices()
    {
        if (InternetService.I == null)
            Instantiate(_internetServicePrefab);

        if (VibrationService.I == null)
            Instantiate(_vibrationServicePrefab);

        if (PopupService.I == null)
            Instantiate(_popupServicePrefab);

        if (AudioService.I == null)
            Instantiate(_audioServicePrefab);

        if (AdsService.I == null)
            Instantiate(_adsServicePrefab);
    }

    private void StartBackgroundMusic()
    {
        if (_backgroundMusic == null)
            return;

        AudioService.I.PlayMusic(_backgroundMusic);
    }
}