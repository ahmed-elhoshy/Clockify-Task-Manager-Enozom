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
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
    }

    public async Task SyncDataToClockifyAsync()
    {
        _logger.LogInformation("Starting Clockify sync...");

        var workspaceId = await GetWorkspaceIdAsync();
        if (string.IsNullOrEmpty(workspaceId))
        {
            throw new InvalidOperationException("No workspace found in Clockify");
        }

        // Sync projects first
        var projects = await _unitOfWork.Projects.GetAllAsync();
        foreach (var project in projects)
        {
            await CreateOrUpdateProjectInClockifyAsync(workspaceId, project);
        }

        // Then sync time entries  
        var timeEntries = await _unitOfWork.TimeEntries.GetAllAsync();
        foreach (var timeEntry in timeEntries)
        {
            await CreateTimeEntryInClockifyAsync(workspaceId, timeEntry);
        }

        _logger.LogInformation("Clockify sync completed successfully");
    }

    private async Task<string?> GetWorkspaceIdAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/workspaces");
        var content = await response.Content.ReadAsStringAsync();
        var workspaces = JsonConvert.DeserializeObject<List<dynamic>>(content);
        return workspaces?.FirstOrDefault()?.id?.ToString();
    }

    //private async Task CreateOrUpdateUserInClockifyAsync(string workspaceId, Domain.Entities.User user)
    //{
    //    try
    //    {
            
    //        var response = await _httpClient.GetAsync($"{_baseUrl}/workspaces/{workspaceId}/users");
    //        if (response.IsSuccessStatusCode)
    //        {
    //            var content = await response.Content.ReadAsStringAsync();
    //            var users = JsonConvert.DeserializeObject<List<dynamic>>(content);
                
    //            // Check if user already exists by name (you'd typically use email)
    //            var existingUser = users?.FirstOrDefault(u => u.name?.ToString() == user.FullName);
                
    //            if (existingUser != null)
    //            {
    //                _logger.LogInformation($"User {user.FullName} already exists in Clockify workspace");
    //            }
    //            else
    //            {
    //                _logger.LogWarning($"User {user.FullName} not found in Clockify workspace. Users must be invited by email first.");
                  
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"Error checking user in Clockify: {user.FullName}");
    //    }
    //}

    private async Task CreateOrUpdateProjectInClockifyAsync(string workspaceId, Domain.Entities.Project project)
    {
        var projectData = new
        {
            name = project.Name,
            isPublic = true,
            color = "#4285f4",
            billable = true
        };

        var json = JsonConvert.SerializeObject(projectData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_baseUrl}/workspaces/{workspaceId}/projects", content);
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Project synced: {project.Name}");
        }
    }



    private async Task CreateTimeEntryInClockifyAsync(string workspaceId, Domain.Entities.TimeEntry timeEntry)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(timeEntry.TaskItemId);
        var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId);
        var user = await _unitOfWork.Users.GetByIdAsync(timeEntry.UserId);

        // Get Clockify project ID
        var projectsResponse = await _httpClient.GetAsync($"{_baseUrl}/workspaces/{workspaceId}/projects");
        var projectsContent = await projectsResponse.Content.ReadAsStringAsync();
        var projects = JsonConvert.DeserializeObject<List<dynamic>>(projectsContent);
        var clockifyProject = projects?.FirstOrDefault(p => p.name?.ToString() == project.Name);
        var clockifyProjectId = clockifyProject?.id?.ToString();

        var timeEntryData = new
        {
            start = timeEntry.Start.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            end = timeEntry.End.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            projectId = clockifyProjectId,
            description = $"{task.Title} - {user.FullName}",
            billable = true
        };

        var json = JsonConvert.SerializeObject(timeEntryData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_baseUrl}/workspaces/{workspaceId}/time-entries", content);
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Time entry synced: {task.Title}");
        }
    }
} 