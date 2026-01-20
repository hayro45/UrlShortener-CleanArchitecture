namespace UrlShortener.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// Results in HTTP 404 response.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key)
        : base($"{name} with key '{key}' was not found.")
    {
    }
}

/// <summary>
/// Exception thrown when a resource conflict occurs.
/// Results in HTTP 409 response.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
