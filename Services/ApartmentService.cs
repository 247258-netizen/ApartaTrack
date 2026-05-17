using ApartaTrack.Models;
using ApartaTrack.Repositories;

namespace ApartaTrack.Services;

/// <summary>
/// Apartment business logic — delegates data access to IApartmentRepository.
/// </summary>
public class ApartmentService : IApartmentService
{
    private readonly IApartmentRepository _repo;

    public ApartmentService(IApartmentRepository repo) => _repo = repo;

    public Task<List<Apartment>> GetAllAsync()        => _repo.GetAllWithLeasesAsync();
    public Task<Apartment?>      GetByIdAsync(int id) => _repo.GetByIdWithLeasesAsync(id);

    public Task<Apartment> CreateAsync(Apartment apartment) => _repo.AddAsync(apartment);

    public Task UpdateAsync(Apartment apartment) => _repo.UpdateAsync(apartment);

    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    public Task<List<Apartment>> GetAvailableAsync() => _repo.GetAvailableAsync();

    public Task<int> GetTotalCountAsync() => _repo.CountAsync();
}
