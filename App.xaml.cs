using Journal_App.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Journal_App;

public partial class App : Application
{
    public App(IServiceProvider services)
    {
        InitializeComponent();

        // Initialize database
        Task.Run(async () =>
        {
            try
            {
                var factory = services.GetRequiredService<IDbContextFactory<AppDbContext>>();
                await DbInitializer.InitializeAsync(factory);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
            }
        });
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage())
        {
            Title = "Journal App"
        };
    }
}