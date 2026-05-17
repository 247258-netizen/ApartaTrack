using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Repositories;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(AppDbContext db) : base(db) { }

    public Task<List<Tenant>> GetAllWithLeasesAsync() =>
        _set.Include(t => t.Leases).ToListAsync();

    public Task<Tenant?> GetByIdWithLeasesAsync(int id) =>
        _set.Include(t => t.Leases).FirstOrDefaultAsync(t => t.Id == id);
}
