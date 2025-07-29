using System.ComponentModel.DataAnnotations;

namespace EnozomFinalTask.Application.DTOs;

public class CreateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;
} 