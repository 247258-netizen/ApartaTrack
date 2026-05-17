using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Repositories;

public class MaintenanceRepository : Repository<MaintenanceRequest>, IMaintenanceRepository
{
    public MaintenanceRepository(AppDbContext db) : base(db) { }

    public Task<List<MaintenanceRequest>> GetAllWithDetailsAsync() =>
        _set.Include(m => m.Apartment)
            .Include(m => m.Tenant)
            .OrderByDescending(m => m.RequestDate)
            .ToListAsync();

    public Task<MaintenanceRequest?> GetByIdWithDetailsAsync(int id) =>
        _set.Include(m => m.Apartment)
            .Include(m => m.Tenant)
            .FirstOrDefaultAsync(m => m.Id == id);

    public Task<List<MaintenanceRequest>> GetByStatusAsync(MaintenanceStatus status) =>
        _set.Include(m => m.Apartment)
            .Include(m => m.Tenant)
            .Where(m => m.Status == status)
            .ToListAsync();

    public Task<List<MaintenanceRequest>> GetByApartmentAsync(int apartmentId) =>
        _set.Include(m => m.Tenant)
            .Where(m => m.ApartmentId == apartmentId)
            .ToListAsync();
}
