namespace Journal_App.Data.Models;

public static class MoodCategories
{
    public static readonly Dictionary<string, List<string>> Moods = new()
    {
        ["Positive"] = new() { "Happy", "Excited", "Relaxed", "Grateful", "Confident" },
        ["Neutral"] = new() { "Calm", "Thoughtful", "Curious", "Nostalgic", "Bored" },
        ["Negative"] = new() { "Sad", "Angry", "Stressed", "Lonely", "Anxious" }
    };

    public static List<string> GetAllMoods()
    {
        return Moods.Values.SelectMany(x => x).OrderBy(x => x).ToList();
    }

    public static string GetMoodCategory(string mood)
    {
        foreach (var category in Moods)
        {
            if (category.Value.Contains(mood))
                return category.Key;
        }
        return "Neutral";
    }
}