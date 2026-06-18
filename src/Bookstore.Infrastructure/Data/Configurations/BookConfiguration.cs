using Bookstore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookstore.Infrastructure.Data.Configurations;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");

        builder.HasKey(book => book.Id);

        builder.Property(book => book.Id)
            .HasColumnName("id");

        builder.Property(book => book.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(book => book.Author)
            .HasColumnName("author")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(book => book.Isbn)
            .HasColumnName("isbn")
            .HasMaxLength(20);

        builder.Property(book => book.Publisher)
            .HasColumnName("publisher")
            .HasMaxLength(120);

        builder.Property(book => book.PublicationYear)
            .HasColumnName("publication_year");

        builder.Property(book => book.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(book => book.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(book => book.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(book => book.Isbn)
            .IsUnique()
            .HasFilter("isbn IS NOT NULL");

        builder.HasIndex(book => new { book.Name, book.Author });
    }
}
