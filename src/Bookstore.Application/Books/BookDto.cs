namespace Bookstore.Application.Books;

public sealed record BookDto(
    Guid Id,
    string Name,
    string Author,
    string? Isbn,
    string? Publisher,
    int? PublicationYear,
    int Quantity,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
