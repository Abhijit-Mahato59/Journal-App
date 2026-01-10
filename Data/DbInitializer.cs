using Microsoft.EntityFrameworkCore;
using JournalApp.Models;

namespace JournalApp.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IDbContextFactory<AppDbContext> factory)
    {
        await using var db = await factory.CreateDbContextAsync();

        // Simple approach (no migrations needed for coursework)
        await db.Database.EnsureCreatedAsync();

        if (!await db.Moods.AnyAsync())
        {
            db.Moods.AddRange(new[]
            {
                new Mood { Name = "Happy", Group = MoodGroup.Positive },
                new Mood { Name = "Excited", Group = MoodGroup.Positive },
                new Mood { Name = "Calm", Group = MoodGroup.Neutral },
                new Mood { Name = "Neutral", Group = MoodGroup.Neutral },
                new Mood { Name = "Sad", Group = MoodGroup.Negative },
                new Mood { Name = "Anxious", Group = MoodGroup.Negative },
            });
        }

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.Add(new Category { Name = "General" });
        }

        if (!await db.Tags.AnyAsync())
        {
            db.Tags.AddRange(new[]
            {
                new Tag { Name = "Work", IsPrebuilt = true },
                new Tag { Name = "Study", IsPrebuilt = true },
                new Tag { Name = "Family", IsPrebuilt = true },
                new Tag { Name = "Health", IsPrebuilt = true },
                new Tag { Name = "Travel", IsPrebuilt = true },
            });
        }

        await db.SaveChangesAsync();
    }
}
