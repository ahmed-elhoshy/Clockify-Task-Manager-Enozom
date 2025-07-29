using System.ComponentModel.DataAnnotations;

namespace EnozomFinalTask.Application.DTOs;

public class CreateTimeEntryDto
{
    [Required]
    public DateTime Start { get; set; }
    
    [Required]
    public DateTime End { get; set; }
    
    [Required]
    public int TaskItemId { get; set; }
    
    [Required]
    public int UserId { get; set; }
}

public class UpdateTimeEntryDto
{
    [Required]
    public DateTime Start { get; set; }
    
    [Required]
    public DateTime End { get; set; }
    
    [Required]
    public int TaskItemId { get; set; }
    
    [Required]
    public int UserId { get; set; }
} 