namespace EnozomFinalTask.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
} 