using BlogAPI.Application.DTOs;
using BlogAPI.Application.Interfaces;
using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Interfaces;
using BlogAPI.Domain.Exceptions;

namespace BlogAPI.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordService passwordService,
    ITokenService tokenService)
    : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await userRepository.GetByEmailAsync(dto.Email);

        if (user is null || !passwordService.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var response = new LoginResponseDto(Username: user.Name, Token: tokenService.GenerateToken(user));

        return response;
    }

    public async Task RegisterAsync(RegisterUserDto dto)
    {
        var existingUser = await userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null) throw new DomainException("Email already in use");

        var passwordHash = passwordService.Hash(dto.Password);

        var newUser = new User(dto.Name, dto.Email, passwordHash, dto.Role);

        await userRepository.AddAsync(newUser);
    }
}