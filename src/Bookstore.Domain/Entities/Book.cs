using Bookstore.Domain.Exceptions;

namespace Bookstore.Domain.Entities;

public sealed class Book
{
    private Book()
    {
        Name = string.Empty;
        Author = string.Empty;
    }

    public Book(
        string name,
        string author,
        string? isbn,
        string? publisher,
        int? publicationYear,
        int quantity)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        SetDetails(name, author, isbn, publisher, publicationYear, quantity);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string? Isbn { get; private set; }
    public string? Publisher { get; private set; }
    public int? PublicationYear { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public void Update(
        string name,
        string author,
        string? isbn,
        string? publisher,
        int? publicationYear,
        int quantity)
    {
        SetDetails(name, author, isbn, publisher, publicationYear, quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetDetails(
        string name,
        string author,
        string? isbn,
        string? publisher,
        int? publicationYear,
        int quantity)
    {
        Name = NormalizeRequired(name, nameof(Name), 150);
        Author = NormalizeRequired(author, nameof(Author), 150);
        Isbn = NormalizeOptional(isbn, 20);
        Publisher = NormalizeOptional(publisher, 120);
        PublicationYear = ValidatePublicationYear(publicationYear);
        Quantity = quantity >= 0
            ? quantity
            : throw new DomainException("Quantity must be greater than or equal to zero.");
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{fieldName} is required.");

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new DomainException($"{fieldName} must have at most {maxLength} characters.");

        return normalized;
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new DomainException($"Value must have at most {maxLength} characters.");

        return normalized;
    }

    private static int? ValidatePublicationYear(int? publicationYear)
    {
        if (publicationYear is null)
            return null;

        var maxYear = DateTime.UtcNow.Year + 1;
        if (publicationYear < 1450 || publicationYear > maxYear)
            throw new DomainException($"Publication year must be between 1450 and {maxYear}.");

        return publicationYear;
    }
}
