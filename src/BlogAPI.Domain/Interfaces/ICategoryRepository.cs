using BlogAPI.Domain.Entities;

namespace BlogAPI.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug);
    Task<Category?> GetByNameAsync(string name);
}
