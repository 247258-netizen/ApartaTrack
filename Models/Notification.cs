using System.ComponentModel.DataAnnotations;

namespace ApartaTrack.Models;

public enum NotificationType
{
    PaymentDue,
    PaymentOverdue,
    LeaseExpiring,
    LeaseExpired,
    MaintenanceUpdate,
    General
}

/// <summary>
/// System notification for alerts and reminders
/// </summary>
public class Notification
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; } = NotificationType.General;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Optional links to related entities
    public int? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public int? LeaseId { get; set; }
    public Lease? Lease { get; set; }

    public int? PaymentId { get; set; }
    public Payment? Payment { get; set; }
}
