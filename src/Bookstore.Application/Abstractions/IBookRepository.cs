using Bookstore.Application.Books;
using Bookstore.Application.Common;
using Bookstore.Domain.Entities;

namespace Bookstore.Application.Abstractions;

public interface IBookRepository
{
    Task<PagedResult<Book>> SearchAsync(BookQueryParameters query, CancellationToken cancellationToken);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAndAuthorAsync(string name, string author, Guid? ignoredId, CancellationToken cancellationToken);
    Task<bool> ExistsByIsbnAsync(string isbn, Guid? ignoredId, CancellationToken cancellationToken);
    Task AddAsync(Book book, CancellationToken cancellationToken);
    void Remove(Book book);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
