using ApartaTrack.Models;

namespace ApartaTrack.Repositories;

public interface ILeaseRepository : IRepository<Lease>
{
    Task<List<Lease>> GetAllWithDetailsAsync();
    Task<Lease?>      GetByIdWithDetailsAsync(int id);
    Task<List<Lease>> GetActiveAsync();
}
