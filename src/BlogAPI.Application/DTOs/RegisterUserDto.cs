namespace BlogAPI.Application.DTOs;

public record RegisterUserDto(string Name, string Email, string Password, string Role = "Author");