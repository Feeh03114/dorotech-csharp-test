using Bookstore.Application.Books;
using Bookstore.Application.Common;
using Bookstore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    /// <summary>
    /// Consulta os livros disponíveis para o público.
    /// </summary>
    /// <remarks>
    /// A listagem sempre retorna os livros em ordem ascendente pelo nome. Use os filtros para localizar livros por nome, autor, ISBN, editora ou ano de publicação.
    /// </remarks>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<BookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<BookDto>>> SearchAsync(
        [FromQuery] BookQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.SearchAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Consulta um livro específico pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}", Name = "GetBookById")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _bookService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cadastra um novo livro no estoque.
    /// </summary>
    /// <remarks>
    /// Requer autenticação de administrador. A API rejeita livros duplicados pelo mesmo nome e autor ou pelo mesmo ISBN.
    /// </remarks>
    [HttpPost]
    [Authorize(Policy = "AdministratorOnly")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookDto>> CreateAsync(
        [FromBody] CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.CreateAsync(request, cancellationToken);
        _logger.LogInformation("Book {BookId} created.", result.Id);

        return CreatedAtRoute("GetBookById", new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza os dados de um livro existente.
    /// </summary>
    /// <remarks>
    /// Requer autenticação de administrador. A atualização também respeita a validação de duplicidade por nome e autor ou ISBN.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdministratorOnly")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookDto>> UpdateAsync(
        Guid id,
        [FromBody] UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _bookService.UpdateAsync(id, request, cancellationToken);
        _logger.LogInformation("Book {BookId} updated.", result.Id);

        return Ok(result);
    }

    /// <summary>
    /// Remove um livro do estoque.
    /// </summary>
    /// <remarks>
    /// Requer autenticação de administrador. Após a exclusão, consultas pelo mesmo identificador retornam 404.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdministratorOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _bookService.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Book {BookId} deleted.", id);

        return NoContent();
    }
}
