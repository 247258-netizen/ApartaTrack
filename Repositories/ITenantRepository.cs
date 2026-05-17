using ApartaTrack.Models;

namespace ApartaTrack.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<List<Tenant>> GetAllWithLeasesAsync();
    Task<Tenant?>      GetByIdWithLeasesAsync(int id);
}
