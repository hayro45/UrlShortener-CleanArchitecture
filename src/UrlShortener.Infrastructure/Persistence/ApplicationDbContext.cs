using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Persistence.Configurations;

namespace UrlShortener.Infrastructure.Persistence;

/// <summary>
/// EF Core Database Context for the URL Shortener application.
/// Represents a session with the database.
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet for Url entities.
    /// </summary>
    public DbSet<Url> Urls => Set<Url>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new UrlConfiguration());
    }
}
