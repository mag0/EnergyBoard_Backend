
using EnergyBoard.Application.DTOs.request.users;

namespace EnergyBoard.Application.interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(LoginUserRequest request);
    Task RegisterAsync(CreateUserRequest request);
}
