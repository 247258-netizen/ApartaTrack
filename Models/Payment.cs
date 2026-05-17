using System.ComponentModel.DataAnnotations;

namespace ApartaTrack.Models;

/// <summary>
/// Payment status enum
/// </summary>
public enum PaymentStatus
{
    Pending,
    Paid,
    Late,
    Cancelled
}

/// <summary>
/// Monthly rent payment record
/// </summary>
public class Payment
{
    public int Id { get; set; }

    [Required]
    public int LeaseId { get; set; }
    public Lease? Lease { get; set; }

    [Required]
    [Range(0, 10000000)]
    public decimal Amount { get; set; }

    [Required]
    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Display(Name = "Paid Date")]
    [DataType(DataType.Date)]
    public DateTime? PaidDate { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [StringLength(500)]
    public string? Notes { get; set; }
}
