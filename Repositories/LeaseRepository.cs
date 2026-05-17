using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Repositories;

public class LeaseRepository : Repository<Lease>, ILeaseRepository
{
    public LeaseRepository(AppDbContext db) : base(db) { }

    public Task<List<Lease>> GetAllWithDetailsAsync() =>
        _set.Include(l => l.Apartment)
            .Include(l => l.Tenant)
            .ToListAsync();

    public Task<Lease?> GetByIdWithDetailsAsync(int id) =>
        _set.Include(l => l.Apartment)
            .Include(l => l.Tenant)
            .Include(l => l.Payments)
            .FirstOrDefaultAsync(l => l.Id == id);

    public Task<List<Lease>> GetActiveAsync() =>
        _set.Include(l => l.Apartment)
            .Include(l => l.Tenant)
            .Where(l => l.IsActive)
            .ToListAsync();
}
