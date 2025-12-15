namespace BlogAPI.Application.DTOs;

public record UpdatePostDto(
    string Title,
    string Content,
    string Slug,
    List<Guid>? CategoryIds = null
);