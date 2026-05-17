using System.ComponentModel.DataAnnotations;

namespace ApartaTrack.Models;

/// <summary>
/// User roles for role-based access control
/// </summary>
public enum UserRole
{
    Admin,
    Manager,
    Staff
}

/// <summary>
/// Application user for authentication and authorization
/// </summary>
public class ApplicationUser
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Staff;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? LastLogin { get; set; }
}
