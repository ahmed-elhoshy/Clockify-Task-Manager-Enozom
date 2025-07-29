using EnozomFinalTask.Application.DTOs;
using EnozomFinalTask.Domain.Entities;
using EnozomFinalTask.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EnozomFinalTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IUnitOfWork unitOfWork, ILogger<ProjectsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetAllAsync();
            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects");
            return StatusCode(500, new { message = "Error retrieving projects", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null)
                return NotFound(new { message = $"Project with ID {id} not found" });

            return Ok(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project with ID {ProjectId}", id);
            return StatusCode(500, new { message = "Error retrieving project", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = new Project
            {
                Name = createProjectDto.Name
            };

            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project created successfully with ID {ProjectId}", project.Id);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return StatusCode(500, new { message = "Error creating project", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updateProjectDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null)
                return NotFound(new { message = $"Project with ID {id} not found" });

            project.Name = updateProjectDto.Name;

            await _unitOfWork.Projects.UpdateAsync(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project with ID {ProjectId} updated successfully", id);
            return Ok(project);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project with ID {ProjectId}", id);
            return StatusCode(500, new { message = "Error updating project", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
            if (project == null)
                return NotFound(new { message = $"Project with ID {id} not found" });

            await _unitOfWork.Projects.DeleteAsync(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project with ID {ProjectId} deleted successfully", id);
            return Ok(new { message = $"Project with ID {id} deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project with ID {ProjectId}", id);
            return StatusCode(500, new { message = "Error deleting project", error = ex.Message });
        }
    }
} 