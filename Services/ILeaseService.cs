using ApartaTrack.Models;

namespace ApartaTrack.Services;

public interface ILeaseService
{
    Task<List<Lease>> GetAllAsync();
    Task<Lease?> GetByIdAsync(int id);
    Task<Lease> CreateAsync(Lease lease);
    Task UpdateAsync(Lease lease);
    Task DeleteAsync(int id);
    Task<List<Lease>> GetActiveAsync();
    Task<int> GetTotalCountAsync();
}
