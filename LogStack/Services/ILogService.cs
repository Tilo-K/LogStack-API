using LogStack.Domain.Models;
using LogStack.Entities;

namespace LogStack.Services;

public interface ILogService
{
    Task<Log> AddLogEntry(Ulid projectId, string logLevel, string origin, DateTime time, string content);
    Task<long> CountLogs(string? logLevel = null, Ulid? projectId = null, string? origin = null);

    Task<IEnumerable<Log>> GetLogs(Ulid? projectId = null, string? logLevel = null, string? origin = null,
        string? content = null, int top = 100, int skip = 0);

    Task DeleteOldLogs(DateTime timeLimit);

    Task<LogFilterOptions> GetFilterOptions(Ulid projectId);
}