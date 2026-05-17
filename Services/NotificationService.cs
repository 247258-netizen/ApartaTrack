using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _db;
    public NotificationService(AppDbContext db) => _db = db;

    public Task<List<Notification>> GetAllAsync() =>
        _db.Notifications
            .Include(n => n.Tenant)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public Task<List<Notification>> GetUnreadAsync() =>
        _db.Notifications
            .Include(n => n.Tenant)
            .Where(n => !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task MarkReadAsync(int id)
    {
        var n = await _db.Notifications.FindAsync(id);
        if (n != null) { n.IsRead = true; await _db.SaveChangesAsync(); }
    }

    public async Task MarkAllReadAsync()
    {
        var unread = await _db.Notifications.Where(n => !n.IsRead).ToListAsync();
        unread.ForEach(n => n.IsRead = true);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var n = await _db.Notifications.FindAsync(id);
        if (n != null) { _db.Notifications.Remove(n); await _db.SaveChangesAsync(); }
    }

    public Task<int> GetUnreadCountAsync() =>
        _db.Notifications.CountAsync(n => !n.IsRead);

    /// <summary>
    /// Auto-generates alerts: overdue payments, expiring leases
    /// </summary>
    public async Task GenerateAlertsAsync()
    {
        var today = DateTime.Today;

        // Overdue payments
        var overduePayments = await _db.Payments
            .Include(p => p.Lease).ThenInclude(l => l!.Tenant)
            .Where(p => p.Status == PaymentStatus.Pending && p.DueDate < today)
            .ToListAsync();

        foreach (var payment in overduePayments)
        {
            bool exists = await _db.Notifications.AnyAsync(n =>
                n.PaymentId == payment.Id && n.Type == NotificationType.PaymentOverdue);
            if (!exists)
            {
                _db.Notifications.Add(new Notification
                {
                    Title = "Overdue Payment",
                    Message = $"Payment of PKR {payment.Amount:N0} for {payment.Lease?.Tenant?.FullName} is overdue since {payment.DueDate:dd/MM/yyyy}.",
                    Type = NotificationType.PaymentOverdue,
                    TenantId = payment.Lease?.TenantId,
                    PaymentId = payment.Id,
                    LeaseId = payment.LeaseId
                });
            }
        }

        // Leases expiring within 30 days
        var expiringLeases = await _db.Leases
            .Include(l => l.Tenant)
            .Include(l => l.Apartment)
            .Where(l => l.IsActive && l.EndDate >= today && l.EndDate <= today.AddDays(30))
            .ToListAsync();

        foreach (var lease in expiringLeases)
        {
            bool exists = await _db.Notifications.AnyAsync(n =>
                n.LeaseId == lease.Id && n.Type == NotificationType.LeaseExpiring);
            if (!exists)
            {
                _db.Notifications.Add(new Notification
                {
                    Title = "Lease Expiring Soon",
                    Message = $"Lease for {lease.Tenant?.FullName} in {lease.Apartment?.UnitNumber} expires on {lease.EndDate:dd/MM/yyyy}.",
                    Type = NotificationType.LeaseExpiring,
                    TenantId = lease.TenantId,
                    LeaseId = lease.Id
                });
            }
        }

        await _db.SaveChangesAsync();
    }
}
