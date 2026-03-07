using DotNetEnv;
using EnergyBoard.Application.interfaces;
using EnergyBoard.Domain.entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnergyBoard.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateToken(Guid userId)
    {
        var jwtKey = Env.GetString("JWT_KEY");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

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