using EnozomFinalTask.Domain.Entities;

namespace EnozomFinalTask.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Project> Projects { get; }
    IRepository<TaskItem> Tasks { get; }
    IRepository<TimeEntry> TimeEntries { get; }
    Task<int> SaveChangesAsync();
} 