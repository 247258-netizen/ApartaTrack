using ApartaTrack.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApartaTrack.Controllers;

/// <summary>
/// MVC controller that serves CSV file downloads.
/// Blazor pages link to these endpoints using <a href="/export/..."> or JS navigation.
/// </summary>
[Route("export")]
public class ExportController : Controller
{
    private readonly ExportService _exportService;

    public ExportController(ExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpGet("payments")]
    public async Task<IActionResult> Payments()
    {
        var bytes = await _exportService.ExportPaymentsCsvAsync();
        return File(bytes, "text/csv", $"Payments_{DateTime.Today:yyyy-MM-dd}.csv");
    }

    [HttpGet("tenants")]
    public async Task<IActionResult> Tenants()
    {
        var bytes = await _exportService.ExportTenantsCsvAsync();
        return File(bytes, "text/csv", $"Tenants_{DateTime.Today:yyyy-MM-dd}.csv");
    }

    [HttpGet("maintenance")]
    public async Task<IActionResult> Maintenance()
    {
        var bytes = await _exportService.ExportMaintenanceCsvAsync();
        return File(bytes, "text/csv", $"Maintenance_{DateTime.Today:yyyy-MM-dd}.csv");
    }

    [HttpGet("revenue/{year:int}")]
    public async Task<IActionResult> Revenue(int year)
    {
        var bytes = await _exportService.ExportMonthlyRevenueCsvAsync(year);
        return File(bytes, "text/csv", $"Revenue_{year}.csv");
    }
}
