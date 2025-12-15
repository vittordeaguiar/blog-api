using BlogAPI.Domain.Entities;

namespace BlogAPI.Domain.Interfaces;

public interface IPostRepository : IRepository<Post>
{
    Task<Post?> GetBySlugAsync(string slug);
    Task<IEnumerable<Post>> GetByAuthorAsync(Guid authorId);
    Task<IEnumerable<Post>> GetPublishedAsync();
    Task<IEnumerable<Post>> GetByCategoryAsync(Guid categoryId);
    Task<(IEnumerable<Post> Posts, int TotalCount)> GetPagedAsync(int page, int pageSize);
}
