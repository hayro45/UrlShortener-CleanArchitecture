using MediatR;

namespace UrlShortener.Application.Features.Urls.Commands.CreateShortUrl;

/// <summary>
/// Command to create a new shortened URL.
/// Follows CQRS pattern - Commands modify state.
/// </summary>
public sealed record CreateShortUrlCommand(string OriginalUrl) : IRequest<CreateShortUrlResponse>;

/// <summary>
/// Response DTO for the CreateShortUrl command.
/// </summary>
public sealed record CreateShortUrlResponse(
    Guid Id,
    string OriginalUrl,
    string ShortCode,
    DateTime CreatedAt
);
