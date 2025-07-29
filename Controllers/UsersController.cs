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
            return Ok(users);
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

            return Ok(user);
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

            _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
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

            _logger.LogInformation("User with ID {UserId} updated successfully", id);
            return Ok(user);
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