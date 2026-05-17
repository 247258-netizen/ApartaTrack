using System.ComponentModel.DataAnnotations;

namespace ApartaTrack.Models;

/// <summary>
/// Tenant who rents an apartment
/// </summary>
public class Tenant
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [StringLength(50)]
    [Display(Name = "CNIC / National ID")]
    public string? NationalId { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Emergency Contact")]
    public string? EmergencyContact { get; set; }

    // Navigation property
    public ICollection<Lease> Leases { get; set; } = new List<Lease>();
}
