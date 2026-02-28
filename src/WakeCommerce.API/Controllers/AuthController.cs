using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WakeCommerce.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gera um token JWT para uso em desenvolvimento/testes. Em produção use um identity provider.
    /// </summary>
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public IActionResult GetToken([FromBody] TokenRequest request)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "ChaveDesenvolvimentoMinimo32Caracteres!";
        var issuer = _configuration["Jwt:Issuer"] ?? "WakeCommerce.API";
        var audience = _configuration["Jwt:Audience"] ?? "WakeCommerce.Client";
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var min) ? min : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request?.UserName ?? "api-user"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new TokenResponse { Token = tokenString, ExpiresInMinutes = expirationMinutes });
    }

    public class TokenRequest
    {
        public string? UserName { get; set; }
    }

    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
    }
}
