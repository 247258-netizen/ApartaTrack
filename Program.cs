using ApartaTrack.Data;
using ApartaTrack.Repositories;
using ApartaTrack.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── MVC Controllers (needed for CSV export endpoints) ───────────────────────
builder.Services.AddControllers();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Repositories (Repository Pattern) ──────────────────────────────────────
builder.Services.AddScoped<IApartmentRepository,   ApartmentRepository>();
builder.Services.AddScoped<ITenantRepository,      TenantRepository>();
builder.Services.AddScoped<ILeaseRepository,       LeaseRepository>();
builder.Services.AddScoped<IPaymentRepository,     PaymentRepository>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

// ── Services (Business Logic Layer) ────────────────────────────────────────
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IApartmentService,    ApartmentService>();
builder.Services.AddScoped<ITenantService,       TenantService>();
builder.Services.AddScoped<ILeaseService,        LeaseService>();
builder.Services.AddScoped<IPaymentService,      PaymentService>();
builder.Services.AddScoped<IMaintenanceService,  MaintenanceService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ExportService>();

// ── Background Service: auto-overdue payments + lease expiry alerts ─────────
builder.Services.AddHostedService<AlertBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    var admin = db.Users.FirstOrDefault(u => u.Username == "admin");
    if (admin == null)
    {
        db.Users.Add(new ApartaTrack.Models.ApplicationUser
        {
            FullName     = "System Admin",
            Username     = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Email        = "admin@apartatrack.com",
            Role         = ApartaTrack.Models.UserRole.Admin,
            IsActive     = true,
            CreatedAt    = DateTime.Now
        });
        db.SaveChanges();
    }
    else
    {
        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        admin.IsActive = true;
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();

// ── Map MVC controllers for export endpoints ────────────────────────────────
app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
