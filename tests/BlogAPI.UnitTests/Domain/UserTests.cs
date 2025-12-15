using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Exceptions;
using FluentAssertions;

namespace BlogAPI.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_Should_CreateUser_When_DataIsValid()
    {
        const string name = "Vittor";
        const string email = "vittor@developer.com";
        const string passwordHash = "hash_seguro_123";
        const string role = "Admin";

        var user = new User(name, email, passwordHash, role);

        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty(); // O ‘ID’ deve ser gerado automaticamente
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Constructor_Should_ThrowException_When_NameIsInvalid(string? invalidName)
    {
        var act = () =>
        {
            var user = new User(invalidName ?? string.Empty, "email@valido.com", "hash", "Author");
        };

        act.Should().Throw<DomainException>().WithMessage("Name cannot be empty");
    }

    [Theory]
    [InlineData("email_invalido")]
    [InlineData("email@sem_ponto")]
    [InlineData("")]
    public void Constructor_Should_ThrowException_When_EmailIsInvalid(string invalidEmail)
    {
        var act = () =>
        {
            var user = new User("Vittor", invalidEmail, "hash", "Author");
        };

        act.Should().Throw<DomainException>().WithMessage("*Email*");
    }
}