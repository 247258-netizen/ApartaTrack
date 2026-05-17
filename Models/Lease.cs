using System.ComponentModel.DataAnnotations;

namespace ApartaTrack.Models;

/// <summary>
/// Lease agreement between a tenant and an apartment
/// </summary>
public class Lease
{
    public int Id { get; set; }

    [Required]
    public int ApartmentId { get; set; }
    public Apartment? Apartment { get; set; }

    [Required]
    public int TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    [Required]
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(0, 10000000)]
    [Display(Name = "Monthly Rent (PKR)")]
    public decimal MonthlyRent { get; set; }

    [Display(Name = "Security Deposit (PKR)")]
    public decimal SecurityDeposit { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    public string? Notes { get; set; }

    // Navigation property
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
