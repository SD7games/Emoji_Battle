[System.Serializable]
public class SavePayload
{
    public PlayerProfile Player = new();
    public AIProfile AI = new();
    public GameProgress Progress = new();

    public SettingsData Settings = new();

    public bool IsFirstLaunchDone;
    public int SaveVersion;
}