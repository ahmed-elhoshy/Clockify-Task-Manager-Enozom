namespace EnozomFinalTask.Application.DTOs;

public class UserResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal EstimateHours { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int AssignedUserId { get; set; }
    public string AssignedUserName { get; set; } = string.Empty;
}

public class TimeEntryResponseDto
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public decimal DurationHours { get; set; }
    public int TaskItemId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class TaskDetailResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal EstimateHours { get; set; }
    public ProjectResponseDto Project { get; set; } = null!;
    public UserResponseDto AssignedUser { get; set; } = null!;
    public List<TimeEntryResponseDto> TimeEntries { get; set; } = new();
    public decimal TotalTimeSpent { get; set; }
}

public class ProjectDetailResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TaskResponseDto> Tasks { get; set; } = new();
    public decimal TotalEstimatedHours { get; set; }
    public decimal TotalTimeSpent { get; set; }
}

public class UserDetailResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public List<TaskResponseDto> AssignedTasks { get; set; } = new();
    public List<TimeEntryResponseDto> TimeEntries { get; set; } = new();
    public decimal TotalTimeSpent { get; set; }
} 