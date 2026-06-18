namespace Bookstore.Application.Books;

public sealed class BookQueryParameters
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Name { get; init; }
    public string? Author { get; init; }
    public string? Isbn { get; init; }
    public string? Publisher { get; init; }
    public int? PublicationYear { get; init; }
}
