
using EnergyBoard.Domain.entities;

namespace EnergyBoard.Domain.interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task UpdateAsync(User user);
}
