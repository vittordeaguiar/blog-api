namespace BlogAPI.Application.DTOs;

public record CreatePostDto(
    string Title,
    string Content,
    string? Slug = null,
    List<Guid>? CategoryIds = null
);