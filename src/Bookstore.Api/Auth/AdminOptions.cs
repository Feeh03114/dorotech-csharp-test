namespace Bookstore.Api.Auth;

public sealed class AdminOptions
{
    public const string SectionName = "Admin";

    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
