using EnozomFinalTask.Application.DTOs;
using EnozomFinalTask.Domain.Entities;
using EnozomFinalTask.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnozomFinalTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUnitOfWork unitOfWork, ILogger<UsersController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var response = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                FullName = u.FullName
            }).ToList();
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, new { message = "Error retrieving users", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found" });

            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var projects = await _unitOfWork.Projects.GetAllAsync();
            var timeEntries = await _unitOfWork.TimeEntries.GetAllAsync();
            
            var userTasks = tasks.Where(t => t.AssignedUserId == id).ToList();
            var userTimeEntries = timeEntries.Where(te => te.UserId == id).ToList();

            var response = new UserDetailResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                AssignedTasks = userTasks.Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    EstimateHours = t.EstimateHours,
                    ProjectId = t.ProjectId,
                    ProjectName = projects.FirstOrDefault(p => p.Id == t.ProjectId)?.Name ?? "Unknown",
                    AssignedUserId = t.AssignedUserId,
                    AssignedUserName = user.FullName
                }).ToList(),
                TimeEntries = userTimeEntries.Select(te => new TimeEntryResponseDto
                {
                    Id = te.Id,
                    Start = te.Start,
                    End = te.End,
                    DurationHours = te.DurationHours,
                    TaskItemId = te.TaskItemId,
                    TaskTitle = tasks.FirstOrDefault(t => t.Id == te.TaskItemId)?.Title ?? "Unknown",
                    UserId = te.UserId,
                    UserName = user.FullName
                }).ToList(),
                TotalTimeSpent = userTimeEntries.Sum(te => te.DurationHours)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            return StatusCode(500, new { message = "Error retrieving user", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                FullName = createUserDto.FullName
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName
            };

            _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { message = "Error creating user", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found" });

            user.FullName = updateUserDto.FullName;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName
            };

            _logger.LogInformation("User with ID {UserId} updated successfully", id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", id);
            return StatusCode(500, new { message = "Error updating user", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found" });

            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User with ID {UserId} deleted successfully", id);
            return Ok(new { message = $"User with ID {id} deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            return StatusCode(500, new { message = "Error deleting user", error = ex.Message });
        }
    }
} 