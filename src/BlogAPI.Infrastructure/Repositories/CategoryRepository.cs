using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Interfaces;
using BlogAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Infrastructure.Repositories;

public class CategoryRepository(BlogDbContext context) : Repository<Category>(context), ICategoryRepository
{
    private readonly BlogDbContext _context = context;

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _context.Categories
            .Include(c => c.Posts)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name);
    }
}