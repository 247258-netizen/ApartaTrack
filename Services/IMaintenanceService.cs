using ApartaTrack.Models;

namespace ApartaTrack.Services;

public interface IMaintenanceService
{
    Task<List<MaintenanceRequest>> GetAllAsync();
    Task<MaintenanceRequest?> GetByIdAsync(int id);
    Task<MaintenanceRequest> CreateAsync(MaintenanceRequest request);
    Task UpdateAsync(MaintenanceRequest request);
    Task DeleteAsync(int id);
    Task<List<MaintenanceRequest>> GetByStatusAsync(MaintenanceStatus status);
    Task<List<MaintenanceRequest>> GetByApartmentAsync(int apartmentId);
    Task<int> GetPendingCountAsync();
}
