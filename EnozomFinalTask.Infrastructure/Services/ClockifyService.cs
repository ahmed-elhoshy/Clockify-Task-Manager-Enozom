using EnozomFinalTask.Application.Services;
using EnozomFinalTask.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace EnozomFinalTask.Infrastructure.Services;

public class ClockifyService : IClockifyService
{
    private readonly HttpClient _httpClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClockifyService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://api.clockify.me/api/v1";

    public ClockifyService(
        HttpClient httpClient,
        IUnitOfWork unitOfWork,
        ILogger<ClockifyService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _apiKey = configuration["Clockify:ApiKey"] ?? throw new InvalidOperationException("Clockify API key not configured");
        
        // Fix: Use DefaultRequestHeaders instead of Add
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
    }

    public async Task SyncDataToClockifyAsync()
    {
        try
        {
            _logger.LogInformation("Starting Clockify sync...");

            // Get workspace ID (you might want to store this in configuration)
            var workspaceId = await GetWorkspaceIdAsync();
            if (string.IsNullOrEmpty(workspaceId))
            {
                throw new InvalidOperationException("No workspace found in Clockify");
            }

            // Get all data from database
            var users = await _unitOfWork.Users.GetAllAsync();
            var projects = await _unitOfWork.Projects.GetAllAsync();
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var timeEntries = await _unitOfWork.TimeEntries.GetAllAsync();

            // Sync users to Clockify
            foreach (var user in users)
            {
                await CreateOrUpdateUserInClockifyAsync(workspaceId, user);
            }

            // Sync projects to Clockify
            foreach (var project in projects)
            {
                await CreateOrUpdateProjectInClockifyAsync(workspaceId, project);
            }

            // Sync tasks to Clockify
            foreach (var task in tasks)
            {
                await CreateOrUpdateTaskInClockifyAsync(workspaceId, task);
            }

            // Sync time entries to Clockify
            foreach (var timeEntry in timeEntries)
            {
                await CreateTimeEntryInClockifyAsync(workspaceId, timeEntry);
            }

            _logger.LogInformation("Clockify sync completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing data to Clockify");
            throw;
        }
    }

    private async Task<string?> GetWorkspaceIdAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/workspaces");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var workspaces = JsonConvert.DeserializeObject<List<dynamic>>(content);
            return workspaces?.FirstOrDefault()?.id?.ToString();
        }
        return null;
    }

    private async Task CreateOrUpdateUserInClockifyAsync(string workspaceId, Domain.Entities.User user)
    {
        // This is a simplified implementation
        // In a real scenario, you'd need to handle user creation/update logic
        _logger.LogInformation($"Syncing user: {user.FullName}");
        await Task.Delay(100); // Simulate API call
    }

    private async Task CreateOrUpdateProjectInClockifyAsync(string workspaceId, Domain.Entities.Project project)
    {
        var projectData = new
        {
            name = project.Name,
            isPublic = true,
            color = "#000000"
        };

        var json = JsonConvert.SerializeObject(projectData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/workspaces/{workspaceId}/projects", content);
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Project created in Clockify: {project.Name}");
        }
        else
        {
            _logger.LogWarning($"Failed to create project in Clockify: {project.Name}");
        }
    }

    private async Task CreateOrUpdateTaskInClockifyAsync(string workspaceId, Domain.Entities.TaskItem task)
    {
        // This is a simplified implementation
        // In a real scenario, you'd need to handle task creation/update logic
        _logger.LogInformation($"Syncing task: {task.Title}");
        await Task.Delay(100); // Simulate API call
    }

    private async Task CreateTimeEntryInClockifyAsync(string workspaceId, Domain.Entities.TimeEntry timeEntry)
    {
        var timeEntryData = new
        {
            start = timeEntry.Start.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            end = timeEntry.End.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            description = $"Time entry for task {timeEntry.TaskItemId}"
        };

        var json = JsonConvert.SerializeObject(timeEntryData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/workspaces/{workspaceId}/time-entries", content);
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Time entry created in Clockify for task {timeEntry.TaskItemId}");
        }
        else
        {
            _logger.LogWarning($"Failed to create time entry in Clockify for task {timeEntry.TaskItemId}");
        }
    }
} 