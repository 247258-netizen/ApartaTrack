using System.Linq.Expressions;

namespace ApartaTrack.Repositories;

/// <summary>
/// Generic repository interface — provides standard CRUD + query operations
/// for any EF Core entity. Services depend on this abstraction, not EF directly.
/// </summary>
public interface IRepository<T> where T : class
{
    // ── Read ────────────────────────────────────────────────────────────────
    Task<List<T>>   GetAllAsync();
    Task<T?>        GetByIdAsync(int id);
    Task<List<T>>   FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?>        FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<int>       CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<bool>      ExistsAsync(Expression<Func<T, bool>> predicate);

    // ── Write ───────────────────────────────────────────────────────────────
    Task<T>  AddAsync(T entity);
    Task     UpdateAsync(T entity);
    Task     DeleteAsync(int id);
    Task     DeleteAsync(T entity);

    // ── Bulk ────────────────────────────────────────────────────────────────
    Task AddRangeAsync(IEnumerable<T> entities);
    Task SaveChangesAsync();
}
