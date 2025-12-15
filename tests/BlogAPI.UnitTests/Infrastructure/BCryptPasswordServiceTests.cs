using FluentAssertions;
using BlogAPI.Infrastructure.Services;

namespace BlogAPI.UnitTests.Infrastructure;

public class BCryptPasswordServiceTests
{
    [Fact]
    public void Hash_Should_GenerateSecureHash_DifferentFromPassword()
    {
        var service = new BCryptPasswordService();
        const string password = "minhaS&nha123!";

        var hash = service.Hash(password);

        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password); // O hash não ser igual à senha
        hash.Should().StartWith("$2");
    }

    [Fact]
    public void Verify_Should_ReturnTrue_When_PasswordIsCorrect()
    {
        var service = new BCryptPasswordService();
        const string password = "senha_teste";
        var hash = service.Hash(password);

        var result = service.Verify(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_Should_ReturnFalse_When_PasswordIsWrong()
    {
        var service = new BCryptPasswordService();
        const string password = "senha_teste";
        var hash = service.Hash(password);

        var result = service.Verify("senha_errada", hash);

        result.Should().BeFalse();
    }
}