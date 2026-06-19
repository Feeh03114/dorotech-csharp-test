namespace Bookstore.Application.Books;

/// <summary>
/// Dados usados para substituir as informações de um livro já cadastrado.
/// </summary>
/// <param name="Name">Novo nome do livro. Obrigatório.</param>
/// <param name="Author">Autor principal do livro. Obrigatório.</param>
/// <param name="Isbn">ISBN do livro. Quando informado, não pode se repetir em outro livro.</param>
/// <param name="Publisher">Editora do livro.</param>
/// <param name="PublicationYear">Ano de publicação do livro.</param>
/// <param name="Quantity">Quantidade de exemplares em estoque. Deve ser zero ou maior.</param>
public sealed record UpdateBookRequest(
    string Name,
    string Author,
    string? Isbn,
    string? Publisher,
    int? PublicationYear,
    int Quantity);
