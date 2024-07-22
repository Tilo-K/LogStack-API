using LogStack.Domain.Models;

namespace LogStack.Services;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetALlProjects();
    Task<IEnumerable<Project>> GetProjectsForUser(Ulid userId);
    Task<Project?> GetProjectById(Ulid projectId);
    Task<Project> CreateProject(string name);
    Task DeleteProject(Ulid projectId);
}