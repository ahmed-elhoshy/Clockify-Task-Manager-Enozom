using EnozomFinalTask.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnozomFinalTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InitController : ControllerBase
{
    private readonly IDataSeedingService _dataSeedingService;
    private readonly ILogger<InitController> _logger;

    public InitController(IDataSeedingService dataSeedingService, ILogger<InitController> logger)
    {
        _dataSeedingService = dataSeedingService;
        _logger = logger;
    }

    [HttpPost("sample-data")]
    public async Task<IActionResult> SeedSampleData()
    {
        try
        {
            await _dataSeedingService.SeedSampleDataAsync();
            return Ok(new { message = "Sample data seeded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding sample data");
            return StatusCode(500, new { message = "Error seeding sample data", error = ex.Message });
        }
    }
} 