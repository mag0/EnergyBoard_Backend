using EnergyBoard.Application.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnergyBoard.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _jwtKey;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _jwtKey = configuration["JWT_KEY"]
            ?? throw new Exception("JWT_KEY no configurado");
    }

    public string GenerateToken(Guid userId)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: "EnergyBoard",
            audience: "EnergyBoardUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}