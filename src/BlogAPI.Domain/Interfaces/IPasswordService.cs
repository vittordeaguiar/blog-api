namespace BlogAPI.Domain.Interfaces;

public interface IPasswordService
{
    string Hash(string plainTextPassword);
    bool Verify(string plainTextPassword, string hashedPassword);
}