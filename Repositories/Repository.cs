using System.Linq.Expressions;
using ApartaTrack.Data;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Repositories;

/// <summary>
/// Concrete generic repository backed by EF Core / AppDbContext.
/// All services use this instead of touching DbContext directly.
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T>    _set;

    public Repository(AppDbContext db)
    {
        _db  = db;
        _set = db.Set<T>();
    }

    // ── Read ────────────────────────────────────────────────────────────────

    public virtual Task<List<T>> GetAllAsync() =>
        _set.ToListAsync();

    public virtual Task<T?> GetByIdAsync(int id) =>
        _set.FindAsync(id).AsTask()!;

    public virtual Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        _set.Where(predicate).ToListAsync();

    public virtual Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
        _set.FirstOrDefaultAsync(predicate);

    public virtual Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) =>
        predicate is null ? _set.CountAsync() : _set.CountAsync(predicate);

    public virtual Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
        _set.AnyAsync(predicate);

    // ── Write ───────────────────────────────────────────────────────────────

    public virtual async Task<T> AddAsync(T entity)
    {
        await _set.AddAsync(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        await _db.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await _set.FindAsync(id);
        if (entity is not null)
        {
            _set.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        await _db.SaveChangesAsync();
    }

    // ── Bulk ────────────────────────────────────────────────────────────────

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _set.AddRangeAsync(entities);
        await _db.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
