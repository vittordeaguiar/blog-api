using BlogAPI.Domain.Interfaces;

namespace BlogAPI.Infrastructure.Services;

public class BCryptPasswordService : IPasswordService
{
    private const int WorkFactor = 12;

    public string Hash(string plainTextPassword) => BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: WorkFactor);

    public bool Verify(string plainTextPassword, string hashedPassword) => BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
}