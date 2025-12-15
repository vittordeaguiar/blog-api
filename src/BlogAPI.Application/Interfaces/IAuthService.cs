using BlogAPI.Application.DTOs;

namespace BlogAPI.Application.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(LoginDto dto);
    Task RegisterAsync(RegisterUserDto dto);
}