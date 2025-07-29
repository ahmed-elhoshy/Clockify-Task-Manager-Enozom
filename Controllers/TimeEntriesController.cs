using EnozomFinalTask.Application.DTOs;
using EnozomFinalTask.Domain.Entities;
using EnozomFinalTask.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnozomFinalTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeEntriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TimeEntriesController> _logger;

    public TimeEntriesController(IUnitOfWork unitOfWork, ILogger<TimeEntriesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTimeEntries()
    {
        try
        {
            var timeEntries = await _unitOfWork.TimeEntries.GetAllAsync();
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();

            var response = timeEntries.Select(te => new TimeEntryResponseDto
            {
                Id = te.Id,
                Start = te.Start,
                End = te.End,
                DurationHours = te.DurationHours,
                TaskItemId = te.TaskItemId,
                TaskTitle = tasks.FirstOrDefault(t => t.Id == te.TaskItemId)?.Title ?? "Unknown",
                UserId = te.UserId,
                UserName = users.FirstOrDefault(u => u.Id == te.UserId)?.FullName ?? "Unknown"
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entries");
            return StatusCode(500, new { message = "Error retrieving time entries", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTimeEntry(int id)
    {
        try
        {
            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(id);
            if (timeEntry == null)
                return NotFound(new { message = $"Time entry with ID {id} not found" });

            var task = await _unitOfWork.Tasks.GetByIdAsync(timeEntry.TaskItemId);
            var user = await _unitOfWork.Users.GetByIdAsync(timeEntry.UserId);

            var response = new TimeEntryResponseDto
            {
                Id = timeEntry.Id,
                Start = timeEntry.Start,
                End = timeEntry.End,
                DurationHours = timeEntry.DurationHours,
                TaskItemId = timeEntry.TaskItemId,
                TaskTitle = task?.Title ?? "Unknown",
                UserId = timeEntry.UserId,
                UserName = user?.FullName ?? "Unknown"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entry with ID {TimeEntryId}", id);
            return StatusCode(500, new { message = "Error retrieving time entry", error = ex.Message });
        }
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetTimeEntriesByTask(int taskId)
    {
        try
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
                return NotFound(new { message = $"Task with ID {taskId} not found" });

            var timeEntries = await _unitOfWork.TimeEntries.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();
            var taskTimeEntries = timeEntries.Where(te => te.TaskItemId == taskId).ToList();
            
            var response = taskTimeEntries.Select(te => new TimeEntryResponseDto
            {
                Id = te.Id,
                Start = te.Start,
                End = te.End,
                DurationHours = te.DurationHours,
                TaskItemId = te.TaskItemId,
                TaskTitle = task.Title,
                UserId = te.UserId,
                UserName = users.FirstOrDefault(u => u.Id == te.UserId)?.FullName ?? "Unknown"
            }).ToList();
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entries for task {TaskId}", taskId);
            return StatusCode(500, new { message = "Error retrieving time entries for task", error = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTimeEntriesByUser(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = $"User with ID {userId} not found" });

            var timeEntries = await _unitOfWork.TimeEntries.GetAllAsync();
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var userTimeEntries = timeEntries.Where(te => te.UserId == userId).ToList();
            
            var response = userTimeEntries.Select(te => new TimeEntryResponseDto
            {
                Id = te.Id,
                Start = te.Start,
                End = te.End,
                DurationHours = te.DurationHours,
                TaskItemId = te.TaskItemId,
                TaskTitle = tasks.FirstOrDefault(t => t.Id == te.TaskItemId)?.Title ?? "Unknown",
                UserId = te.UserId,
                UserName = user.FullName
            }).ToList();
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time entries for user {UserId}", userId);
            return StatusCode(500, new { message = "Error retrieving time entries for user", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTimeEntry([FromBody] CreateTimeEntryDto createTimeEntryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate that the task exists
            var task = await _unitOfWork.Tasks.GetByIdAsync(createTimeEntryDto.TaskItemId);
            if (task == null)
                return BadRequest(new { message = $"Task with ID {createTimeEntryDto.TaskItemId} not found" });

            // Validate that the user exists
            var user = await _unitOfWork.Users.GetByIdAsync(createTimeEntryDto.UserId);
            if (user == null)
                return BadRequest(new { message = $"User with ID {createTimeEntryDto.UserId} not found" });

            // Validate that end time is after start time
            if (createTimeEntryDto.End <= createTimeEntryDto.Start)
                return BadRequest(new { message = "End time must be after start time" });

            var timeEntry = new TimeEntry
            {
                Start = createTimeEntryDto.Start,
                End = createTimeEntryDto.End,
                TaskItemId = createTimeEntryDto.TaskItemId,
                UserId = createTimeEntryDto.UserId
            };

            await _unitOfWork.TimeEntries.AddAsync(timeEntry);
            await _unitOfWork.SaveChangesAsync();

            // Create clean response DTO
            var response = new TimeEntryResponseDto
            {
                Id = timeEntry.Id,
                Start = timeEntry.Start,
                End = timeEntry.End,
                DurationHours = timeEntry.DurationHours,
                TaskItemId = timeEntry.TaskItemId,
                TaskTitle = task.Title,
                UserId = timeEntry.UserId,
                UserName = user.FullName
            };

            _logger.LogInformation("Time entry created successfully with ID {TimeEntryId}", timeEntry.Id);
            return CreatedAtAction(nameof(GetTimeEntry), new { id = timeEntry.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating time entry");
            return StatusCode(500, new { message = "Error creating time entry", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTimeEntry(int id, [FromBody] UpdateTimeEntryDto updateTimeEntryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(id);
            if (timeEntry == null)
                return NotFound(new { message = $"Time entry with ID {id} not found" });

            // Validate that the task exists
            var task = await _unitOfWork.Tasks.GetByIdAsync(updateTimeEntryDto.TaskItemId);
            if (task == null)
                return BadRequest(new { message = $"Task with ID {updateTimeEntryDto.TaskItemId} not found" });

            // Validate that the user exists
            var user = await _unitOfWork.Users.GetByIdAsync(updateTimeEntryDto.UserId);
            if (user == null)
                return BadRequest(new { message = $"User with ID {updateTimeEntryDto.UserId} not found" });

            // Validate that end time is after start time
            if (updateTimeEntryDto.End <= updateTimeEntryDto.Start)
                return BadRequest(new { message = "End time must be after start time" });

            timeEntry.Start = updateTimeEntryDto.Start;
            timeEntry.End = updateTimeEntryDto.End;
            timeEntry.TaskItemId = updateTimeEntryDto.TaskItemId;
            timeEntry.UserId = updateTimeEntryDto.UserId;

            await _unitOfWork.TimeEntries.UpdateAsync(timeEntry);
            await _unitOfWork.SaveChangesAsync();

            // Create clean response DTO
            var response = new TimeEntryResponseDto
            {
                Id = timeEntry.Id,
                Start = timeEntry.Start,
                End = timeEntry.End,
                DurationHours = timeEntry.DurationHours,
                TaskItemId = timeEntry.TaskItemId,
                TaskTitle = task.Title,
                UserId = timeEntry.UserId,
                UserName = user.FullName
            };

            _logger.LogInformation("Time entry with ID {TimeEntryId} updated successfully", id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating time entry with ID {TimeEntryId}", id);
            return StatusCode(500, new { message = "Error updating time entry", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTimeEntry(int id)
    {
        try
        {
            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(id);
            if (timeEntry == null)
                return NotFound(new { message = $"Time entry with ID {id} not found" });

            await _unitOfWork.TimeEntries.DeleteAsync(timeEntry);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Time entry with ID {TimeEntryId} deleted successfully", id);
            return Ok(new { message = $"Time entry with ID {id} deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting time entry with ID {TimeEntryId}", id);
            return StatusCode(500, new { message = "Error deleting time entry", error = ex.Message });
        }
    }
} 