using ApartaTrack.Models;

namespace ApartaTrack.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<List<Payment>> GetAllWithDetailsAsync();
    Task<List<Payment>> GetByLeaseAsync(int leaseId);
    Task<decimal>       GetTotalCollectedAsync();
}
