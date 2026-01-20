using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Url entity using EF Core.
/// Implements the repository interface defined in the Domain layer.
/// </summary>
public sealed class UrlRepository : IUrlRepository
{
    private readonly ApplicationDbContext _context;

    public UrlRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Url?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        return await _context.Urls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Url url, CancellationToken cancellationToken = default)
    {
        await _context.Urls.AddAsync(url, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken = default)
    {
        return await _context.Urls
            .AsNoTracking()
            .AnyAsync(x => x.ShortCode == shortCode, cancellationToken);
    }
}
