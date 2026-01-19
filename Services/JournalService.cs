using Microsoft.EntityFrameworkCore;
using Journal_App.Data;
using Journal_App.Data.Models;

namespace Journal_App.Services;

public class JournalService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public JournalService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var dateOnly = date.Date;
        return await context.JournalEntries
            .FirstOrDefaultAsync(e => e.Date.Date == dateOnly);
    }

    public async Task<List<JournalEntry>> GetEntriesAsync(int skip = 0, int take = 20)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.JournalEntries
            .OrderByDescending(e => e.Date)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> GetEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.JournalEntries
            .Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> SearchEntriesAsync(string searchTerm)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.JournalEntries
            .Where(e => e.Title.Contains(searchTerm) || e.Content.Contains(searchTerm))
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> GetEntriesByMoodAsync(string mood)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.JournalEntries
            .Where(e => e.PrimaryMood == mood || e.SecondaryMood1 == mood || e.SecondaryMood2 == mood)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<List<JournalEntry>> GetEntriesByTagAsync(string tag)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.JournalEntries
            .Where(e => e.Tags != null && e.Tags.Contains(tag))
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<JournalEntry> CreateEntryAsync(JournalEntry entry)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existingEntry = await context.JournalEntries
            .FirstOrDefaultAsync(e => e.Date.Date == entry.Date.Date);

        if (existingEntry != null)
        {
            throw new InvalidOperationException("An entry already exists for this date.");
        }

        entry.CreatedAt = DateTime.Now;
        entry.UpdatedAt = DateTime.Now;

        context.JournalEntries.Add(entry);
        await context.SaveChangesAsync();
        return entry;
    }

    public async Task<JournalEntry> UpdateEntryAsync(JournalEntry entry)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.JournalEntries.FindAsync(entry.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Entry not found.");
        }

        existing.Title = entry.Title;
        existing.Content = entry.Content;
        existing.PrimaryMood = entry.PrimaryMood;
        existing.SecondaryMood1 = entry.SecondaryMood1;
        existing.SecondaryMood2 = entry.SecondaryMood2;
        existing.Category = entry.Category;
        existing.Tags = entry.Tags;
        existing.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteEntryAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var entry = await context.JournalEntries.FindAsync(id);
        if (entry != null)
        {
            context.JournalEntries.Remove(entry);
            await context.SaveChangesAsync();
        }
    }

    public async Task<int> GetTotalEntriesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.JournalEntries.CountAsync();
    }

    public async Task<(int currentStreak, int longestStreak, int missedDays)> GetStreakInfoAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var allEntries = await context.JournalEntries
            .OrderByDescending(e => e.Date)
            .Select(e => e.Date.Date)
            .ToListAsync();

        if (!allEntries.Any())
            return (0, 0, 0);

        int currentStreak = 0;
        var today = DateTime.Today;
        var checkDate = today;

        while (allEntries.Contains(checkDate))
        {
            currentStreak++;
            checkDate = checkDate.AddDays(-1);
        }

        int longestStreak = 0;
        int tempStreak = 0;
        var firstDate = allEntries.Last();

        for (var date = firstDate; date <= today; date = date.AddDays(1))
        {
            if (allEntries.Contains(date))
            {
                tempStreak++;
                longestStreak = Math.Max(longestStreak, tempStreak);
            }
            else
            {
                tempStreak = 0;
            }
        }

        var totalDays = (today - firstDate).Days + 1;
        var missedDays = totalDays - allEntries.Count;

        return (currentStreak, longestStreak, missedDays);
    }

    public async Task<Dictionary<string, int>> GetMoodDistributionAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.JournalEntries.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(e => e.Date.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(e => e.Date.Date <= endDate.Value.Date);

        var entries = await query.ToListAsync();

        var distribution = new Dictionary<string, int>
        {
            ["Positive"] = 0,
            ["Neutral"] = 0,
            ["Negative"] = 0
        };

        foreach (var entry in entries)
        {
            var category = MoodCategories.GetMoodCategory(entry.PrimaryMood);
            distribution[category]++;
        }

        return distribution;
    }

    public async Task<string?> GetMostFrequentMoodAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.JournalEntries.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(e => e.Date.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(e => e.Date.Date <= endDate.Value.Date);

        return await query
            .GroupBy(e => e.PrimaryMood)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync();
    }

    public async Task<Dictionary<string, int>> GetTagFrequencyAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.JournalEntries.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(e => e.Date.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(e => e.Date.Date <= endDate.Value.Date);

        var entries = await query.Where(e => e.Tags != null).ToListAsync();

        var tagFrequency = new Dictionary<string, int>();

        foreach (var entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry.Tags)) continue;

            var tags = entry.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var tag in tags)
            {
                var cleanTag = tag.Trim();
                if (tagFrequency.ContainsKey(cleanTag))
                    tagFrequency[cleanTag]++;
                else
                    tagFrequency[cleanTag] = 1;
            }
        }

        return tagFrequency.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }

    public async Task<double> GetAverageWordCountAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.JournalEntries.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(e => e.Date.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(e => e.Date.Date <= endDate.Value.Date);

        var entries = await query.ToListAsync();

        if (!entries.Any()) return 0;

        return entries.Average(e => e.WordCount);
    }
}