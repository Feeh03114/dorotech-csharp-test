using Bookstore.Application.Books;
using Bookstore.Application.Common;

namespace Bookstore.Application.Services;

public interface IBookService
{
    Task<PagedResult<BookDto>> SearchAsync(BookQueryParameters query, CancellationToken cancellationToken);
    Task<BookDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken);
    Task<BookDto> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
