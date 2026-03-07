using EnergyBoard.Application.DTOs.request.users;
using EnergyBoard.Application.interfaces;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;

namespace EnergyBoard.Application.services;
public class AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtGenerator) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtGenerator = jwtGenerator;

    public async Task<string> LoginAsync(LoginUserRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        return _jwtGenerator.GenerateToken(user.Id);
    }

    public async Task RegisterAsync(CreateUserRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
            throw new ArgumentException("Email already registered");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }
}
