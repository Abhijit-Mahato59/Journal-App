using JournalApp.Data;
using JournalApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JournalApp.Services;

public class JournalService : SearchService
{
    private readonly AppDbContext _db;
    public JournalService(AppDbContext db) => _db = db;

    private static DateTime D(DateTime x) => x.Date;

    public async Task<List<JournalEntry>> GetRecentAsync(int count = 20)
    {
        return await _db.JournalEntries
            .OrderByDescending(e => e.EntryDate)
            .Take(count)
            .ToListAsync();
    }

    public Task<JournalEntry?> GetByDateAsync(DateTime date)
    {
        var d = D(date);
        return _db.JournalEntries.FirstOrDefaultAsync(e => e.EntryDate == d);
    }

    public async Task<JournalEntry> UpsertAsync(JournalEntry entry)
    {
        entry.EntryDate = D(entry.EntryDate);

        var existing = await _db.JournalEntries
            .FirstOrDefaultAsync(e => e.EntryDate == entry.EntryDate);

        if (existing is null)
        {
            _db.JournalEntries.Add(entry);
        }
        else
        {
            existing.Title = entry.Title;
            existing.ContentMarkdown = entry.ContentMarkdown;
            existing.Mood = entry.Mood;
        }

        await _db.SaveChangesAsync();
        return existing ?? entry;
    }

    public async Task DeleteByDateAsync(DateTime date)
    {
        var d = D(date);
        var existing = await _db.JournalEntries.FirstOrDefaultAsync(e => e.EntryDate == d);
        if (existing is null) return;

        _db.JournalEntries.Remove(existing);
        await _db.SaveChangesAsync();
    }

    public async Task<HashSet<DateTime>> GetEntryDatesInMonthAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        var dates = await _db.JournalEntries
            .Where(e => e.EntryDate >= start && e.EntryDate < end)
            .Select(e => e.EntryDate)
            .ToListAsync();

        return dates.Select(x => x.Date).ToHashSet();
    }
}
