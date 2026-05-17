using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext db) : base(db) { }

    public Task<List<Payment>> GetAllWithDetailsAsync() =>
        _set.Include(p => p.Lease).ThenInclude(l => l!.Tenant)
            .Include(p => p.Lease).ThenInclude(l => l!.Apartment)
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();

    public Task<List<Payment>> GetByLeaseAsync(int leaseId) =>
        _set.Where(p => p.LeaseId == leaseId)
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();

    public Task<decimal> GetTotalCollectedAsync() =>
        _set.Where(p => p.Status == PaymentStatus.Paid)
            .SumAsync(p => p.Amount);
}
