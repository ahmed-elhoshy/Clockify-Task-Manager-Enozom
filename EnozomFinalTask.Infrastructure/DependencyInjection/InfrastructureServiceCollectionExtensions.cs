using EnozomFinalTask.Application.Services;
using EnozomFinalTask.Domain.Interfaces;
using EnozomFinalTask.Infrastructure.Data;
using EnozomFinalTask.Infrastructure.Repositories;
using EnozomFinalTask.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnozomFinalTask.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
            ));

        // Add Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add Services
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IDataSeedingService, DataSeedingService>();
        services.AddScoped<IClockifyService, ClockifyService>();

        // Add HttpClient for Clockify
        services.AddHttpClient<IClockifyService, ClockifyService>();

        return services;
    }
} 