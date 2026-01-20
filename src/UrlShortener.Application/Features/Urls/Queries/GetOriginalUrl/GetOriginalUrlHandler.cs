using MediatR;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Features.Urls.Queries.GetOriginalUrl;

/// <summary>
/// Handler for the GetOriginalUrl query.
/// Implements the use case for retrieving the original URL.
/// </summary>
public sealed class GetOriginalUrlHandler : IRequestHandler<GetOriginalUrlQuery, GetOriginalUrlResponse?>
{
    private readonly IUrlRepository _urlRepository;

    public GetOriginalUrlHandler(IUrlRepository urlRepository)
    {
        _urlRepository = urlRepository ?? throw new ArgumentNullException(nameof(urlRepository));
    }

    public async Task<GetOriginalUrlResponse?> Handle(
        GetOriginalUrlQuery request,
        CancellationToken cancellationToken)
    {
        var url = await _urlRepository.GetByShortCodeAsync(request.ShortCode, cancellationToken);

        if (url is null)
        {
            return null;
        }

        return new GetOriginalUrlResponse(
            url.Id,
            url.OriginalUrl,
            url.ShortCode,
            url.CreatedAt
        );
    }
}
