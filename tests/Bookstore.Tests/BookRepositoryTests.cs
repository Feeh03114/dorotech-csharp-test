using Bookstore.Application.Books;
using Bookstore.Domain.Entities;
using Bookstore.Infrastructure.Data;
using Bookstore.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bookstore.Tests;

public sealed class BookRepositoryTests
{
    [Fact]
    public async Task SearchAsync_returns_books_ordered_by_name()
    {
        await using var context = CreateContext();
        context.Books.AddRange(
            new Book("Refactoring", "Martin Fowler", null, null, 2018, 2),
            new Book("Clean Code", "Robert C. Martin", null, null, 2008, 3),
            new Book("Domain-Driven Design", "Eric Evans", null, null, 2003, 1));
        await context.SaveChangesAsync();
        var repository = new BookRepository(context);

        var result = await repository.SearchAsync(new BookQueryParameters(), CancellationToken.None);

        result.Items.Select(book => book.Name).Should().Equal(
            "Clean Code",
            "Domain-Driven Design",
            "Refactoring");
    }

    [Fact]
    public async Task SearchAsync_filters_by_author_and_isbn()
    {
        await using var context = CreateContext();
        context.Books.AddRange(
            new Book("Clean Code", "Robert C. Martin", "9780132350884", null, 2008, 3),
            new Book("Refactoring", "Martin Fowler", "9780134757599", null, 2018, 2));
        await context.SaveChangesAsync();
        var repository = new BookRepository(context);

        var result = await repository.SearchAsync(new BookQueryParameters
        {
            Author = "martin",
            Isbn = "7599"
        }, CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items.Single().Name.Should().Be("Refactoring");
    }

    [Fact]
    public async Task ExistsByNameAndAuthorAsync_ignores_current_book_id()
    {
        await using var context = CreateContext();
        var book = new Book("Clean Code", "Robert C. Martin", null, null, 2008, 3);
        context.Books.Add(book);
        await context.SaveChangesAsync();
        var repository = new BookRepository(context);

        var existsForSameBook = await repository.ExistsByNameAndAuthorAsync(
            "clean code",
            "robert c. martin",
            book.Id,
            CancellationToken.None);

        existsForSameBook.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByIsbnAsync_detects_duplicate_isbn()
    {
        await using var context = CreateContext();
        context.Books.Add(new Book("Clean Code", "Robert C. Martin", "9780132350884", null, 2008, 3));
        await context.SaveChangesAsync();
        var repository = new BookRepository(context);

        var exists = await repository.ExistsByIsbnAsync("9780132350884", null, CancellationToken.None);

        exists.Should().BeTrue();
    }

    private static BookstoreDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BookstoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BookstoreDbContext(options);
    }
}
