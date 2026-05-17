using ApartaTrack.Models;

namespace ApartaTrack.Services;

public interface IPaymentService
{
    Task<List<Payment>> GetAllAsync();
    Task<Payment?> GetByIdAsync(int id);
    Task<Payment> CreateAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(int id);
    Task<List<Payment>> GetByLeaseAsync(int leaseId);
    Task<decimal> GetTotalCollectedAsync();
}
