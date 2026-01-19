using Microsoft.EntityFrameworkCore;
using Journal_App.Data;
using System.Security.Cryptography;
using System.Text;

namespace Journal_App.Services;

public class AuthService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public AuthService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<bool> IsPasswordSetAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.AppSettings.FindAsync(1);
        return settings?.PasswordHash != null;
    }

    public async Task SetPasswordAsync(string password)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.AppSettings.FindAsync(1);

        if (settings == null)
        {
            settings = new Data.Models.AppSettings { Id = 1 };
            context.AppSettings.Add(settings);
        }

        settings.PasswordHash = HashPassword(password);
        await context.SaveChangesAsync();
    }

    public async Task<bool> VerifyPasswordAsync(string password)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.AppSettings.FindAsync(1);

        if (settings?.PasswordHash == null)
            return false;

        return settings.PasswordHash == HashPassword(password);
    }

    public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
    {
        if (!await VerifyPasswordAsync(oldPassword))
            return false;

        await SetPasswordAsync(newPassword);
        return true;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public async Task UpdateLastLoginAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.AppSettings.FindAsync(1);

        if (settings != null)
        {
            settings.LastLoginDate = DateTime.Now;
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> GetDarkModeAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.AppSettings.FindAsync(1);
        return settings?.IsDarkMode ?? false;
    }

    public async Task SetDarkModeAsync(bool isDark)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.AppSettings.FindAsync(1);

        if (settings != null)
        {
            settings.IsDarkMode = isDark;
            await context.SaveChangesAsync();
        }
    }
}