using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Services;

public class MonthlyReport
{
    public string Month { get; set; } = "";
    public decimal TotalCollected { get; set; }
    public int PaymentCount { get; set; }
}

public class ReportService
{
    private readonly AppDbContext _db;
    public ReportService(AppDbContext db) => _db = db;

    public async Task<List<MonthlyReport>> GetMonthlyRevenueAsync(int year)
    {
        var payments = await _db.Payments
            .Where(p => p.Status == PaymentStatus.Paid && p.PaidDate.HasValue && p.PaidDate.Value.Year == year)
            .ToListAsync();

        return payments
            .GroupBy(p => p.PaidDate!.Value.Month)
            .Select(g => new MonthlyReport
            {
                Month = new DateTime(year, g.Key, 1).ToString("MMM"),
                TotalCollected = g.Sum(p => p.Amount),
                PaymentCount = g.Count()
            })
            .OrderBy(r => DateTime.ParseExact(r.Month, "MMM", null).Month)
            .ToList();
    }

    public async Task<decimal> GetTotalRevenueAsync() =>
        await _db.Payments.Where(p => p.Status == PaymentStatus.Paid).SumAsync(p => p.Amount);

    public async Task<decimal> GetPendingAmountAsync() =>
        await _db.Payments.Where(p => p.Status == PaymentStatus.Pending).SumAsync(p => p.Amount);

    public async Task<int> GetOccupancyRateAsync()
    {
        int total = await _db.Apartments.CountAsync();
        if (total == 0) return 0;
        int occupied = await _db.Apartments.CountAsync(a => !a.IsAvailable);
        return (int)Math.Round((double)occupied / total * 100);
    }

    public async Task<List<Tenant>> GetTopTenantsAsync(int count = 5)
    {
        var tenantPayments = await _db.Payments
            .Include(p => p.Lease).ThenInclude(l => l!.Tenant)
            .Where(p => p.Status == PaymentStatus.Paid)
            .GroupBy(p => p.Lease!.TenantId)
            .Select(g => new { TenantId = g.Key, Total = g.Sum(p => p.Amount) })
            .OrderByDescending(x => x.Total)
            .Take(count)
            .ToListAsync();

        var ids = tenantPayments.Select(x => x.TenantId).ToList();
        return await _db.Tenants.Where(t => ids.Contains(t.Id)).ToListAsync();
    }
}
