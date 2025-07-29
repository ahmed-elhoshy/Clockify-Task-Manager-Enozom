namespace EnozomFinalTask.Application.DTOs;

public class TimeEntryReportDto
{
    public string User { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Task { get; set; } = string.Empty;
    public decimal OriginalEstimate { get; set; }
    public decimal TimeSpent { get; set; }
} 