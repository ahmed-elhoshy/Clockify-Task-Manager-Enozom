using System.ComponentModel.DataAnnotations;

namespace EnozomFinalTask.Application.DTOs;

public class CreateTaskDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [Range(0.1, 1000)]
    public decimal EstimateHours { get; set; }
    
    [Required]
    public int ProjectId { get; set; }
    
    [Required]
    public int AssignedUserId { get; set; }
}

public class UpdateTaskDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [Range(0.1, 1000)]
    public decimal EstimateHours { get; set; }
    
    [Required]
    public int ProjectId { get; set; }
    
    [Required]
    public int AssignedUserId { get; set; }
} 