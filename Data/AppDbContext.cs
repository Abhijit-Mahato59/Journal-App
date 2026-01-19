using Microsoft.EntityFrameworkCore;
using Journal_App.Data.Models;

namespace Journal_App.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<AppSettings> AppSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<JournalEntry>()
            .HasIndex(e => e.Date)
            .IsUnique();

        modelBuilder.Entity<AppSettings>().HasData(new AppSettings
        {
            Id = 1,
            IsDarkMode = false
        });
    }
}