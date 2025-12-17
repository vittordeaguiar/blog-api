using BlogAPI.Application.DTOs;

namespace BlogAPI.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
    Task RegisterAsync(RegisterUserDto dto);
}