using BlogAPI.Application.DTOs;
using BlogAPI.Application.Interfaces;
using BlogAPI.Application.Services;
using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BlogAPI.UnitTests.Application;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _tokenServiceMock = new Mock<ITokenService>();

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _passwordServiceMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Fact]
    public async Task Login_Should_ReturnToken_When_CredentialsAreValid()
    {
        var loginDto = new LoginDto("teste@email.com", "senha123");
        var user = new User("Teste", "teste@email.com", "hash_banco", "Author");
        const string expectedToken = "token_jwt_valido";

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);

        _passwordServiceMock.Setup(p => p.Verify(loginDto.Password, user.PasswordHash)).Returns(true);

        _tokenServiceMock.Setup(t => t.GenerateToken(user)).Returns(expectedToken);

        var result = await _authService.LoginAsync(loginDto);

        result.Should().Be(expectedToken);
    }

    [Fact]
    public async Task Login_Should_ThrowException_When_UserNotFound()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var act = async () => await _authService.LoginAsync(new LoginDto("email", "senha"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}