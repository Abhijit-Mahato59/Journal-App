namespace JournalApp.Data;

public static class DbPaths
{
    public static string GetDbPath(string fileName = "journal.db")
    {
        // Local app data folder (works on Windows/Android etc.)
        return Path.Combine(FileSystem.AppDataDirectory, fileName);
    }
}
