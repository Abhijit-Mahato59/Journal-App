using System.ComponentModel.DataAnnotations;

namespace JournalApp.Data.Entities;

public class JournalEntry
{
    public int Id { get; set; }

    // One entry per day
    public DateTime EntryDate { get; set; }  // store Date only (use .Date)

    [MaxLength(150)]
    public string Title { get; set; } = "";

    public string ContentMarkdown { get; set; } = "";

    public Mood Mood { get; set; } = Mood.Neutral;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
