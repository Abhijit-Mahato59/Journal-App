namespace JournalApp.Services;

public static class DbPath
{
    public static string GetDatabasePath(string fileName = "journal.db")
        => Path.Combine(FileSystem.AppDataDirectory, fileName);
}
