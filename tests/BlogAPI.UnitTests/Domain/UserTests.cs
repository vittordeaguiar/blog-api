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
        user.Id.Should().NotBeEmpty();
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

    [Fact]
    public void Constructor_Should_ThrowException_When_NameIsTooShort()
    {
        const string shortName = "Ab";

        var act = () => new User(shortName, "valid@email.com", "hash", "Author");

        act.Should().Throw<DomainException>()
            .WithMessage("Name must be between 3 and 100 characters");
    }

    [Fact]
    public void Constructor_Should_CreateUser_When_NameIsAtMinimumLength()
    {
        const string minName = "Bob";

        var user = new User(minName, "valid@email.com", "hash", "Author");

        user.Should().NotBeNull();
        user.Name.Should().Be(minName);
        user.Name.Length.Should().Be(3);
    }

    [Fact]
    public void Constructor_Should_CreateUser_When_NameIsAtMaximumLength()
    {
        var maxName = new string('A', 100);

        var user = new User(maxName, "valid@email.com", "hash", "Author");

        user.Should().NotBeNull();
        user.Name.Should().Be(maxName);
        user.Name.Length.Should().Be(100);
    }

    [Fact]
    public void Constructor_Should_ThrowException_When_NameIsTooLong()
    {
        var longName = new string('A', 101);

        var act = () => new User(longName, "valid@email.com", "hash", "Author");

        act.Should().Throw<DomainException>()
            .WithMessage("Name must be between 3 and 100 characters");
    }

    [Fact]
    public void Constructor_Should_ThrowException_When_EmailIsNull()
    {
        var act = () => new User("ValidName", null!, "hash", "Author");

        act.Should().Throw<DomainException>()
            .WithMessage("*Email*");
    }

    [Fact]
    public void Constructor_Should_CreateUser_When_EmailHasValidDomain()
    {
        const string validEmail = "user@example.com";

        var user = new User("ValidName", validEmail, "hash", "Author");

        user.Should().NotBeNull();
        user.Email.Should().Be(validEmail);
    }

    [Fact]
    public void Constructor_Should_ThrowException_When_EmailHasNoAtSymbol()
    {
        const string invalidEmail = "invalidemail.com";

        var act = () => new User("ValidName", invalidEmail, "hash", "Author");

        act.Should().Throw<DomainException>()
            .WithMessage("Invalid Email address");
    }

    [Fact]
    public void Constructor_Should_ThrowException_When_PasswordHashIsEmpty()
    {
        var act = () => new User("ValidName", "valid@email.com", "", "Author");

        act.Should().Throw<DomainException>()
            .WithMessage("PasswordHash cannot be empty");
    }

    [Fact]
    public void Constructor_Should_ThrowException_When_PasswordHashIsNull()
    {
        var act = () => new User("ValidName", "valid@email.com", null!, "Author");

        act.Should().Throw<DomainException>()
            .WithMessage("PasswordHash cannot be empty");
    }

    [Fact]
    public void Constructor_Should_CreateUser_When_PasswordHashIsValid()
    {
        const string validHash = "secure_hash_123";

        var user = new User("ValidName", "valid@email.com", validHash, "Author");

        user.Should().NotBeNull();
        user.PasswordHash.Should().Be(validHash);
    }

    [Fact]
    public void Constructor_Should_CreateUser_When_RoleIsAdmin()
    {
        const string role = "Admin";

        var user = new User("ValidName", "valid@email.com", "hash", role);

        user.Should().NotBeNull();
        user.Role.Should().Be(role);
    }

    [Fact]
    public void Constructor_Should_CreateUser_When_RoleIsAuthor()
    {
        const string role = "Author";

        var user = new User("ValidName", "valid@email.com", "hash", role);

        user.Should().NotBeNull();
        user.Role.Should().Be(role);
    }

    [Fact]
    public void Constructor_Should_ThrowException_When_RoleIsInvalid()
    {
        const string invalidRole = "User";

        var act = () => new User("ValidName", "valid@email.com", "hash", invalidRole);

        act.Should().Throw<DomainException>()
            .WithMessage("Invalid Role");
    }

    [Fact]
    public void Constructor_Should_ThrowException_When_RoleIsEmpty()
    {
        var act = () => new User("ValidName", "valid@email.com", "hash", "");

        act.Should().Throw<DomainException>()
            .WithMessage("*must not be empty*");
    }

    [Fact]
    public void Constructor_Should_ThrowException_When_RoleIsCaseIncorrect()
    {
        const string lowercaseRole = "admin";

        var act = () => new User("ValidName", "valid@email.com", "hash", lowercaseRole);

        act.Should().Throw<DomainException>()
            .WithMessage("Invalid Role");
    }

    [Fact]
    public void Constructor_Should_GenerateUniqueIds_ForDifferentInstances()
    {
        var user1 = new User("User One", "user1@email.com", "hash", "Author");
        var user2 = new User("User Two", "user2@email.com", "hash", "Admin");

        user1.Id.Should().NotBe(user2.Id);
        user1.Id.Should().NotBeEmpty();
        user2.Id.Should().NotBeEmpty();
    }
}