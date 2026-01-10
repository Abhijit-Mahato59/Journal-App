using JournalApp.Data.Entities;

namespace JournalApp.Services;

public interface SearchService
{
    Task<List<JournalEntry>> GetRecentAsync(int count = 20);
    Task<JournalEntry?> GetByDateAsync(DateTime date);
    Task<JournalEntry> UpsertAsync(JournalEntry entry);
    Task DeleteByDateAsync(DateTime date);

    Task<HashSet<DateTime>> GetEntryDatesInMonthAsync(int year, int month);
}
