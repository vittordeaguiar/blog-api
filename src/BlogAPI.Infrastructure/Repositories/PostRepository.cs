using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Interfaces;
using BlogAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Infrastructure.Repositories;

public class PostRepository(BlogDbContext context) : Repository<Post>(context), IPostRepository
{
    private readonly BlogDbContext _context = context;

    public async Task<Post?> GetBySlugAsync(string slug)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<Post>> GetByAuthorAsync(Guid authorId)
    {
        return await _context.Posts
            .Include(p => p.Categories)
            .AsNoTracking()
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetPublishedAsync()
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByCategoryAsync(Guid categoryId)
    {
        return await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .AsNoTracking()
            .Where(p => p.Categories.Any(c => c.Id == categoryId))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Post> Posts, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync();

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (posts, totalCount);
    }
}