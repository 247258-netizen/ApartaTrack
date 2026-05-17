using ApartaTrack.Models;

namespace ApartaTrack.Services;

public interface IApartmentService
{
    Task<List<Apartment>> GetAllAsync();
    Task<Apartment?> GetByIdAsync(int id);
    Task<Apartment> CreateAsync(Apartment apartment);
    Task UpdateAsync(Apartment apartment);
    Task DeleteAsync(int id);
    Task<List<Apartment>> GetAvailableAsync();
    Task<int> GetTotalCountAsync();
}
