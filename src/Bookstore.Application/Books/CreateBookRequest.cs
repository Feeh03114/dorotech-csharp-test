namespace Bookstore.Application.Books;

/// <summary>
/// Dados necessários para cadastrar um novo livro no estoque.
/// </summary>
/// <param name="Name">Nome do livro. Obrigatório.</param>
/// <param name="Author">Autor principal do livro. Obrigatório.</param>
/// <param name="Isbn">ISBN do livro. Quando informado, não pode se repetir.</param>
/// <param name="Publisher">Editora do livro.</param>
/// <param name="PublicationYear">Ano de publicação do livro.</param>
/// <param name="Quantity">Quantidade de exemplares em estoque. Deve ser zero ou maior.</param>
public sealed record CreateBookRequest(
    string Name,
    string Author,
    string? Isbn,
    string? Publisher,
    int? PublicationYear,
    int Quantity);
