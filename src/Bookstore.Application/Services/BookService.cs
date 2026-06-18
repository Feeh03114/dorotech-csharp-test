using Bookstore.Application.Abstractions;
using Bookstore.Application.Books;
using Bookstore.Application.Common;
using Bookstore.Application.Exceptions;
using Bookstore.Domain.Entities;

namespace Bookstore.Application.Services;

public sealed class BookService : IBookService
{
    private const int MaxPageSize = 100;
    private readonly IBookRepository _repository;

    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<BookDto>> SearchAsync(BookQueryParameters query, CancellationToken cancellationToken)
    {
        ValidateQuery(query);

        var result = await _repository.SearchAsync(query, cancellationToken);
        return new PagedResult<BookDto>(
            result.Items.Select(Map).ToArray(),
            result.PageNumber,
            result.PageSize,
            result.TotalItems);
    }

    public async Task<BookDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        return book is null ? throw new NotFoundException("Book was not found.") : Map(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken)
    {
        await EnsureUniqueAsync(request.Name, request.Author, request.Isbn, null, cancellationToken);

        var book = new Book(
            request.Name,
            request.Author,
            request.Isbn,
            request.Publisher,
            request.PublicationYear,
            request.Quantity);

        await _repository.AddAsync(book, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(book);
    }

    public async Task<BookDto> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        if (book is null)
            throw new NotFoundException("Book was not found.");

        await EnsureUniqueAsync(request.Name, request.Author, request.Isbn, id, cancellationToken);

        book.Update(
            request.Name,
            request.Author,
            request.Isbn,
            request.Publisher,
            request.PublicationYear,
            request.Quantity);

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(book);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(id, cancellationToken);
        if (book is null)
            throw new NotFoundException("Book was not found.");

        _repository.Remove(book);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateQuery(BookQueryParameters query)
    {
        if (query.PageNumber <= 0)
            throw new ArgumentException("PageNumber must be greater than zero.", nameof(query));

        if (query.PageSize <= 0)
            throw new ArgumentException("PageSize must be greater than zero.", nameof(query));

        if (query.PageSize > MaxPageSize)
            throw new ArgumentException($"PageSize must be less than or equal to {MaxPageSize}.", nameof(query));
    }

    private async Task EnsureUniqueAsync(
        string name,
        string author,
        string? isbn,
        Guid? ignoredId,
        CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByNameAndAuthorAsync(name, author, ignoredId, cancellationToken))
            throw new ConflictException("A book with the same name and author is already registered.");

        if (!string.IsNullOrWhiteSpace(isbn) &&
            await _repository.ExistsByIsbnAsync(isbn, ignoredId, cancellationToken))
        {
            throw new ConflictException("A book with the same ISBN is already registered.");
        }
    }

    private static BookDto Map(Book book)
    {
        return new BookDto(
            book.Id,
            book.Name,
            book.Author,
            book.Isbn,
            book.Publisher,
            book.PublicationYear,
            book.Quantity,
            book.CreatedAt,
            book.UpdatedAt);
    }
}
