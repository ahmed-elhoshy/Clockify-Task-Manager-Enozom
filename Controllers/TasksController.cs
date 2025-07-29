using EnozomFinalTask.Application.DTOs;
using EnozomFinalTask.Domain.Entities;
using EnozomFinalTask.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnozomFinalTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TasksController> _logger;

    public TasksController(IUnitOfWork unitOfWork, ILogger<TasksController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
        try
        {
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var projects = await _unitOfWork.Projects.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();

            var response = tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                EstimateHours = t.EstimateHours,
                ProjectId = t.ProjectId,
                ProjectName = projects.FirstOrDefault(p => p.Id == t.ProjectId)?.Name ?? "Unknown",
                AssignedUserId = t.AssignedUserId,
                AssignedUserName = users.FirstOrDefault(u => u.Id == t.AssignedUserId)?.FullName ?? "Unknown"
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks");
            return StatusCode(500, new { message = "Error retrieving tasks", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        try
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
                return NotFound(new { message = $"Task with ID {id} not found" });

            var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
            var user = await _unitOfWork.Users.GetByIdAsync(task.AssignedUserId);
            var timeEntries = await _unitOfWork.TimeEntries.GetAllAsync();
            var taskTimeEntries = timeEntries.Where(te => te.TaskItemId == task.Id).ToList();

            var response = new TaskDetailResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                EstimateHours = task.EstimateHours,
                Project = new ProjectResponseDto { Id = project.Id, Name = project.Name },
                AssignedUser = new UserResponseDto { Id = user.Id, FullName = user.FullName },
                TimeEntries = taskTimeEntries.Select(te => new TimeEntryResponseDto
                {
                    Id = te.Id,
                    Start = te.Start,
                    End = te.End,
                    DurationHours = te.DurationHours,
                    TaskItemId = te.TaskItemId,
                    TaskTitle = task.Title,
                    UserId = te.UserId,
                    UserName = user.FullName
                }).ToList(),
                TotalTimeSpent = taskTimeEntries.Sum(te => te.DurationHours)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task with ID {TaskId}", id);
            return StatusCode(500, new { message = "Error retrieving task", error = ex.Message });
        }
    }

    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetTasksByProject(int projectId)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
            if (project == null)
                return NotFound(new { message = $"Project with ID {projectId} not found" });

            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();
            var projectTasks = tasks.Where(t => t.ProjectId == projectId).ToList();
            
            var response = projectTasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                EstimateHours = t.EstimateHours,
                ProjectId = t.ProjectId,
                ProjectName = project.Name,
                AssignedUserId = t.AssignedUserId,
                AssignedUserName = users.FirstOrDefault(u => u.Id == t.AssignedUserId)?.FullName ?? "Unknown"
            }).ToList();
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for project {ProjectId}", projectId);
            return StatusCode(500, new { message = "Error retrieving tasks for project", error = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetTasksByUser(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = $"User with ID {userId} not found" });

            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var projects = await _unitOfWork.Projects.GetAllAsync();
            var userTasks = tasks.Where(t => t.AssignedUserId == userId).ToList();
            
            var response = userTasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                EstimateHours = t.EstimateHours,
                ProjectId = t.ProjectId,
                ProjectName = projects.FirstOrDefault(p => p.Id == t.ProjectId)?.Name ?? "Unknown",
                AssignedUserId = t.AssignedUserId,
                AssignedUserName = user.FullName
            }).ToList();
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for user {UserId}", userId);
            return StatusCode(500, new { message = "Error retrieving tasks for user", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate that the project exists
            var project = await _unitOfWork.Projects.GetByIdAsync(createTaskDto.ProjectId);
            if (project == null)
                return BadRequest(new { message = $"Project with ID {createTaskDto.ProjectId} not found" });

            // Validate that the user exists
            var user = await _unitOfWork.Users.GetByIdAsync(createTaskDto.AssignedUserId);
            if (user == null)
                return BadRequest(new { message = $"User with ID {createTaskDto.AssignedUserId} not found" });

            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                EstimateHours = createTaskDto.EstimateHours,
                ProjectId = createTaskDto.ProjectId,
                AssignedUserId = createTaskDto.AssignedUserId
            };

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            // Create response DTO with clean data
            var response = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                EstimateHours = task.EstimateHours,
                ProjectId = task.ProjectId,
                ProjectName = project.Name,
                AssignedUserId = task.AssignedUserId,
                AssignedUserName = user.FullName
            };

            _logger.LogInformation("Task created successfully with ID {TaskId}", task.Id);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(500, new { message = "Error creating task", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
                return NotFound(new { message = $"Task with ID {id} not found" });

            // Validate that the project exists
            var project = await _unitOfWork.Projects.GetByIdAsync(updateTaskDto.ProjectId);
            if (project == null)
                return BadRequest(new { message = $"Project with ID {updateTaskDto.ProjectId} not found" });

            // Validate that the user exists
            var user = await _unitOfWork.Users.GetByIdAsync(updateTaskDto.AssignedUserId);
            if (user == null)
                return BadRequest(new { message = $"User with ID {updateTaskDto.AssignedUserId} not found" });

            task.Title = updateTaskDto.Title;
            task.EstimateHours = updateTaskDto.EstimateHours;
            task.ProjectId = updateTaskDto.ProjectId;
            task.AssignedUserId = updateTaskDto.AssignedUserId;

            await _unitOfWork.Tasks.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();

            // Create response DTO with clean data
            var response = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                EstimateHours = task.EstimateHours,
                ProjectId = task.ProjectId,
                ProjectName = project.Name,
                AssignedUserId = task.AssignedUserId,
                AssignedUserName = user.FullName
            };

            _logger.LogInformation("Task with ID {TaskId} updated successfully", id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task with ID {TaskId}", id);
            return StatusCode(500, new { message = "Error updating task", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
                return NotFound(new { message = $"Task with ID {id} not found" });

            await _unitOfWork.Tasks.DeleteAsync(task);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task with ID {TaskId} deleted successfully", id);
            return Ok(new { message = $"Task with ID {id} deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task with ID {TaskId}", id);
            return StatusCode(500, new { message = "Error deleting task", error = ex.Message });
        }
    }
} 