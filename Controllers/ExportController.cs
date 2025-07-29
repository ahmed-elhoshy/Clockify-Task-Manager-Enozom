using EnozomFinalTask.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnozomFinalTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly ILogger<ExportController> _logger;

    public ExportController(IExportService exportService, ILogger<ExportController> logger)
    {
        _exportService = exportService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ExportTimeEntries()
    {
        try
        {
            var csvData = await _exportService.ExportTimeEntriesToCsvAsync();
            
            return File(
                csvData,
                "text/csv",
                $"time-entries-report-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.csv"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting time entries");
            return StatusCode(500, new { message = "Error exporting time entries", error = ex.Message });
        }
    }
} 