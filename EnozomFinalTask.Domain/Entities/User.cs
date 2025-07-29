namespace EnozomFinalTask.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
} 