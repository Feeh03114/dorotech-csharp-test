using Bookstore.Application.Abstractions;
using Bookstore.Application.Books;
using Bookstore.Application.Common;
using Bookstore.Application.Exceptions;
using Bookstore.Application.Services;
using Bookstore.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Bookstore.Tests;

public sealed class BookServiceTests
{
    [Fact]
    public async Task CreateAsync_creates_book_when_payload_is_unique()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var result = await service.CreateAsync(new CreateBookRequest(
            "Clean Code",
            "Robert C. Martin",
            "9780132350884",
            "Prentice Hall",
            2008,
            4), CancellationToken.None);

        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Clean Code");
        repository.Books.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateAsync_rejects_duplicate_name_and_author()
    {
        var repository = new FakeBookRepository();
        repository.Books.Add(new Book("Clean Code", "Robert C. Martin", null, null, 2008, 1));
        var service = new BookService(repository);

        var act = () => service.CreateAsync(new CreateBookRequest(
            " clean code ",
            "ROBERT C. MARTIN",
            null,
            null,
            2008,
            2), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("A book with the same name and author is already registered.");
    }

    [Fact]
    public async Task CreateAsync_rejects_duplicate_isbn()
    {
        var repository = new FakeBookRepository();
        repository.Books.Add(new Book("Domain-Driven Design", "Eric Evans", "9780321125217", null, 2003, 1));
        var service = new BookService(repository);

        var act = () => service.CreateAsync(new CreateBookRequest(
            "DDD Reference",
            "Eric Evans",
            "9780321125217",
            null,
            2003,
            1), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("A book with the same ISBN is already registered.");
    }

    [Fact]
    public async Task UpdateAsync_throws_not_found_when_book_does_not_exist()
    {
        var service = new BookService(new FakeBookRepository());

        var act = () => service.UpdateAsync(Guid.NewGuid(), new UpdateBookRequest(
            "Refactoring",
            "Martin Fowler",
            null,
            null,
            2018,
            3), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Book was not found.");
    }

    [Fact]
    public async Task SearchAsync_rejects_invalid_pagination()
    {
        var service = new BookService(new FakeBookRepository());

        var act = () => service.SearchAsync(new BookQueryParameters
        {
            PageNumber = 0,
            PageSize = 10
        }, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("PageNumber must be greater than zero.*");
    }

    private sealed class FakeBookRepository : IBookRepository
    {
        public List<Book> Books { get; } = new();

        public Task<PagedResult<Book>> SearchAsync(BookQueryParameters query, CancellationToken cancellationToken)
        {
            var items = Books
                .OrderBy(book => book.Name)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToArray();

            return Task.FromResult(new PagedResult<Book>(items, query.PageNumber, query.PageSize, Books.Count));
        }

        public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Books.SingleOrDefault(book => book.Id == id));
        }

        public Task<bool> ExistsByNameAndAuthorAsync(string name, string author, Guid? ignoredId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Books.Any(book =>
                book.Id != ignoredId &&
                string.Equals(book.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(book.Author.Trim(), author.Trim(), StringComparison.OrdinalIgnoreCase)));
        }

        public Task<bool> ExistsByIsbnAsync(string isbn, Guid? ignoredId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Books.Any(book =>
                book.Id != ignoredId &&
                string.Equals(book.Isbn, isbn.Trim(), StringComparison.OrdinalIgnoreCase)));
        }

        public Task AddAsync(Book book, CancellationToken cancellationToken)
        {
            Books.Add(book);
            return Task.CompletedTask;
        }

        public void Remove(Book book)
        {
            Books.Remove(book);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
