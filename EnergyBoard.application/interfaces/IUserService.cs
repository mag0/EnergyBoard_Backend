
using EnergyBoard.Application.DTOs.request.users;
using EnergyBoard.Application.DTOs.response.users;
using EnergyBoard.Domain.entities;

namespace EnergyBoard.Application.interfaces;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task UpdateAsync(Guid id, UpdateUserRequest request);
    Task DeleteAsync(Guid id);
}
