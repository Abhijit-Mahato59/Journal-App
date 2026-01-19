using Microsoft.EntityFrameworkCore;

namespace Journal_App.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IDbContextFactory<AppDbContext> factory)
    {
        await using var context = await factory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }
}