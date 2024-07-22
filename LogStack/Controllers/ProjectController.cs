using LogStack.Domain.Models;
using LogStack.Entities;
using LogStack.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogStack.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpGet("getProjects")]
    [TypeFilter(typeof(AuthCheckFilter))]
    [AuthCheck(NeedsToBeAdmin = false)]
    public async Task<IEnumerable<Project>> GetProjects()
    {
        Token token = Token.FromHttpContext(HttpContext);

        return await projectService.GetProjectsForUser(token.UserId);
    }

    [HttpPost("createProject")]
    [TypeFilter(typeof(AuthCheckFilter))]
    [AuthCheck(NeedsToBeAdmin = true)]
    public async Task<Project> CreateProject([FromQuery] string projectName)
    {
        return await projectService.CreateProject(projectName);
    }

    [HttpDelete("deleteProject")]
    [TypeFilter(typeof(AuthCheckFilter))]
    [AuthCheck(NeedsToBeAdmin = true)]
    public async Task DeleteProject([FromQuery] string projectId)
    {
        await projectService.DeleteProject(Ulid.Parse(projectId));
    }
}