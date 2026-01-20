using MediatR;

namespace UrlShortener.Application.Features.Urls.Queries.GetOriginalUrl;

/// <summary>
/// Query to retrieve the original URL by its short code.
/// Follows CQRS pattern - Queries do not modify state.
/// </summary>
public sealed record GetOriginalUrlQuery(string ShortCode) : IRequest<GetOriginalUrlResponse?>;

/// <summary>
/// Response DTO for the GetOriginalUrl query.
/// </summary>
public sealed record GetOriginalUrlResponse(
    Guid Id,
    string OriginalUrl,
    string ShortCode,
    DateTime CreatedAt
);
