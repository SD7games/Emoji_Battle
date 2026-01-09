using UnityEngine;

public sealed class GameDataService
{
    public static GameDataService I { get; private set; }

    public SavePayload Data { get; private set; }
    private IDataStorage _storage;

    private const int CURRENT_SAVE_VERSION = 1;

    private const string KEY = "save_payload";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        I ??= new GameDataService();
        I.Initialize();
    }

    private void Initialize()
    {
        _storage = new PlayerPrefsStorage();
        Data = _storage.LoadJson<SavePayload>(KEY);

        if (Data == null || Data.SaveVersion != CURRENT_SAVE_VERSION)
        {
            Data = new SavePayload();
            Data.SaveVersion = CURRENT_SAVE_VERSION;
            Save();
        }
    }

    public void Save()
    {
        _storage.SaveJson(KEY, Data);
    }
}