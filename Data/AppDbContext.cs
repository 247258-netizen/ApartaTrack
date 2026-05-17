using Microsoft.EntityFrameworkCore;
using ApartaTrack.Models;

namespace ApartaTrack.Data;

/// <summary>
/// EF Core Database Context — manages all tables and relationships
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Apartment>          Apartments          => Set<Apartment>();
    public DbSet<Tenant>             Tenants             => Set<Tenant>();
    public DbSet<Lease>              Leases              => Set<Lease>();
    public DbSet<Payment>            Payments            => Set<Payment>();
    public DbSet<ApplicationUser>    Users               => Set<ApplicationUser>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<Notification>       Notifications       => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apartment → Leases
        modelBuilder.Entity<Lease>()
            .HasOne(l => l.Apartment).WithMany(a => a.Leases)
            .HasForeignKey(l => l.ApartmentId).OnDelete(DeleteBehavior.Restrict);

        // Tenant → Leases
        modelBuilder.Entity<Lease>()
            .HasOne(l => l.Tenant).WithMany(t => t.Leases)
            .HasForeignKey(l => l.TenantId).OnDelete(DeleteBehavior.Restrict);

        // Lease → Payments
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Lease).WithMany(l => l.Payments)
            .HasForeignKey(p => p.LeaseId).OnDelete(DeleteBehavior.Cascade);

        // Apartment → MaintenanceRequests
        modelBuilder.Entity<MaintenanceRequest>()
            .HasOne(m => m.Apartment).WithMany()
            .HasForeignKey(m => m.ApartmentId).OnDelete(DeleteBehavior.Cascade);

        // Tenant → MaintenanceRequests (optional)
        modelBuilder.Entity<MaintenanceRequest>()
            .HasOne(m => m.Tenant).WithMany()
            .HasForeignKey(m => m.TenantId).OnDelete(DeleteBehavior.SetNull);

        // Notification optional FK
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Tenant).WithMany()
            .HasForeignKey(n => n.TenantId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Lease).WithMany()
            .HasForeignKey(n => n.LeaseId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Payment).WithMany()
            .HasForeignKey(n => n.PaymentId).OnDelete(DeleteBehavior.SetNull);

        // Decimal precision
        modelBuilder.Entity<Apartment>().Property(a => a.MonthlyRent).HasPrecision(18, 2);
        modelBuilder.Entity<Lease>().Property(l => l.MonthlyRent).HasPrecision(18, 2);
        modelBuilder.Entity<Lease>().Property(l => l.SecurityDeposit).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<MaintenanceRequest>().Property(m => m.EstimatedCost).HasPrecision(18, 2);

        // Seed admin user (password: Admin@123)
        modelBuilder.Entity<ApplicationUser>().HasData(new ApplicationUser
        {
            Id = 1, FullName = "System Admin", Username = "admin",
            PasswordHash = "$2a$11$kYsgIaQtj7OV6o7La/0M6e0TQriAqv0.WDviZfGLEWCdwdvSGTAOe",
            Email = "admin@apartatrack.com", Role = UserRole.Admin,
            IsActive = true, CreatedAt = new DateTime(2024, 1, 1)
        });

        // Seed apartments
        modelBuilder.Entity<Apartment>().HasData(
            new Apartment { Id = 1, UnitNumber = "A-101", Address = "Gulberg III, Lahore", Bedrooms = 2, Bathrooms = 1, MonthlyRent = 35000, IsAvailable = true, Description = "2-bed furnished apartment" },
            new Apartment { Id = 2, UnitNumber = "B-202", Address = "DHA Phase 5, Lahore", Bedrooms = 3, Bathrooms = 2, MonthlyRent = 65000, IsAvailable = false, Description = "3-bed semi-furnished" },
            new Apartment { Id = 3, UnitNumber = "C-303", Address = "Model Town, Lahore", Bedrooms = 1, Bathrooms = 1, MonthlyRent = 20000, IsAvailable = true, Description = "1-bed bachelor apartment" }
        );

        modelBuilder.Entity<Tenant>().HasData(
            new Tenant { Id = 1, FullName = "Ahmed Khan", Email = "ahmed@example.com", Phone = "03001234567", NationalId = "35202-1234567-1" }
        );
    }
}
