using BlogAPI.Application.Common;
using BlogAPI.Application.DTOs;

namespace BlogAPI.Application.Interfaces;

public interface IPostService
{
    Task<PostResponseDto> CreatePostAsync(CreatePostDto dto, Guid authorId);
    Task<PagedResult<PostResponseDto>> GetPostsAsync(int page, int pageSize);
    Task<PostResponseDto> GetPostByIdAsync(Guid id);
    Task<PostResponseDto> GetPostBySlugAsync(string slug);
    Task<PostResponseDto> UpdatePostAsync(Guid id, UpdatePostDto dto);
    Task DeletePostAsync(Guid id);
    Task<PostResponseDto> PublishPostAsync(Guid id);
    Task<PostResponseDto> UnpublishPostAsync(Guid id);
}