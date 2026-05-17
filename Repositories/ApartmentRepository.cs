using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Repositories;

public class ApartmentRepository : Repository<Apartment>, IApartmentRepository
{
    public ApartmentRepository(AppDbContext db) : base(db) { }

    public Task<List<Apartment>> GetAllWithLeasesAsync() =>
        _set.Include(a => a.Leases).ToListAsync();

    public Task<Apartment?> GetByIdWithLeasesAsync(int id) =>
        _set.Include(a => a.Leases).FirstOrDefaultAsync(a => a.Id == id);

    public Task<List<Apartment>> GetAvailableAsync() =>
        _set.Where(a => a.IsAvailable).ToListAsync();
}
