namespace EnozomFinalTask.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal EstimateHours { get; set; }
    public int ProjectId { get; set; }
    public int AssignedUserId { get; set; }
    
    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual User AssignedUser { get; set; } = null!;
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
} 