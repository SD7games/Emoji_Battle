using System;

[Serializable]
public class PlayerData
{
    public string Name = "Player";
    public string EmojiColor = "Default";
    public int EmojiIndex = 0;

    public void Save(IStorage storage)
    {
        storage.SaveString("Name", Name);
        storage.SaveString("EmojiColor", EmojiColor);
        storage.SaveInt("EmojiIndex", EmojiIndex);
    }

    public void Load(IStorage storage)
    {
        Name = storage.LoadString("Name", Name);
        EmojiColor = storage.LoadString("EmojiColor", EmojiColor);
        EmojiIndex = storage.LoadInt("EmojiIndex", EmojiIndex);
    }
}
