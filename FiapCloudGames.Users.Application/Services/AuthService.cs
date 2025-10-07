using FiapCloudGames.Users.Application.Interfaces;
using FiapCloudGames.Users.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FiapCloudGames.Users.Application.Services;

public class AuthService(IConfiguration configuration) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(User user)
    {
        string issuer = _configuration["Jwt:Issuer"]!;
        string audience = _configuration["Jwt:Audience"]!;
        string key = _configuration["Jwt:Secret"]!;

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Audience = audience,
            Issuer = issuer,
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature),
            Subject = new([
                new(nameof(user.UserId), user.UserId.ToString()!),
                new(nameof(user.Name), user.Name),
                new(ClaimTypes.Role, user.UserType.ToString())
            ])
        };

        Log.Information("Gerando Token JWT para o usuário {email}", user.Email);
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
