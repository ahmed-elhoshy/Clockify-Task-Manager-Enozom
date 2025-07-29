using EnozomFinalTask.Application.Services;
using EnozomFinalTask.Domain.Entities;
using EnozomFinalTask.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EnozomFinalTask.Infrastructure.Services;

public class DataSeedingService : IDataSeedingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DataSeedingService> _logger;

    public DataSeedingService(IUnitOfWork unitOfWork, ILogger<DataSeedingService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SeedSampleDataAsync()
    {
        try
        {
            // Check if data already exists
            var existingUsers = await _unitOfWork.Users.GetAllAsync();
            if (existingUsers.Any())
            {
                _logger.LogInformation("Sample data already exists, skipping seeding");
                return;
            }

            // Create Users
            var users = new List<User>
            {
                new User { FullName = "John Doe" },
                new User { FullName = "Jane Smith" },
                new User { FullName = "Bob Johnson" }
            };

            foreach (var user in users)
            {
                await _unitOfWork.Users.AddAsync(user);
            }

            // Create Projects
            var projects = new List<Project>
            {
                new Project { Name = "E-commerce Platform" },
                new Project { Name = "Mobile App Development" },
                new Project { Name = "API Integration" }
            };

            foreach (var project in projects)
            {
                await _unitOfWork.Projects.AddAsync(project);
            }

            await _unitOfWork.SaveChangesAsync();

            // Create Tasks
            var tasks = new List<TaskItem>
            {
                new TaskItem { Title = "Database Design", EstimateHours = 8, ProjectId = 1, AssignedUserId = 1 },
                new TaskItem { Title = "User Authentication", EstimateHours = 6, ProjectId = 1, AssignedUserId = 2 },
                new TaskItem { Title = "Payment Integration", EstimateHours = 10, ProjectId = 1, AssignedUserId = 1 },
                new TaskItem { Title = "UI/UX Design", EstimateHours = 12, ProjectId = 2, AssignedUserId = 2 },
                new TaskItem { Title = "API Development", EstimateHours = 8, ProjectId = 3, AssignedUserId = 3 },
                new TaskItem { Title = "Testing", EstimateHours = 4, ProjectId = 2, AssignedUserId = 1 }
            };

            foreach (var task in tasks)
            {
                await _unitOfWork.Tasks.AddAsync(task);
            }

            await _unitOfWork.SaveChangesAsync();

            // Create Time Entries
            var timeEntries = new List<TimeEntry>
            {
                new TimeEntry { Start = DateTime.Now.AddDays(-7).AddHours(9), End = DateTime.Now.AddDays(-7).AddHours(17), TaskItemId = 1, UserId = 1 },
                new TimeEntry { Start = DateTime.Now.AddDays(-6).AddHours(9), End = DateTime.Now.AddDays(-6).AddHours(15), TaskItemId = 2, UserId = 2 },
                new TimeEntry { Start = DateTime.Now.AddDays(-5).AddHours(10), End = DateTime.Now.AddDays(-5).AddHours(18), TaskItemId = 3, UserId = 1 },
                new TimeEntry { Start = DateTime.Now.AddDays(-4).AddHours(9), End = DateTime.Now.AddDays(-4).AddHours(17), TaskItemId = 4, UserId = 2 },
                new TimeEntry { Start = DateTime.Now.AddDays(-3).AddHours(8), End = DateTime.Now.AddDays(-3).AddHours(16), TaskItemId = 5, UserId = 3 },
                new TimeEntry { Start = DateTime.Now.AddDays(-2).AddHours(9), End = DateTime.Now.AddDays(-2).AddHours(13), TaskItemId = 6, UserId = 1 }
            };

            foreach (var timeEntry in timeEntries)
            {
                await _unitOfWork.TimeEntries.AddAsync(timeEntry);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Sample data seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding sample data");
            throw;
        }
    }
} 