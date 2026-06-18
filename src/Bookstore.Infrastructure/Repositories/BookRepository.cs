using Bookstore.Application.Abstractions;
using Bookstore.Application.Books;
using Bookstore.Application.Common;
using Bookstore.Domain.Entities;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Infrastructure.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly BookstoreDbContext _context;

    public BookRepository(BookstoreDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Book>> SearchAsync(BookQueryParameters query, CancellationToken cancellationToken)
    {
        var books = _context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Name))
            books = books.Where(book => book.Name.ToLower().Contains(query.Name.Trim().ToLower()));

        if (!string.IsNullOrWhiteSpace(query.Author))
            books = books.Where(book => book.Author.ToLower().Contains(query.Author.Trim().ToLower()));

        if (!string.IsNullOrWhiteSpace(query.Isbn))
            books = books.Where(book => book.Isbn != null && book.Isbn.ToLower().Contains(query.Isbn.Trim().ToLower()));

        if (!string.IsNullOrWhiteSpace(query.Publisher))
            books = books.Where(book => book.Publisher != null && book.Publisher.ToLower().Contains(query.Publisher.Trim().ToLower()));

        if (query.PublicationYear is not null)
            books = books.Where(book => book.PublicationYear == query.PublicationYear);

        var totalItems = await books.CountAsync(cancellationToken);
        var items = await books
            .OrderBy(book => book.Name)
            .ThenBy(book => book.Author)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);

        return new PagedResult<Book>(items, query.PageNumber, query.PageSize, totalItems);
    }

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Books.SingleOrDefaultAsync(book => book.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNameAndAuthorAsync(string name, string author, Guid? ignoredId, CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim().ToLower();
        var normalizedAuthor = author.Trim().ToLower();

        return _context.Books.AnyAsync(book =>
            book.Id != ignoredId &&
            book.Name.ToLower() == normalizedName &&
            book.Author.ToLower() == normalizedAuthor,
            cancellationToken);
    }

    public Task<bool> ExistsByIsbnAsync(string isbn, Guid? ignoredId, CancellationToken cancellationToken)
    {
        var normalizedIsbn = isbn.Trim().ToLower();

        return _context.Books.AnyAsync(book =>
            book.Id != ignoredId &&
            book.Isbn != null &&
            book.Isbn.ToLower() == normalizedIsbn,
            cancellationToken);
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken)
    {
        await _context.Books.AddAsync(book, cancellationToken);
    }

    public void Remove(Book book)
    {
        _context.Books.Remove(book);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
