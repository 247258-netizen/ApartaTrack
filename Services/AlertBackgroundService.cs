using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartaTrack.Services;

/// <summary>
/// Hosted background service that runs every hour.
/// 1. Marks overdue payments as PaymentStatus.Late
/// 2. Generates notifications for overdue payments and expiring leases
/// </summary>
public class AlertBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AlertBackgroundService> _logger;

    public AlertBackgroundService(IServiceScopeFactory scopeFactory, ILogger<AlertBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AlertBackgroundService started.");

        // Run immediately on startup, then every hour
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunChecksAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AlertBackgroundService.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task RunChecksAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await MarkOverduePaymentsAsync(db);
        await GenerateAlertNotificationsAsync(db);

        _logger.LogInformation("Alert checks completed at {Time}.", DateTime.Now);
    }

    /// <summary>
    /// Finds all Pending payments whose DueDate has passed and marks them Late.
    /// </summary>
    private static async Task MarkOverduePaymentsAsync(AppDbContext db)
    {
        var today = DateTime.Today;

        var overduePayments = await db.Payments
            .Where(p => p.Status == PaymentStatus.Pending && p.DueDate < today)
            .ToListAsync();

        foreach (var payment in overduePayments)
            payment.Status = PaymentStatus.Late;

        if (overduePayments.Count > 0)
            await db.SaveChangesAsync();
    }

    /// <summary>
    /// Creates notifications for overdue payments and leases expiring within 30 days.
    /// Avoids duplicate notifications using AnyAsync check.
    /// </summary>
    private static async Task GenerateAlertNotificationsAsync(AppDbContext db)
    {
        var today = DateTime.Today;

        // ── Overdue payment notifications ──────────────────────────────────
        var latePayments = await db.Payments
            .Include(p => p.Lease).ThenInclude(l => l!.Tenant)
            .Where(p => p.Status == PaymentStatus.Late)
            .ToListAsync();

        foreach (var payment in latePayments)
        {
            bool exists = await db.Notifications.AnyAsync(n =>
                n.PaymentId == payment.Id && n.Type == NotificationType.PaymentOverdue);

            if (!exists)
            {
                db.Notifications.Add(new Notification
                {
                    Title     = "Overdue Payment",
                    Message   = $"Payment of PKR {payment.Amount:N0} for {payment.Lease?.Tenant?.FullName} is overdue since {payment.DueDate:dd/MM/yyyy}.",
                    Type      = NotificationType.PaymentOverdue,
                    TenantId  = payment.Lease?.TenantId,
                    PaymentId = payment.Id,
                    LeaseId   = payment.LeaseId
                });
            }
        }

        // ── Lease expiry notifications (within 30 days) ────────────────────
        var expiringLeases = await db.Leases
            .Include(l => l.Tenant)
            .Include(l => l.Apartment)
            .Where(l => l.IsActive && l.EndDate >= today && l.EndDate <= today.AddDays(30))
            .ToListAsync();

        foreach (var lease in expiringLeases)
        {
            bool exists = await db.Notifications.AnyAsync(n =>
                n.LeaseId == lease.Id && n.Type == NotificationType.LeaseExpiring);

            if (!exists)
            {
                db.Notifications.Add(new Notification
                {
                    Title    = "Lease Expiring Soon",
                    Message  = $"Lease for {lease.Tenant?.FullName} in {lease.Apartment?.UnitNumber} expires on {lease.EndDate:dd/MM/yyyy}.",
                    Type     = NotificationType.LeaseExpiring,
                    TenantId = lease.TenantId,
                    LeaseId  = lease.Id
                });
            }
        }

        // ── Expired leases — deactivate and notify ─────────────────────────
        var expiredLeases = await db.Leases
            .Include(l => l.Tenant)
            .Include(l => l.Apartment)
            .Where(l => l.IsActive && l.EndDate < today)
            .ToListAsync();

        foreach (var lease in expiredLeases)
        {
            lease.IsActive = false;

            // Mark apartment available
            var apt = await db.Apartments.FindAsync(lease.ApartmentId);
            if (apt is not null) apt.IsAvailable = true;

            bool exists = await db.Notifications.AnyAsync(n =>
                n.LeaseId == lease.Id && n.Type == NotificationType.LeaseExpired);

            if (!exists)
            {
                db.Notifications.Add(new Notification
                {
                    Title    = "Lease Expired",
                    Message  = $"Lease for {lease.Tenant?.FullName} in {lease.Apartment?.UnitNumber} has expired.",
                    Type     = NotificationType.LeaseExpired,
                    TenantId = lease.TenantId,
                    LeaseId  = lease.Id
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
