using Moq;
using UrlShortener.Application.Features.Urls.Commands.CreateShortUrl;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Tests.Features.Urls.Commands;

/// <summary>
/// Unit tests for the CreateShortUrlHandler.
/// Uses xUnit for testing and Moq for mocking dependencies.
/// </summary>
public sealed class CreateShortUrlHandlerTests
{
    private readonly Mock<IUrlRepository> _urlRepositoryMock;
    private readonly CreateShortUrlHandler _handler;

    public CreateShortUrlHandlerTests()
    {
        _urlRepositoryMock = new Mock<IUrlRepository>();
        _handler = new CreateShortUrlHandler(_urlRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidUrl_ReturnsSuccessfulResponse()
    {
        // Arrange
        var command = new CreateShortUrlCommand("https://www.example.com");
        
        _urlRepositoryMock
            .Setup(x => x.ShortCodeExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _urlRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Url>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.OriginalUrl, result.OriginalUrl);
        Assert.NotEmpty(result.ShortCode);
        Assert.Equal(7, result.ShortCode.Length);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.True(result.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_ValidUrl_CallsRepositoryAddAsync()
    {
        // Arrange
        var command = new CreateShortUrlCommand("https://www.example.com");
        
        _urlRepositoryMock
            .Setup(x => x.ShortCodeExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _urlRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Url>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _urlRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Url>(u => u.OriginalUrl == command.OriginalUrl), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShortCodeCollision_GeneratesNewCode()
    {
        // Arrange
        var command = new CreateShortUrlCommand("https://www.example.com");
        var callCount = 0;
        
        // First call returns true (collision), second returns false
        _urlRepositoryMock
            .Setup(x => x.ShortCodeExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount == 1; // First call returns true (collision)
            });
        
        _urlRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Url>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, callCount); // Should have checked twice
        _urlRepositoryMock.Verify(
            x => x.ShortCodeExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_GeneratedShortCode_ContainsOnlyValidCharacters()
    {
        // Arrange
        var command = new CreateShortUrlCommand("https://www.example.com");
        const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        _urlRepositoryMock
            .Setup(x => x.ShortCodeExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _urlRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Url>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.All(result.ShortCode, c => Assert.Contains(c, validCharacters));
    }

    [Fact]
    public async Task Handle_NullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateShortUrlHandler(null!));
    }

    [Fact]
    public async Task Handle_MultipleUrls_GeneratesUniqueShortCodes()
    {
        // Arrange
        var command1 = new CreateShortUrlCommand("https://www.example1.com");
        var command2 = new CreateShortUrlCommand("https://www.example2.com");
        
        _urlRepositoryMock
            .Setup(x => x.ShortCodeExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _urlRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Url>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        // Assert
        Assert.NotEqual(result1.ShortCode, result2.ShortCode);
        Assert.NotEqual(result1.Id, result2.Id);
    }
}
