using JournalApp.Data;
using Microsoft.EntityFrameworkCore;

namespace JournalApp;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;

        // DB init (safe to run at startup)
        _ = InitializeAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // This replaces: MainPage = new MainPage();
        return new Window(new MainPage());
    }

    private async Task InitializeAsync()
    {
        try
        {
            var factory = _services.GetRequiredService<IDbContextFactory<AppDbContext>>();
            await DbInitializer.InitializeAsync(factory);
        }
        catch
        {
            // keep simple for now
        }
    }
}
