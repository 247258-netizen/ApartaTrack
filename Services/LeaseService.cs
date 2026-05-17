using ApartaTrack.Models;
using ApartaTrack.Repositories;

namespace ApartaTrack.Services;

/// <summary>
/// Lease business logic — delegates data access to ILeaseRepository.
/// Business rules (apartment availability toggle) live here, not in the repo.
/// </summary>
public class LeaseService : ILeaseService
{
    private readonly ILeaseRepository     _leaseRepo;
    private readonly IApartmentRepository _aptRepo;

    public LeaseService(ILeaseRepository leaseRepo, IApartmentRepository aptRepo)
    {
        _leaseRepo = leaseRepo;
        _aptRepo   = aptRepo;
    }

    public Task<List<Lease>> GetAllAsync()        => _leaseRepo.GetAllWithDetailsAsync();
    public Task<Lease?>      GetByIdAsync(int id) => _leaseRepo.GetByIdWithDetailsAsync(id);

    public async Task<Lease> CreateAsync(Lease lease)
    {
        // Business rule: mark apartment unavailable when lease is created
        var apt = await _aptRepo.GetByIdAsync(lease.ApartmentId);
        if (apt is not null) { apt.IsAvailable = false; await _aptRepo.UpdateAsync(apt); }

        return await _leaseRepo.AddAsync(lease);
    }

    public Task UpdateAsync(Lease lease) => _leaseRepo.UpdateAsync(lease);

    public async Task DeleteAsync(int id)
    {
        var lease = await _leaseRepo.GetByIdAsync(id);
        if (lease is null) return;

        // Business rule: mark apartment available again when lease is removed
        var apt = await _aptRepo.GetByIdAsync(lease.ApartmentId);
        if (apt is not null) { apt.IsAvailable = true; await _aptRepo.UpdateAsync(apt); }

        await _leaseRepo.DeleteAsync(lease);
    }

    public Task<List<Lease>> GetActiveAsync() => _leaseRepo.GetActiveAsync();

    public Task<int> GetTotalCountAsync() => _leaseRepo.CountAsync(l => l.IsActive);
}
