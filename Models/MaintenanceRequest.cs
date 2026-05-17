using System.ComponentModel.DataAnnotations;

namespace ApartaTrack.Models;

public enum MaintenanceStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public enum MaintenancePriority
{
    Low,
    Medium,
    High,
    Urgent
}

/// <summary>
/// Maintenance request raised by tenant or staff for an apartment
/// </summary>
public class MaintenanceRequest
{
    public int Id { get; set; }

    public int ApartmentId { get; set; }
    public Apartment? Apartment { get; set; }

    public int? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public MaintenancePriority Priority { get; set; } = MaintenancePriority.Medium;

    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Pending;

    public DateTime RequestDate { get; set; } = DateTime.Now;

    public DateTime? ResolvedDate { get; set; }

    public string? Notes { get; set; }

    [Display(Name = "Estimated Cost (PKR)")]
    public decimal? EstimatedCost { get; set; }
}
