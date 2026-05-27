using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fcg.Identity.Application.Abstractions.Authentication;
using Fcg.Identity.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Fcg.Identity.Infrastructure.Auth.Authentication;

public sealed class TokenProviderService : ITokenProviderService
{
    private readonly JwtSettings _settings;

    public TokenProviderService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string Generate(Guid userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
