using LogStack.Domain;
using LogStack.Domain.Models;
using LogStack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LogStack.Services;

public class LogService(AppDbContext dbContext) : ILogService
{
    public async Task<Log> AddLogEntry(Ulid projectId, string logLevel, string origin, DateTime time, string content)
    {
        Log newLog = new Log()
        {
            Origin = origin,
            Time = time,
            Content = content,
            ProjectId = projectId,
            LogLevel = logLevel,
            Day = time.Day,
            Month = time.Month,
            Year = time.Year
        };

        EntityEntry<Log> addedLog = await dbContext.Logs.AddAsync(newLog);
        await dbContext.SaveChangesAsync();
        return addedLog.Entity;
    }

    public async Task<long> CountLogs(string? logLevel = null, Ulid? projectId = null, string? origin = null)
    {
        IQueryable<Log> logs = dbContext.Logs.AsQueryable();

        if (logLevel is not null)
        {
            logs = logs.Where(l => l.LogLevel == logLevel);
        }

        if (projectId is not null)
        {
            logs = logs.Where(l => l.ProjectId == projectId);
        }

        if (origin is not null)
        {
            logs = logs.Where(l => l.Origin == origin);
        }

        return await logs.LongCountAsync();
    }

    public async Task<IEnumerable<Log>> GetLogs(Ulid? projectId = null, string? logLevel = null, string? origin = null,
        string? content = null, int top = 100, int skip = 0)
    {
        IQueryable<Log> logs = dbContext.Logs.AsQueryable();

        if (logLevel is not null)
        {
            logs = logs.Where(l => l.LogLevel == logLevel);
        }

        if (projectId is not null)
        {
            logs = logs.Where(l => l.ProjectId == projectId);
        }

        if (origin is not null)
        {
            logs = logs.Where(l => l.Origin == origin);
        }

        if (content is not null)
        {
            logs = logs.Where(l => l.Content.Contains(content));
        }

        logs = logs.OrderByDescending(l => l.Time);
        logs = logs.Skip(skip);
        logs = logs.Take(top);

        return await logs.ToListAsync();
    }

    public async Task<LogFilterOptions> GetFilterOptions(Ulid projectId)
    {
        LogFilterOptions options = new LogFilterOptions();
        options.LogLevels = await dbContext.Logs
            .Where(l => l.ProjectId == projectId)
            .Select(l => l.LogLevel)
            .Distinct()
            .ToListAsync();
        options.Origins = await dbContext.Logs
            .Where(l => l.ProjectId == projectId)
            .Select(l => l.Origin)
            .Distinct()
            .ToListAsync();

        return options;
    }

    public async Task DeleteOldLogs(DateTime timeLimit)
    {
        await dbContext.Logs.Where(l => l.Time > timeLimit).ExecuteDeleteAsync();
    }
}