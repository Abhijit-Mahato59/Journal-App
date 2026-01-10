namespace JournalApp.Models;

public class Mood
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public MoodGroup Group { get; set; }
}
