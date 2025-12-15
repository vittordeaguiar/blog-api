namespace BlogAPI.Application.DTOs;

public record PostResponseDto(
    Guid Id,
    string Title,
    string Content,
    string Slug,
    Guid AuthorId,
    string AuthorName,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? PublishedAt,
    bool IsPublished,
    List<CategoryResponseDto> Categories
);

public record CategoryResponseDto(
    Guid Id,
    string Name,
    string Slug
);