namespace BlogAPI.Application.DTOs;

public record LoginResponseDto(
    string Username,
    string Token
);