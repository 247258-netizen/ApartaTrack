using ApartaTrack.Models;

namespace ApartaTrack.Repositories;

public interface IMaintenanceRepository : IRepository<MaintenanceRequest>
{
    Task<List<MaintenanceRequest>> GetAllWithDetailsAsync();
    Task<MaintenanceRequest?>      GetByIdWithDetailsAsync(int id);
    Task<List<MaintenanceRequest>> GetByStatusAsync(MaintenanceStatus status);
    Task<List<MaintenanceRequest>> GetByApartmentAsync(int apartmentId);
}
