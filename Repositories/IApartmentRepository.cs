using ApartaTrack.Models;

namespace ApartaTrack.Repositories;

/// <summary>Apartment-specific queries on top of the generic repository</summary>
public interface IApartmentRepository : IRepository<Apartment>
{
    Task<List<Apartment>> GetAllWithLeasesAsync();
    Task<Apartment?>      GetByIdWithLeasesAsync(int id);
    Task<List<Apartment>> GetAvailableAsync();
}
