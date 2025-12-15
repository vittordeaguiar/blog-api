using BlogAPI.Domain.Entities;

namespace BlogAPI.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}