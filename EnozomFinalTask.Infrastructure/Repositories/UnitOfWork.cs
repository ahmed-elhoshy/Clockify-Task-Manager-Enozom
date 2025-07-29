using EnozomFinalTask.Domain.Entities;
using EnozomFinalTask.Domain.Interfaces;
using EnozomFinalTask.Infrastructure.Data;

namespace EnozomFinalTask.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<User> _users;
    private readonly IRepository<Project> _projects;
    private readonly IRepository<TaskItem> _tasks;
    private readonly IRepository<TimeEntry> _timeEntries;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _users = new Repository<User>(context);
        _projects = new Repository<Project>(context);
        _tasks = new Repository<TaskItem>(context);
        _timeEntries = new Repository<TimeEntry>(context);
    }

    public IRepository<User> Users => _users;
    public IRepository<Project> Projects => _projects;
    public IRepository<TaskItem> Tasks => _tasks;
    public IRepository<TimeEntry> TimeEntries => _timeEntries;

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
} 