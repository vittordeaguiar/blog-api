namespace BlogAPI.Application.DTOs;

public record UpdatePostDto(
    string Title,
    string Content,
    string? Slug = null,
    List<Guid>? CategoryIds = null
);