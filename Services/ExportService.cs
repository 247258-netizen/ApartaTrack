using ApartaTrack.Data;
using ApartaTrack.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ApartaTrack.Services;

/// <summary>
/// Generates CSV export strings for Payments, Tenants, and Maintenance reports.
/// Returned string is UTF-8 text — controllers / Blazor pages convert to byte[] for download.
/// </summary>
public class ExportService
{
    private readonly AppDbContext _db;
    public ExportService(AppDbContext db) => _db = db;

    // ── Payments CSV ──────────────────────────────────────────────────────
    public async Task<byte[]> ExportPaymentsCsvAsync()
    {
        var payments = await _db.Payments
            .Include(p => p.Lease).ThenInclude(l => l!.Tenant)
            .Include(p => p.Lease).ThenInclude(l => l!.Apartment)
            .OrderByDescending(p => p.DueDate)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("ID,Tenant,Apartment,Amount (PKR),Due Date,Paid Date,Status,Notes");

        foreach (var p in payments)
        {
            sb.AppendLine(string.Join(",",
                p.Id,
                CsvEscape(p.Lease?.Tenant?.FullName),
                CsvEscape(p.Lease?.Apartment?.UnitNumber),
                p.Amount.ToString("N0"),
                p.DueDate.ToString("dd/MM/yyyy"),
                p.PaidDate?.ToString("dd/MM/yyyy") ?? "",
                p.Status.ToString(),
                CsvEscape(p.Notes)
            ));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // ── Tenants CSV ───────────────────────────────────────────────────────
    public async Task<byte[]> ExportTenantsCsvAsync()
    {
        var tenants = await _db.Tenants
            .Include(t => t.Leases).ThenInclude(l => l.Apartment)
            .OrderBy(t => t.FullName)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("ID,Full Name,Email,Phone,CNIC,Date of Birth,Active Leases,Apartments");

        foreach (var t in tenants)
        {
            var activeLeases = t.Leases.Where(l => l.IsActive).ToList();
            sb.AppendLine(string.Join(",",
                t.Id,
                CsvEscape(t.FullName),
                CsvEscape(t.Email),
                CsvEscape(t.Phone),
                CsvEscape(t.NationalId),
                t.DateOfBirth?.ToString("dd/MM/yyyy") ?? "",
                activeLeases.Count,
                CsvEscape(string.Join("; ", activeLeases.Select(l => l.Apartment?.UnitNumber ?? "")))
            ));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // ── Maintenance CSV ───────────────────────────────────────────────────
    public async Task<byte[]> ExportMaintenanceCsvAsync()
    {
        var requests = await _db.MaintenanceRequests
            .Include(m => m.Apartment)
            .Include(m => m.Tenant)
            .OrderByDescending(m => m.RequestDate)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("ID,Apartment,Tenant,Title,Priority,Status,Request Date,Resolved Date,Estimated Cost (PKR),Notes");

        foreach (var m in requests)
        {
            sb.AppendLine(string.Join(",",
                m.Id,
                CsvEscape(m.Apartment?.UnitNumber),
                CsvEscape(m.Tenant?.FullName ?? "Staff"),
                CsvEscape(m.Title),
                m.Priority.ToString(),
                m.Status.ToString(),
                m.RequestDate.ToString("dd/MM/yyyy"),
                m.ResolvedDate?.ToString("dd/MM/yyyy") ?? "",
                m.EstimatedCost?.ToString("N0") ?? "0",
                CsvEscape(m.Notes)
            ));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // ── Monthly Revenue CSV ───────────────────────────────────────────────
    public async Task<byte[]> ExportMonthlyRevenueCsvAsync(int year)
    {
        var payments = await _db.Payments
            .Where(p => p.Status == PaymentStatus.Paid && p.PaidDate.HasValue && p.PaidDate.Value.Year == year)
            .ToListAsync();

        var monthly = payments
            .GroupBy(p => p.PaidDate!.Value.Month)
            .Select(g => new
            {
                Month         = new DateTime(year, g.Key, 1).ToString("MMMM"),
                MonthNum      = g.Key,
                TotalCollected = g.Sum(p => p.Amount),
                PaymentCount  = g.Count()
            })
            .OrderBy(r => r.MonthNum)
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine($"Monthly Revenue Report — {year}");
        sb.AppendLine("Month,Total Collected (PKR),Payment Count");

        foreach (var m in monthly)
            sb.AppendLine($"{m.Month},{m.TotalCollected:N0},{m.PaymentCount}");

        sb.AppendLine();
        sb.AppendLine($"Grand Total,{monthly.Sum(m => m.TotalCollected):N0},{monthly.Sum(m => m.PaymentCount)}");

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // ── Helper ────────────────────────────────────────────────────────────
    private static string CsvEscape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        // Wrap in quotes if value contains comma, quote, or newline
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
