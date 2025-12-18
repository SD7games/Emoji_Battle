using UnityEngine;

public sealed class DevSceneInitializer : MonoBehaviour
{
    [SerializeField] private PopupService _popupServicePrefab;

    private void Awake()
    {
        Instantiate(_popupServicePrefab);
    }
}