namespace BlogAPI.Application.DTOs;

public record CreatePostDto(
    string Title,
    string Content,
    string Slug,
    List<Guid>? CategoryIds = null
);