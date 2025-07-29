using EnozomFinalTask.Application.DTOs;

namespace EnozomFinalTask.Application.Services;
 
public interface IExportService
{
    Task<byte[]> ExportTimeEntriesToCsvAsync();
} 