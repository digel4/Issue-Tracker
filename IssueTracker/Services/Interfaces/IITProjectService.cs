using IssueTracker.Models;

namespace IssueTracker.Services.Interfaces;

public interface IITProjectService
{
    #region Add New Project
    public Task AddNewProjectAsync(Project project);
    #endregion

    #region  Add Project Manager
    public Task<bool> AddProjectManagerAsync(string userId, int projectId);
    #endregion

    #region Add User To Project 
    public Task<bool> AddUserToProjectAsync(string userId, int projectId);
    #endregion
    
    #region Archive Project
    public Task ArchiveProjectAsync(Project project);
    #endregion
    
    #region Get All Projects By Company
    public Task<List<Project>> GetAllProjectsByCompany(int companyId);
    #endregion
    
    #region Get All Projects By Priority
    public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName);
    #endregion
    
    #region Get All Project Members Except PM 
    public Task<List<ITUser>> GetAllProjectMembersExceptPMAsync(int projectId);
    #endregion
    
    #region Get Archived Projects By Company
    public Task<List<Project>> GetArchivedProjectsByCompany(int companyId);
    #endregion
        
    #region Get Unassigned Projects 
    public Task<List<Project>> GetUnassignedProjectsAsync(int companyId);
    #endregion
    
    #region Get Developers On Project 
    public Task<List<ITUser>> GetDevelopersOnProjectAsync(int projectId);
    #endregion
    
    #region Get Project Manager 
    public Task<ITUser> GetProjectManagerAsync(int projectId);
    #endregion
    
    #region Get Project Members By Role 
    public Task<List<ITUser>> GetProjectMembersByRoleAsync(int projectId, string role);
    #endregion
    
    #region Get Project By Id
    public Task<Project> GetProjectByIdAsync(int projectId, int companyId);
    #endregion
    
    #region Get Submitters On Project
    public Task<List<ITUser>> GetSubmittersOnProjectAsync(int projectId);
    #endregion
    
    #region Get Users Not On Project 
    public Task<List<ITUser>> GetUsersNotOnProjectAsync(int projectId, int companyId);
    #endregion
    
    #region Get User Projects
    public Task<List<Project>> GetUserProjectsAsync(string userId);
    #endregion

    public Task<List<Project>> GetUserArchivedProjectsAsync(string userId);
    
    #region is Assigned Project Manager
    public Task<bool> isAssignedProjectManagerAsync(string userId, int projectId); 
    #endregion
    
    #region Is User On Project
    public Task<bool> IsUserOnProjectAsync(string userId, int projectId);
    #endregion
    
    #region Look up Project Priority Id
    public Task<int> LookupProjectPriorityId(string priorityName);
    #endregion
    
    #region Remove Project Manager 
    public Task RemoveProjectManagerAsync(int projectId);
    #endregion
    
    #region Remove Users From Project By Role
    public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);
    #endregion
    
    #region Remove User From Project
    public Task RemoveUserFromProjectAsync(string userId, int projectId);
        #endregion

    public Task<bool> DeleteProjectAsync(int companyId, int projectId);

    Task<bool> RemoveMemberFromAllProjectsAsync(int companyId, string employeeId);
        
    #region Restore Project
    public Task RestoreProjectAsync(Project project);
        #endregion
        
    #region Update Project
    public Task UpdateProjectAsync(Project project);
    #endregion
}