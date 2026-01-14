using UnityEngine;

public sealed class DevSceneInitializer : MonoBehaviour
{
    [SerializeField] private VibrationService _vibrationServicePrefab;
    [SerializeField] private PopupService _popupServicePrefab;
    [SerializeField] private AudioService _audioServicePrefab;
    [SerializeField] private AdsService _adsServicePrefab;
    [SerializeField] private MusicDefinition _devMusic;
    [SerializeField] private InternetService _internetServicePrefab;
#if UNITY_EDITOR

    private void Awake()
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

#endif

    private void Start()
    {
        if (_devMusic != null)
            AudioService.I.PlayMusic(_devMusic);
    }
}