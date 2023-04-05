using IssueTracker.Models;

namespace IssueTracker.Services.Interfaces;

public interface IITProjectService
{
    public Task AddNewProjectAsync(Project project);

    public Task<bool> AddProjectManagerAsync(string userId, int projectId);

    public Task<bool> AddUserToProjectAsync(string userId, int projectId);

    public Task ArchiveProjectAsync(Project project);

    public Task<List<Project>> GetAllProjectsByCompany(int companyId);

    public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName);

    public Task<List<ITUser>> GetAllProjectMembersExceptPMAsync(int projectId);

    public Task<List<Project>> GetArchivedProjectsByCompany(int companyId);

    public Task<List<ITUser>> GetDevelopersOnProjectAsync(int projectId);

    public Task<ITUser> GetProjectManagerAsync(int projectId);

    public Task<List<ITUser>> GetProjectMembersByRoleAsync(int projectId, string role);

    public Task<Project> GetProjectByIdAsync(int projectId, int companyId);

    public Task<List<ITUser>> GetSubmittersOnProjectAsync(int projectId);

    public Task<List<ITUser>> GetUsersNotOnProjectAsync(int projectId, int companyId);

    public Task<List<Project>> GetUserProjectsAsync(string userId);

    public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

    public Task<int> LookupProjectPriorityId(string priorityName);

    public Task RemoveProjectManagerAsync(int projectId);

    public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);

    public Task RemoveUserFromProjectAsync(string userId, int projectId);

    public Task UpdateProjectAsync(Project project);
}