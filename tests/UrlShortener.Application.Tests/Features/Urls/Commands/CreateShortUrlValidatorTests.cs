using UrlShortener.Application.Features.Urls.Commands.CreateShortUrl;

namespace UrlShortener.Application.Tests.Features.Urls.Commands;

/// <summary>
/// Unit tests for the CreateShortUrlValidator.
/// Tests URL validation rules.
/// </summary>
public sealed class CreateShortUrlValidatorTests
{
    private readonly CreateShortUrlValidator _validator;

    public CreateShortUrlValidatorTests()
    {
        _validator = new CreateShortUrlValidator();
    }

    [Theory]
    [InlineData("https://www.example.com")]
    [InlineData("http://example.com")]
    [InlineData("https://example.com/path/to/resource")]
    [InlineData("https://example.com?query=value")]
    [InlineData("https://subdomain.example.com")]
    public async Task Validate_ValidUrl_ShouldNotHaveErrors(string url)
    {
        // Arrange
        var command = new CreateShortUrlCommand(url);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_EmptyUrl_ShouldHaveError(string url)
    {
        // Arrange
        var command = new CreateShortUrlCommand(url);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("example.com")]
    [InlineData("ftp://example.com")]
    [InlineData("//example.com")]
    [InlineData("www.example.com")]
    public async Task Validate_InvalidUrlFormat_ShouldHaveError(string url)
    {
        // Arrange
        var command = new CreateShortUrlCommand(url);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => 
            e.ErrorMessage.Contains("valid absolute URI") || 
            e.ErrorMessage.Contains("http or https"));
    }
}
