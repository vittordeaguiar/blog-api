using BlogAPI.Domain.Interfaces;
using BlogAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Infrastructure.Repositories;

public class Repository<T>(BlogDbContext context) : IRepository<T> where T : class
{
    private readonly BlogDbContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    protected IQueryable<T> Query => _dbSet;

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => EF.Property<Guid>(e, "Id") == id);
    }
}