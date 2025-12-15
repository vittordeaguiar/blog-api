using BlogAPI.Domain.Entities;

namespace BlogAPI.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}