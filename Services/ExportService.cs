using Journal_App.Data.Models;

namespace Journal_App.Services;

public class ExportService
{
    public async Task<byte[]> ExportToPdfAsync(List<JournalEntry> entries)
    {
        await Task.CompletedTask;
        return Array.Empty<byte>();
    }
}