using BlogAPI.Domain.Exceptions;
using BlogAPI.Domain.Validators;

namespace BlogAPI.Domain.Entities;

public class User
{
    protected User()
    {
        Name = null!;
        Email = null!;
        PasswordHash = null!;
        Role = null!;
    }

    public User(string name, string email, string passwordHash, string role)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Id = Guid.NewGuid();

        Validate();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }

    private void Validate()
    {
        var validator = new UserValidator();
        var result = validator.Validate(this);

        if (result.IsValid) return;

        var errorMessage = result.Errors.FirstOrDefault()?.ErrorMessage;
        if (errorMessage != null) throw new DomainException(errorMessage);
    }
}