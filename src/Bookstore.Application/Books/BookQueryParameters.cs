namespace Bookstore.Application.Books;

/// <summary>
/// Filtros e controles de paginação usados na consulta pública de livros.
/// </summary>
public sealed class BookQueryParameters
{
    /// <summary>
    /// Página desejada. O valor mínimo é 1.
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Quantidade de livros por página. O valor máximo aceito pela API é 100.
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Filtra livros que contenham este texto no nome.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Filtra livros que contenham este texto no autor.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Filtra livros que contenham este texto no ISBN.
    /// </summary>
    public string? Isbn { get; init; }

    /// <summary>
    /// Filtra livros que contenham este texto na editora.
    /// </summary>
    public string? Publisher { get; init; }

    /// <summary>
    /// Filtra livros publicados exatamente neste ano.
    /// </summary>
    public int? PublicationYear { get; init; }
}
