using AutoMapper;
using EnergyBoard.Application.DTOs.request.users;
using EnergyBoard.Application.DTOs.response.users;
using EnergyBoard.Application.interfaces;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;

namespace EnergyBoard.Application.services;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await GetExistingUser(id);
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => _mapper.Map<UserResponse>(u));
    }

    public async Task UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await GetExistingUser(id);

        if (!string.IsNullOrWhiteSpace(request.Name))
            user.Name = request.Name;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            if (request.Password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters long");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetExistingUser(id);

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
    }

    private async Task<User> GetExistingUser(Guid id)
    {
        return await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found");
    }
}