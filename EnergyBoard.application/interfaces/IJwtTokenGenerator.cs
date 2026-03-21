
namespace EnergyBoard.Application.interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId);
}
