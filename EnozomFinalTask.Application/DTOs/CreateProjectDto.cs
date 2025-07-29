using System.ComponentModel.DataAnnotations;

namespace EnozomFinalTask.Application.DTOs;

public class CreateProjectDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateProjectDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
} 