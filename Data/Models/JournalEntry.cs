using System.ComponentModel.DataAnnotations;

namespace Journal_App.Data.Models;

public class JournalEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public string PrimaryMood { get; set; } = string.Empty;

    public string? SecondaryMood1 { get; set; }
    public string? SecondaryMood2 { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public string? Tags { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int WordCount => string.IsNullOrWhiteSpace(Content)
        ? 0
        : Content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
}