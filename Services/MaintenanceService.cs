using ApartaTrack.Models;
using ApartaTrack.Repositories;

namespace ApartaTrack.Services;

/// <summary>
/// Maintenance business logic — delegates data access to IMaintenanceRepository.
/// </summary>
public class MaintenanceService : IMaintenanceService
{
    private readonly IMaintenanceRepository _repo;

    public MaintenanceService(IMaintenanceRepository repo) => _repo = repo;

    public Task<List<MaintenanceRequest>> GetAllAsync()        => _repo.GetAllWithDetailsAsync();
    public Task<MaintenanceRequest?>      GetByIdAsync(int id) => _repo.GetByIdWithDetailsAsync(id);

    public async Task<MaintenanceRequest> CreateAsync(MaintenanceRequest request)
    {
        request.RequestDate = DateTime.Now;
        return await _repo.AddAsync(request);
    }

    public async Task UpdateAsync(MaintenanceRequest request)
    {
        // Business rule: auto-set resolved date when status moves to Completed
        if (request.Status == MaintenanceStatus.Completed && request.ResolvedDate == null)
            request.ResolvedDate = DateTime.Now;

        await _repo.UpdateAsync(request);
    }

    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    public Task<List<MaintenanceRequest>> GetByStatusAsync(MaintenanceStatus status) =>
        _repo.GetByStatusAsync(status);

    public Task<List<MaintenanceRequest>> GetByApartmentAsync(int apartmentId) =>
        _repo.GetByApartmentAsync(apartmentId);

    public Task<int> GetPendingCountAsync() =>
        _repo.CountAsync(m => m.Status == MaintenanceStatus.Pending);
}
