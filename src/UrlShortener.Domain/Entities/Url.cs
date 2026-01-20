namespace UrlShortener.Domain.Entities;

/// <summary>
/// Represents a shortened URL entity in the domain.
/// This is an aggregate root that encapsulates URL shortening business logic.
/// </summary>
public sealed class Url
{
    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// Ensures entity invariants are always satisfied.
    /// </summary>
    private Url(Guid id, string originalUrl, string shortCode, DateTime createdAt)
    {
        Id = id;
        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Unique identifier for the URL entity.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// The original long URL that was shortened.
    /// </summary>
    public string OriginalUrl { get; private set; }

    /// <summary>
    /// The generated unique short code for the URL.
    /// </summary>
    public string ShortCode { get; private set; }

    /// <summary>
    /// Timestamp when the URL was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Factory method to create a new Url entity with proper initialization.
    /// </summary>
    /// <param name="originalUrl">The original URL to shorten.</param>
    /// <param name="shortCode">The unique short code for this URL.</param>
    /// <returns>A new Url entity instance.</returns>
    public static Url Create(string originalUrl, string shortCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalUrl, nameof(originalUrl));
        ArgumentException.ThrowIfNullOrWhiteSpace(shortCode, nameof(shortCode));

        return new Url(
            Guid.NewGuid(),
            originalUrl,
            shortCode,
            DateTime.UtcNow
        );
    }

    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value
    private Url() { }
#pragma warning restore CS8618
}
