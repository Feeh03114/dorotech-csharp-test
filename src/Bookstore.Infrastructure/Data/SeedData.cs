using Bookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(BookstoreDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Books.AnyAsync(cancellationToken))
            return;

        context.Books.AddRange(
            new Book("Clean Code", "Robert C. Martin", "9780132350884", "Prentice Hall", 2008, 8),
            new Book("Domain-Driven Design", "Eric Evans", "9780321125217", "Addison-Wesley", 2003, 5),
            new Book("Refactoring", "Martin Fowler", "9780134757599", "Addison-Wesley", 2018, 6),
            new Book("The Pragmatic Programmer", "David Thomas", "9780135957059", "Addison-Wesley", 2019, 7),
            new Book("Patterns of Enterprise Application Architecture", "Martin Fowler", "9780321127426", "Addison-Wesley", 2002, 4));

        await context.SaveChangesAsync(cancellationToken);
    }
}
