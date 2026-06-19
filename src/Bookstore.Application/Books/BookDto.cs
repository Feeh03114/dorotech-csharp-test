namespace Bookstore.Application.Books;

/// <summary>
/// Livro retornado pela API para consulta pública ou operações de administrador.
/// </summary>
/// <param name="Id">Identificador único do livro.</param>
/// <param name="Name">Nome do livro, usado também na ordenação padrão da listagem.</param>
/// <param name="Author">Autor principal do livro.</param>
/// <param name="Isbn">ISBN do livro, quando informado.</param>
/// <param name="Publisher">Editora responsável pela publicação, quando informada.</param>
/// <param name="PublicationYear">Ano de publicação, quando informado.</param>
/// <param name="Quantity">Quantidade de exemplares disponíveis no estoque.</param>
/// <param name="CreatedAt">Data e hora em UTC em que o livro foi cadastrado.</param>
/// <param name="UpdatedAt">Data e hora em UTC da última alteração, quando houver.</param>
public sealed record BookDto(
    Guid Id,
    string Name,
    string Author,
    string? Isbn,
    string? Publisher,
    int? PublicationYear,
    int Quantity,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
