using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Journal_App.Data;
using System.Security.Cryptography;
using System.Text;

namespace Journal_App.Services;

public class AuthService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly IJSRuntime _jsRuntime;

    public AuthService(IDbContextFactory<AppDbContext> contextFactory, IJSRuntime jsRuntime)
    {
        _contextFactory = contextFactory;
        _jsRuntime = jsRuntime;
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

    public async Task RemovePasswordAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.AppSettings.FindAsync(1);

        if (settings != null)
        {
            settings.PasswordHash = null;
            await context.SaveChangesAsync();
        }

        // Also clear the session
        await LogoutAsync();
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

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var authenticated = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "journal_authenticated");
            return authenticated == "true";
        }
        catch
        {
            return false;
        }
    }

    public async Task SetAuthenticatedAsync(bool authenticated)
    {
        try
        {
            if (authenticated)
            {
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "journal_authenticated", "true");
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "journal_authenticated");
            }
        }
        catch
        {
            // Ignore errors
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "journal_authenticated");
        }
        catch
        {
            // Ignore errors
        }
    }
}