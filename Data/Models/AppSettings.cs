using System.ComponentModel.DataAnnotations;

namespace Journal_App.Data.Models;

public class AppSettings
{
    [Key]
    public int Id { get; set; } = 1;

    public string? PasswordHash { get; set; }

    public bool IsDarkMode { get; set; } = false;

    public DateTime? LastLoginDate { get; set; }
}