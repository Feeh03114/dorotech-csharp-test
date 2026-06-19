using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bookstore.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Bookstore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly AdminOptions _adminOptions;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IOptions<AdminOptions> adminOptions,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthController> logger)
    {
        _adminOptions = adminOptions.Value;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Autentica o administrador e retorna um token JWT para usar no Swagger.
    /// </summary>
    /// <remarks>
    /// Use o token retornado no botão Authorize do Swagger no formato: Bearer &lt;accessToken&gt;.
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        if (!IsValidAdmin(request))
        {
            _logger.LogWarning("Invalid administrator login attempt for username {Username}.", request.Username);
            return Unauthorized();
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
        var token = CreateToken(expiresAt);

        return Ok(new LoginResponse(token, expiresAt));
    }

    private bool IsValidAdmin(LoginRequest request)
    {
        return string.Equals(request.Username, _adminOptions.Username, StringComparison.Ordinal) &&
               string.Equals(request.Password, _adminOptions.Password, StringComparison.Ordinal);
    }

    private string CreateToken(DateTime expiresAt)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _adminOptions.Username),
            new Claim(ClaimTypes.Name, _adminOptions.Username),
            new Claim(ClaimTypes.Role, "Administrator")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// Credenciais do administrador usadas para gerar o token JWT.
/// </summary>
/// <param name="Username">Usuário administrador configurado na aplicação.</param>
/// <param name="Password">Senha do administrador configurada na aplicação.</param>
public sealed record LoginRequest(string Username, string Password);

/// <summary>
/// Resultado do login com o token de acesso e sua data de expiração.
/// </summary>
/// <param name="AccessToken">Token JWT usado no header Authorization como Bearer token.</param>
/// <param name="ExpiresAt">Data e hora em UTC em que o token expira.</param>
public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt);
