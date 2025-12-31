using UnityEngine;

public sealed class DevSceneInitializer : MonoBehaviour
{
    [SerializeField] private PopupService _popupServicePrefab;
    [SerializeField] private AudioService _audioServicePrefab;
    [SerializeField] private MusicDefinition _devMusic;
#if UNITY_EDITOR

    private void Awake()
    {
        if (PopupService.I == null)
            Instantiate(_popupServicePrefab);

        if (AudioService.I == null)
            Instantiate(_audioServicePrefab);
    }

#endif

    private void Start()
    {
        if (_devMusic != null)
            AudioService.I.PlayMusic(_devMusic);
    }
}