using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Features.Urls.Commands.CreateShortUrl;
using UrlShortener.Application.Features.Urls.Queries.GetOriginalUrl;

namespace UrlShortener.Api.Controllers;

/// <summary>
/// API Controller for URL shortening operations.
/// Follows REST conventions and uses MediatR for CQRS dispatch.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class UrlsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UrlsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Creates a shortened URL from a long URL.
    /// </summary>
    /// <param name="request">The request containing the original URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created short URL details.</returns>
    /// <response code="201">Short URL created successfully.</response>
    /// <response code="400">Invalid URL format.</response>
    /// <response code="429">Rate limit exceeded.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateShortUrlResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Create(
        [FromBody] CreateShortUrlRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateShortUrlCommand(request.OriginalUrl);
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetByShortCode),
            new { shortCode = result.ShortCode },
            result);
    }

    /// <summary>
    /// Retrieves the original URL by its short code.
    /// </summary>
    /// <param name="shortCode">The short code to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The original URL details.</returns>
    /// <response code="200">URL found.</response>
    /// <response code="404">Short code not found.</response>
    [HttpGet("{shortCode}")]
    [ProducesResponseType(typeof(GetOriginalUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByShortCode(
        [FromRoute] string shortCode,
        CancellationToken cancellationToken)
    {
        var query = new GetOriginalUrlQuery(shortCode);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Short code '{shortCode}' was not found."
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Redirects to the original URL using the short code.
    /// This is the primary URL shortener functionality.
    /// </summary>
    /// <param name="shortCode">The short code to redirect.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Redirect to original URL.</returns>
    /// <response code="302">Redirecting to original URL.</response>
    /// <response code="404">Short code not found.</response>
    [HttpGet("/r/{shortCode}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Redirect(
        [FromRoute] string shortCode,
        CancellationToken cancellationToken)
    {
        var query = new GetOriginalUrlQuery(shortCode);
        var result = await _mediator.Send(query, cancellationToken);

        if (result is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Short code '{shortCode}' was not found."
            });
        }

        // 302 Found - Temporary redirect (allows tracking)
        // Use RedirectPermanent (301) if you don't need to track clicks
        return Redirect(result.OriginalUrl);
    }
}

/// <summary>
/// Request DTO for creating a short URL.
/// </summary>
public sealed record CreateShortUrlRequest(string OriginalUrl);
