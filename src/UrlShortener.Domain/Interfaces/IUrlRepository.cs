using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces;

/// <summary>
/// Repository interface for Url entity operations.
/// Defined in Domain layer to enforce Dependency Inversion Principle.
/// Implementation resides in Infrastructure layer.
/// </summary>
public interface IUrlRepository
{
    /// <summary>
    /// Retrieves a URL entity by its short code.
    /// </summary>
    /// <param name="shortCode">The short code to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL entity if found; otherwise, null.</returns>
    Task<Url?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new URL entity to the repository.
    /// </summary>
    /// <param name="url">The URL entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Url url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a short code already exists in the repository.
    /// </summary>
    /// <param name="shortCode">The short code to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the short code exists; otherwise, false.</returns>
    Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken = default);
}
