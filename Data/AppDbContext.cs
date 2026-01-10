using Microsoft.EntityFrameworkCore;
using JournalApp.Models;

namespace JournalApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<Mood> Moods => Set<Mood>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<EntryTag> EntryTags => Set<EntryTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One entry per day rule (enforced at DB level too)
        modelBuilder.Entity<JournalEntry>()
            .HasIndex(e => e.EntryDate)
            .IsUnique();

        // Many-to-many via join table
        modelBuilder.Entity<EntryTag>()
            .HasKey(et => new { et.EntryId, et.TagId });

        modelBuilder.Entity<EntryTag>()
            .HasOne(et => et.Entry)
            .WithMany(e => e.EntryTags)
            .HasForeignKey(et => et.EntryId);

        modelBuilder.Entity<EntryTag>()
            .HasOne(et => et.Tag)
            .WithMany()
            .HasForeignKey(et => et.TagId);

        // Basic constraints
        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();
    }
}
