using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Journal_App.Data;
using Journal_App.Services;

namespace Journal_App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Blazor WebView
        builder.Services.AddMauiBlazorWebView();

        // MudBlazor
        builder.Services.AddMudServices();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // SQLite Database
        var dbPath = DbPaths.GetDbPath("journal.db");
        builder.Services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Services
        builder.Services.AddScoped<JournalService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<ExportService>();

        return builder.Build();
    }
}