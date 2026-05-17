using ApartaTrack.Models;
using ApartaTrack.Repositories;

namespace ApartaTrack.Services;

/// <summary>
/// Payment business logic — delegates data access to IPaymentRepository.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repo;

    public PaymentService(IPaymentRepository repo) => _repo = repo;

    public Task<List<Payment>> GetAllAsync()        => _repo.GetAllWithDetailsAsync();
    public Task<Payment?>      GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task<Payment> CreateAsync(Payment payment) => _repo.AddAsync(payment);

    public Task UpdateAsync(Payment payment) => _repo.UpdateAsync(payment);

    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    public Task<List<Payment>> GetByLeaseAsync(int leaseId) => _repo.GetByLeaseAsync(leaseId);

    public Task<decimal> GetTotalCollectedAsync() => _repo.GetTotalCollectedAsync();
}
