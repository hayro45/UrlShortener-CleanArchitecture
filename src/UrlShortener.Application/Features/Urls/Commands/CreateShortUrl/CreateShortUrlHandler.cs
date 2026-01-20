using MediatR;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Features.Urls.Commands.CreateShortUrl;

/// <summary>
/// Handler for the CreateShortUrl command.
/// Implements the use case for shortening a URL.
/// </summary>
public sealed class CreateShortUrlHandler : IRequestHandler<CreateShortUrlCommand, CreateShortUrlResponse>
{
    private readonly IUrlRepository _urlRepository;
    private const int ShortCodeLength = 7;
    private const string ShortCodeCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public CreateShortUrlHandler(IUrlRepository urlRepository)
    {
        _urlRepository = urlRepository ?? throw new ArgumentNullException(nameof(urlRepository));
    }

    public async Task<CreateShortUrlResponse> Handle(
        CreateShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        // Generate a unique short code
        var shortCode = await GenerateUniqueShortCodeAsync(cancellationToken);

        // Create the domain entity using the factory method
        var url = Url.Create(request.OriginalUrl, shortCode);

        // Persist the entity
        await _urlRepository.AddAsync(url, cancellationToken);

        // Return the response DTO
        return new CreateShortUrlResponse(
            url.Id,
            url.OriginalUrl,
            url.ShortCode,
            url.CreatedAt
        );
    }

    /// <summary>
    /// Generates a unique short code that doesn't exist in the repository.
    /// Uses a retry mechanism to ensure uniqueness.
    /// </summary>
    private async Task<string> GenerateUniqueShortCodeAsync(CancellationToken cancellationToken)
    {
        const int maxAttempts = 10;
        var random = Random.Shared;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var shortCode = GenerateRandomShortCode(random);

            if (!await _urlRepository.ShortCodeExistsAsync(shortCode, cancellationToken))
            {
                return shortCode;
            }
        }

        throw new InvalidOperationException(
            $"Failed to generate a unique short code after {maxAttempts} attempts.");
    }

    /// <summary>
    /// Generates a random short code of specified length.
    /// </summary>
    private static string GenerateRandomShortCode(Random random)
    {
        return string.Create(ShortCodeLength, random, static (span, rng) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = ShortCodeCharacters[rng.Next(ShortCodeCharacters.Length)];
            }
        });
    }
}
