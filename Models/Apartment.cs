using System.ComponentModel.DataAnnotations;

namespace ApartaTrack.Models;

/// <summary>
/// Apartment unit with rent and availability info
/// </summary>
public class Apartment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Unit number is required")]
    [StringLength(50)]
    [Display(Name = "Unit Number")]
    public string UnitNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Range(0, 20, ErrorMessage = "Bedrooms must be between 0 and 20")]
    public int Bedrooms { get; set; }

    [Range(0, 20, ErrorMessage = "Bathrooms must be between 0 and 20")]
    public int Bathrooms { get; set; }

    [Required]
    [Range(0, 10000000)]
    [Display(Name = "Monthly Rent (PKR)")]
    public decimal MonthlyRent { get; set; }

    [Display(Name = "Available")]
    public bool IsAvailable { get; set; } = true;

    [StringLength(500)]
    public string? Description { get; set; }

    // Navigation property
    public ICollection<Lease> Leases { get; set; } = new List<Lease>();
}
