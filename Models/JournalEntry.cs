namespace JournalApp.Models;

public class JournalEntry
{
    public int Id { get; set; }

    // store "date only" as DateTime.Date (00:00:00)
    public DateTime EntryDate { get; set; }

    public string Title { get; set; } = "";
    public string Content { get; set; } = ""; // markdown later

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int PrimaryMoodId { get; set; }
    public Mood? PrimaryMood { get; set; }

    public int? SecondaryMood1Id { get; set; }
    public Mood? SecondaryMood1 { get; set; }

    public int? SecondaryMood2Id { get; set; }
    public Mood? SecondaryMood2 { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public List<EntryTag> EntryTags { get; set; } = new();
}
