namespace EnozomFinalTask.Domain.Entities;

public class TimeEntry
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int TaskItemId { get; set; }
    public int UserId { get; set; }
    
    // Navigation properties
    public virtual TaskItem TaskItem { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    
    // Computed property
    public decimal DurationHours => (decimal)(End - Start).TotalHours;
} 