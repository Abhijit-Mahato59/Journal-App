namespace Journal_App.Data;

public static class DbPaths
{
    public static string GetDbPath(string dbName)
    {
        var appDataPath = FileSystem.AppDataDirectory;
        return Path.Combine(appDataPath, dbName);
    }
}