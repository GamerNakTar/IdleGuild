public sealed class QuestDefinition
{
    public QuestDefinition(
        string id,
        string title,
        string description,
        float durationSeconds,
        int rewardGold,
        int rewardExperience)
    {
        Id = id;
        Title = title;
        Description = description;
        DurationSeconds = durationSeconds;
        RewardGold = rewardGold;
        RewardExperience = rewardExperience;
    }

    public string Id { get; }
    public string Title { get; }
    public string Description { get; }
    public float DurationSeconds { get; }
    public int RewardGold { get; }
    public int RewardExperience { get; }

    public string DurationLabel => $"{DurationSeconds:0.#}초";
    public string RewardLabel => $"골드 {RewardGold} / 경험치 {RewardExperience}";
}
