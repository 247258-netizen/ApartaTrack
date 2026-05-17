using ApartaTrack.Models;

namespace ApartaTrack.Services;

public interface ITenantService
{
    Task<List<Tenant>> GetAllAsync();
    Task<Tenant?> GetByIdAsync(int id);
    Task<Tenant> CreateAsync(Tenant tenant);
    Task UpdateAsync(Tenant tenant);
    Task DeleteAsync(int id);
    Task<int> GetTotalCountAsync();
}
