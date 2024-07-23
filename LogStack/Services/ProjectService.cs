using LogStack.Domain;
using LogStack.Domain.Models;
using LogStack.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LogStack.Services;

public class ProjectService(AppDbContext dbContext, IUserService userService) : IProjectService
{
    public async Task<IEnumerable<Project>> GetALlProjects()
    {
        return await dbContext.Projects.ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetProjectsForUser(Ulid userId)
    {
        User? user = await userService.GetUserById(userId);
        if (user is null) return new List<Project>();

        if (user.Admin) return await GetALlProjects();

        List<Ulid> userHasAccessTo = await dbContext.HasAccess
            .Where(a => a.UserId == userId)
            .Select(a => a.ProjectId)
            .ToListAsync();

        return await dbContext.Projects
            .Where(p => userHasAccessTo.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<Project?> GetProjectById(Ulid projectId)
    {
        return await dbContext.Projects
            .Where(p => p.Id == projectId)
            .FirstOrDefaultAsync();
    }

    public async Task<Project> CreateProject(string name)
    {
        string secret = SecretHelper.GenerateSecret();
        Project newProject = new Project()
        {
            Name = name,
            Secret = secret
        };

        EntityEntry<Project> entry = await dbContext.Projects.AddAsync(newProject);
        await dbContext.SaveChangesAsync();
        
        return entry.Entity;
    }

    public async Task DeleteProject(Ulid projectId)
    {
        await dbContext.Projects.Where(p => p.Id == projectId).ExecuteDeleteAsync();
    }
}