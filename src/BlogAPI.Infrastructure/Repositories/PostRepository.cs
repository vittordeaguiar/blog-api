using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Interfaces;
using BlogAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Infrastructure.Repositories;

public class PostRepository(BlogDbContext context) : Repository<Post>(context), IPostRepository
{
    public async Task<Post?> GetBySlugAsync(string slug)
    {
        return await context.Posts
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<Post>> GetByAuthorAsync(Guid authorId)
    {
        return await context.Posts
            .Include(p => p.Categories)
            .AsNoTracking()
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetPublishedAsync()
    {
        return await context.Posts
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByCategoryAsync(Guid categoryId)
    {
        return await context.Posts
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .AsNoTracking()
            .Where(p => p.Categories.Any(c => c.Id == categoryId))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}
