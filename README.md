# Enozom Final Task - Clean Architecture Web API

A Web API project built with ASP.NET Core using Clean Architecture principles, featuring MySQL database integration, Entity Framework Core (Code-First), and Clockify API integration.

## 🏗️ Architecture

This project follows Clean Architecture principles with the following layers:

- **Domain**: Core entities and interfaces
- **Application**: Business logic, DTOs, and service interfaces
- **Infrastructure**: Data access, external services, and implementations
- **Web API**: Controllers and application entry point

## 🚀 Features

- **Clean Architecture**: Separation of concerns with proper dependency direction
- **Entity Framework Core**: Code-First approach with MySQL database
- **Repository Pattern**: Generic repository implementation with Unit of Work
- **Clockify Integration**: REST API integration for time tracking
- **CSV Export**: Time entries export functionality using CsvHelper
- **Dependency Injection**: Proper service registration and configuration

## 📋 Prerequisites

- .NET 8.0 SDK
- MySQL Server
- Clockify API Key

## 🛠️ Setup Instructions

### 1. Database Setup

1. Install and configure MySQL Server
2. Create a new database named `EnozomFinalTask`
3. Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EnozomFinalTask;User=your_username;Password=your_password;"
  }
}
```

### 2. Clockify Configuration

1. Get your Clockify API key from your Clockify account
2. Update the API key in `appsettings.json`:

```json
{
  "Clockify": {
    "ApiKey": "your_clockify_api_key_here"
  }
}
```

### 3. Database Migration

Run the following commands to create and apply the database migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Build and Run

```bash
dotnet restore
dotnet build
dotnet run
```

The API will be available at `https://localhost:7001` (or the configured port).

## 📚 API Endpoints

### 1. Seed Sample Data

```
POST /api/init/sample-data
```

Seeds the database with sample users, projects, tasks, and time entries.

### 2. Sync to Clockify

```
POST /api/sync/clockify
```

Synchronizes all current data (users, projects, tasks, time entries) to Clockify via REST API.

### 3. Export Time Entries

```
GET /api/export
```

Returns a CSV file containing all time entries with the following format:

- User
- Project
- Task
- OriginalEstimate (hrs)
- TimeSpent (hrs)

## 🗄️ Database Schema

### Entities

- **User**: Id, FullName
- **Project**: Id, Name
- **TaskItem**: Id, Title, EstimateHours, ProjectId, AssignedUserId
- **TimeEntry**: Id, Start, End, TaskItemId, UserId

### Relationships

- Project has many Tasks
- User has many AssignedTasks and TimeEntries
- TaskItem belongs to Project and AssignedUser
- TimeEntry belongs to TaskItem and User

## 🔧 Project Structure

```
EnozomFinalTask/
├── EnozomFinalTask.Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Project.cs
│   │   ├── TaskItem.cs
│   │   └── TimeEntry.cs
│   └── Interfaces/
│       ├── IRepository.cs
│       └── IUnitOfWork.cs
├── EnozomFinalTask.Application/
│   ├── DTOs/
│   │   └── TimeEntryReportDto.cs
│   └── Services/
│       ├── IExportService.cs
│       ├── IDataSeedingService.cs
│       └── IClockifyService.cs
├── EnozomFinalTask.Infrastructure/
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Repositories/
│   │   ├── Repository.cs
│   │   └── UnitOfWork.cs
│   ├── Services/
│   │   ├── ExportService.cs
│   │   ├── DataSeedingService.cs
│   │   └── ClockifyService.cs
│   └── DependencyInjection/
│       └── InfrastructureServiceCollectionExtensions.cs
└── Controllers/
    ├── InitController.cs
    ├── SyncController.cs
    └── ExportController.cs
```

## 🧪 Testing the API

1. **Seed Sample Data**:

   ```bash
   curl -X POST https://localhost:7001/api/init/sample-data
   ```

2. **Export Time Entries**:

   ```bash
   curl -X GET https://localhost:7001/api/export -o time-entries.csv
   ```

3. **Sync to Clockify** (requires valid API key):
   ```bash
   curl -X POST https://localhost:7001/api/sync/clockify
   ```

## 📦 Dependencies

- **Microsoft.EntityFrameworkCore**: ORM framework
- **Pomelo.EntityFrameworkCore.MySql**: MySQL provider for EF Core
- **CsvHelper**: CSV generation library
- **Newtonsoft.Json**: JSON serialization
- **Microsoft.Extensions.Http**: HTTP client factory
- **Swashbuckle.AspNetCore**: API documentation

## 🔒 Security Notes

- Store sensitive configuration (database passwords, API keys) in user secrets or environment variables in production
- Implement proper authentication and authorization for production use
- Use HTTPS in production environments

## 🚀 Deployment

The application can be deployed to various platforms:

- Azure App Service
- AWS Elastic Beanstalk
- Docker containers
- On-premises servers

Make sure to update the connection strings and API keys for the target environment.
