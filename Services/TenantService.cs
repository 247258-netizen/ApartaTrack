using ApartaTrack.Models;
using ApartaTrack.Repositories;

namespace ApartaTrack.Services;

/// <summary>
/// Tenant business logic — delegates data access to ITenantRepository.
/// </summary>
public class TenantService : ITenantService
{
    private readonly ITenantRepository _repo;

    public TenantService(ITenantRepository repo) => _repo = repo;

    public Task<List<Tenant>> GetAllAsync()        => _repo.GetAllWithLeasesAsync();
    public Task<Tenant?>      GetByIdAsync(int id) => _repo.GetByIdWithLeasesAsync(id);

    public Task<Tenant> CreateAsync(Tenant tenant) => _repo.AddAsync(tenant);

    public Task UpdateAsync(Tenant tenant) => _repo.UpdateAsync(tenant);

    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    public Task<int> GetTotalCountAsync() => _repo.CountAsync();
}
