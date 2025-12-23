public enum AIStrategyType
{
    Easy,
    Norm,
    Hard
}

[System.Serializable]
public class AIProfile
{
    public string Name = "Bambik";
    public AIStrategyType Strategy = AIStrategyType.Easy;
    public int EmojiColor;
    public int EmojiIndex;
}