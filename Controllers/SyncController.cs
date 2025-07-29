using EnozomFinalTask.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnozomFinalTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly IClockifyService _clockifyService;
    private readonly ILogger<SyncController> _logger;

    public SyncController(IClockifyService clockifyService, ILogger<SyncController> logger)
    {
        _clockifyService = clockifyService;
        _logger = logger;
    }

    [HttpPost("clockify")]
    public async Task<IActionResult> SyncToClockify()
    {
        try
        {
            await _clockifyService.SyncDataToClockifyAsync();
            return Ok(new { message = "Data synchronized to Clockify successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing data to Clockify");
            return StatusCode(500, new { message = "Error syncing data to Clockify", error = ex.Message });
        }
    }
} 