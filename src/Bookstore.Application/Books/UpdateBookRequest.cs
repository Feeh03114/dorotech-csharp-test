namespace Bookstore.Application.Books;

public sealed record UpdateBookRequest(
    string Name,
    string Author,
    string? Isbn,
    string? Publisher,
    int? PublicationYear,
    int Quantity);
