using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for the Url entity.
/// Configures table name, columns, indexes, and constraints.
/// </summary>
public sealed class UrlConfiguration : IEntityTypeConfiguration<Url>
{
    public void Configure(EntityTypeBuilder<Url> builder)
    {
        // Table name (PostgreSQL convention: lowercase with underscores)
        builder.ToTable("urls");

        // Primary key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.OriginalUrl)
            .HasColumnName("original_url")
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.ShortCode)
            .HasColumnName("short_code")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Index for fast short code lookups
        builder.HasIndex(x => x.ShortCode)
            .IsUnique()
            .HasDatabaseName("ix_urls_short_code");
    }
}
