using EnozomFinalTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnozomFinalTask.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.AssignedUser)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimeEntry>()
            .HasOne(te => te.TaskItem)
            .WithMany(t => t.TimeEntries)
            .HasForeignKey(te => te.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimeEntry>()
            .HasOne(te => te.User)
            .WithMany(u => u.TimeEntries)
            .HasForeignKey(te => te.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 