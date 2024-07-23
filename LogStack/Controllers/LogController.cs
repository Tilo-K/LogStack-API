using LogStack.Domain.Models;
using LogStack.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogStack.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController(ILogService logService, IProjectService projectService) : ControllerBase
{
    private async Task<bool> IsValidLogRequest(HttpContext context, string projectId)
    {
        string? secret = HttpContext.Request.Headers.Authorization.FirstOrDefault();
        if (secret is null) return false;

        Project? project = await projectService.GetProjectById(Ulid.Parse(projectId));
        if (project is null) return false;

        if (project.Secret != secret) return false;

        return true;
    }

    [HttpPost("createLog")]
    public async Task<ActionResult<Log>> CreateLog([FromQuery] string projectId, [FromQuery] string logLevel,
        [FromQuery] string origin, [FromQuery] DateTime time, [FromQuery] string content)
    {
        bool validRequest = await IsValidLogRequest(HttpContext, projectId);
        if (!validRequest) return BadRequest();

        Log log = await logService.AddLogEntry(Ulid.Parse(projectId), logLevel, projectId, time, content);

        return Ok(log);
    }

    [HttpGet("getLogs")]
    public async Task<ActionResult<IEnumerable<Log>>> GetLogs([FromQuery] string secret, [FromQuery] string projectId,
        [FromQuery] string? logLevel = null, [FromQuery] string? origin = null,
        [FromQuery] string? content = null, [FromQuery] int top = 100, [FromQuery] int skip = 0)
    {
        bool validRequest = await IsValidLogRequest(HttpContext, projectId);
        if (!validRequest) return BadRequest();

        IEnumerable<Log> logs = await logService.GetLogs(Ulid.Parse(projectId), logLevel, origin, content, top, skip);

        return Ok(logs);
    }
    
    [HttpGet("getLogCount")]
    public async Task<ActionResult<long>> GetLogCount([FromQuery] string projectId)
    {
        bool validRequest = await IsValidLogRequest(HttpContext, projectId);
        if (!validRequest) return BadRequest();

        long count = await logService.CountLogs(projectId: Ulid.Parse(projectId));

        return Ok(count);
    }
}