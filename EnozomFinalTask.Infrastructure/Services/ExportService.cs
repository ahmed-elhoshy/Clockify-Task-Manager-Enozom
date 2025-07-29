using CsvHelper;
using EnozomFinalTask.Application.DTOs;
using EnozomFinalTask.Application.Services;
using EnozomFinalTask.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace EnozomFinalTask.Infrastructure.Services;

public class ExportService : IExportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExportService> _logger;

    public ExportService(IUnitOfWork unitOfWork, ILogger<ExportService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<byte[]> ExportTimeEntriesToCsvAsync()
    {
        try
        {
            var timeEntries = await _unitOfWork.TimeEntries.GetAllAsync();
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();
            var projects = await _unitOfWork.Projects.GetAllAsync();

            var reportData = timeEntries.Select(te =>
            {
                var task = tasks.FirstOrDefault(t => t.Id == te.TaskItemId);
                var user = users.FirstOrDefault(u => u.Id == te.UserId);
                var project = projects.FirstOrDefault(p => p.Id == task?.ProjectId);

                return new TimeEntryReportDto
                {
                    User = user?.FullName ?? "Unknown",
                    Project = project?.Name ?? "Unknown",
                    Task = task?.Title ?? "Unknown",
                    OriginalEstimate = task?.EstimateHours ?? 0,
                    TimeSpent = te.DurationHours
                };
            }).ToList();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(reportData);
            await writer.FlushAsync();

            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting time entries to CSV");
            throw;
        }
    }
} 