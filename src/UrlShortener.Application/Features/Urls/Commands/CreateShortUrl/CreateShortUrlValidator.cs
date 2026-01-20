using FluentValidation;

namespace UrlShortener.Application.Features.Urls.Commands.CreateShortUrl;

/// <summary>
/// Validator for CreateShortUrlCommand using FluentValidation.
/// Ensures the URL is valid before processing.
/// </summary>
public sealed class CreateShortUrlValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlValidator()
    {
        RuleFor(x => x.OriginalUrl)
            .NotEmpty()
            .WithMessage("URL is required.")
            .Must(BeAValidUrl)
            .WithMessage("URL must be a valid absolute URI with http or https scheme.");
    }

    /// <summary>
    /// Validates that the provided string is a valid absolute URL.
    /// </summary>
    private static bool BeAValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
