using UnityEngine;

public static class GD
{
    private static IStorage _storage;
    private static PlayerData _player;
    private static AIData _ai;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        _storage = new PlayerPrefsStorage();

        _player = new PlayerData();
        _ai = new AIData();

        Load();
    }

    public static PlayerData Player => _player;

    public static AIData AI => _ai;

    public static void Save()
    {
        _player.Save(_storage);
        _ai.Save(_storage);

        _storage.Save();
    }

    public static void Load()
    {
        _player.Load(_storage);
        _ai.Load(_storage);
    }

    public static void ResetAll()
    {
        _storage.Clear();

        _player = new PlayerData();
        _ai = new AIData();
    }
}
