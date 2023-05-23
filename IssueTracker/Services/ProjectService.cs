using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services;

public class ProjectService : IProjectService
{
    #region Properties
    private readonly ApplicationDbContext _context;
    private readonly IRolesService  _rolesService;
    private readonly UserManager<ITUser> _userManager;

    #endregion
    
    #region Contructor
    public ProjectService(
        ApplicationDbContext context,
        IRolesService rolesService,
        UserManager<ITUser> userManager
        )
    {
        // Dependency Injection /  service layer
        _context = context;
        _rolesService = rolesService;
        _userManager = userManager;
    }
    #endregion
    
    #region Add New Project 
    // CRUD - Create
    public async Task<int> AddNewProjectAsync(Project project)
    {
        _context.Add(project);
        await _context.SaveChangesAsync();
        return project.Id;
    }
    #endregion

    #region Add Project Manager 
    // I don't think this is working. May start a recursive loop
    public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
    {
        ITUser? currentPM = await GetProjectManagerAsync(projectId);

        // Remove the current PM if necessary 
        if (currentPM != null)
        {
            try
            {
                await RemoveProjectManagerAsync(projectId);
            }
            catch (Exception e)
            {
                Console.WriteLine($"****ERROR**** - Error Removing project manager from project. --->  {e.Message}");
                return false;
            }
        }
        
        // Add the new PM
        try
        {
            //AddProjectManagerAsync(userId, projectId);
            await AddUserToProjectAsync(userId, projectId);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error adding project manager to project. --->  {e.Message}");
            return false;
        }
        
        
    }
    #endregion
    
    #region Add User To Project 
    public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
    {
        ITUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if ( !await IsUserOnProjectAsync(userId, projectId) )
            {
                try
                {
                    project.Members.Add(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        return false;  
    }
    #endregion
    
    #region Archive Project 
    // CRUD - Archive (Delete)
    public async Task ArchiveProjectAsync(Project project)
    {
        try
        {
            project.Archived = true;
            await UpdateProjectAsync(project);
        
            // Archive the Tickets for the Project
            foreach (Ticket ticket in project.Tickets)
            {
                ticket.ArchivedByProject = true;
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error archiving project. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get All Projects By Company
    // CRUD - Read 
    public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
    {
        // This is the same as writing List<Project> result = new List<Project>();
        List<Project> projects = new();
        // We need to make sure we get all the other tables associated with projects like project priority. We do this with .Include. This is called eager loading information. We do not get the virtual props by default only the local ones.
        projects = await _context.Projects
            .Where(p => p.CompanyId == companyId && p.Archived == false)
            .Include(p => p.Members)
            .Include(p => p.Company)
            .Include(p => p.Tickets)
                //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.Comments)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.Attachments)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.History)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.Notifications)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.DeveloperUser)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.OwnerUser)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.TicketStatus)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.TicketPriority)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.TicketType)
            .Include(p => p.ProjectPriority)
            .ToListAsync();
        
        return projects;
    }
    #endregion
    
    #region Get All Projects By Priority
    // CRUD - Read 
    public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
    {
        // This is the same as writing List<Project> result = new List<Project>();
        List<Project> projects = new();

        projects = await GetAllProjectsByCompany(companyId);

        int? priorityId = await LookupProjectPriorityId(priorityName);

        return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();
    }
    #endregion
    
    #region Get All Project Members Except PM
    public async Task<List<ITUser>> GetAllProjectMembersExceptPMAsync(int projectId)
    {
        List<ITUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
        List<ITUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
        List<ITUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

        List<ITUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

        return teamMembers;
    }
    #endregion
    
    #region Get Archived Projects By Company
    public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
    {
        // This is the same as writing List<Project> result = new List<Project>();
        List<Project> projects = new();
        // We need to make sure we get all the other tables associated with projects like project priority. We do this with .Include. This is called eager loading information. We do not get the virtual props by default only the local ones.
        projects = await _context.Projects
            .Where(p => p.CompanyId == companyId && p.Archived == true)
            .Include(p => p.Members)
            .Include(p => p.Tickets)
                //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.Comments)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.Attachments)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.History)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.Notifications)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.DeveloperUser)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.OwnerUser)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.TicketStatus)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.TicketPriority)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.TicketType)
            .Include(p => p.ProjectPriority)
            .ToListAsync();
        

        return projects;
    }
    #endregion
    
    #region Get All Unassigned Projects

    public async Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
    {
        List<Project> result = new();
        List<Project> projects = new();

        try
        {
            projects = await _context.Projects
                .Include(p => p.ProjectPriority)
                .Where(p => p.CompanyId == companyId)
                .ToListAsync();

            foreach (Project project in projects)
            {
                if ( (await GetProjectMembersByRoleAsync(project.Id, nameof(Roles.ProjectManager))).Count == 0)
                {
                    result.Add(project);
                }
            }

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting unassigned projects --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Developers On Project 
    public async Task<List<ITUser>> GetDevelopersOnProjectAsync(int projectId)
    {
        Project? project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        List<ITUser> developers = new List<ITUser>();

        if (project == null)
            return developers;

        foreach (ITUser projectMember in project.Members)
        {
            ITUser member = await _context.Users.FirstAsync(u => u.Id == projectMember.Id);

            if (await _userManager.IsInRoleAsync(member, nameof(Roles.Developer)))
                developers.Add(member);
        }

        return developers;
    }
    #endregion
    
    #region Get Project Manager
    public async Task<ITUser> GetProjectManagerAsync(int projectId)
    {
        Project project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        // The ? means that if project is null then it won't continue.
        foreach (ITUser member in project?.Members)
        {
            if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
            {
                return member;
            }
        }
        return null;
    }
    #endregion
    
    #region Get Project Members By Role
    public async Task<List<ITUser>> GetProjectMembersByRoleAsync(int projectId, string role)
    {
        Project project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        List<ITUser> members = new();

        foreach (var user in project.Members)
        {
            if (await _rolesService.IsUserInRoleAsync(user, role))
            {
                members.Add(user);
            }
        }

        return members;
    }
    #endregion
    
    #region Get Project By Id
    // CRUD - Read 
    public async Task<Project?> GetProjectByIdAsync(int projectId, int companyId)
    {
        Project? project = await _context.Projects
            .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketPriority)
            .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketType)
            .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketStatus)
            .Include(p => p.Tickets)
                .ThenInclude(t => t.DeveloperUser)
            .Include(p => p.Tickets)
                .ThenInclude(t => t.OwnerUser)
            .Include(p => p.Members)
            .Include(p => p.ProjectPriority)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
        
        return project;
    }
    #endregion
    
    #region Get Submitters On Project
    public async Task<List<ITUser>> GetSubmittersOnProjectAsync(int projectId)
    {
        Project? project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        List<ITUser> members = new List<ITUser>();

        if (project is null)
            return members;

        foreach (ITUser projectMember in project.Members)
        {
            ITUser member = await _context.Users.FirstAsync(u => u.Id == projectMember.Id);

            if (await _userManager.IsInRoleAsync(member, nameof(Roles.Submitter)))
                members.Add(member);
        }

        return members;
    }
    #endregion
    
    #region Get Users Not On Project 
    public async Task<List<ITUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
    {
        List<ITUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

        return users.Where(u => u.CompanyId == companyId).ToList();
    }
    #endregion
    
    #region Get User Projects
    public async Task<List<Project>?> GetUserProjectsAsync(string userId)
    {
        try
        {
            List<Project>? userProjects = (await _context.Users
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.ProjectPriority)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketPriority)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketStatus)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketType)
                    .FirstOrDefaultAsync(u => u.Id == userId))
// This returns a user and then we access the projects property of that user through dot notation
                ?.Projects.ToList();
            
            return userProjects?.Where(u => u.Archived == false).ToList();

        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error Getting user projects list. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get User Archived Projects
    public async Task<List<Project>?> GetUserArchivedProjectsAsync(string userId)
    {
        try
        {
            List<Project>? userProjects = (await _context.Users
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.ProjectPriority)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketPriority)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketStatus)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketType)
                    .FirstOrDefaultAsync(u => u.Id == userId))
                // This returns a user and then we access the projects property of that user through dot notation
                ?.Projects.ToList();

            return userProjects?.Where(u => u.Archived).ToList();

        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error Getting user projects list. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Is Assigned Project Manager
    public async Task<bool> isAssignedProjectManagerAsync(string userId, int projectId)
    {
        try
        {
            string projectManagerId = (await GetProjectManagerAsync(projectId)).Id;

            if (projectManagerId == userId)
            {
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error running isAssignedProjectManagerAsync --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Is User On Project
    public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
    {
        Project? project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
            return false;

        return project.Members.Any(m => m.Id == userId);
    }
    #endregion
    
    #region Lookup Project Priority Id
    public async Task<int?> LookupProjectPriorityId(string priorityName)
    {
        int? priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName))?.Id;

        return priorityId;
    }
    #endregion
    
    #region Remove Project Manager
    // CRUD - Delete 
    public async Task RemoveProjectManagerAsync(int projectId)
    {
        Project? project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);
        try
        {
            // The ? means that if project is null then it won't continue.
            foreach (ITUser member in project?.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    await RemoveUserFromProjectAsync(member.Id, projectId);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error Removing project manager from project. --->  {e.Message}");
            throw;
        }

    }
    #endregion
    
    #region Remove Users From Project By Role
    // CRUD - Delete  
    public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
    {

        try
        {
            List<ITUser> members = await GetProjectMembersByRoleAsync(projectId,role);

            Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

            foreach (ITUser user in members)
            {
                try
                {
                    project?.Members.Remove(user);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error Removing User by role from project. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Remove User From Project
    // CRUD - Delete 
    public async Task RemoveUserFromProjectAsync(string userId, int projectId)
    {
        try
        {
            ITUser? itUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                if (await IsUserOnProjectAsync(userId, projectId))
                {
                    project?.Members.Remove(itUser);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error Removing User from project. --->  {e.Message}");
            throw;
        }
    }
    #endregion

    #region Remove Member From All Projects
    public async Task<bool> RemoveMemberFromAllProjectsAsync(int companyId, string memberId)
    {
        ITUser? employee = await _context.Users.FirstOrDefaultAsync(u => u.Id == memberId);

        if (employee == null)
            return false;

        foreach (Project project in await GetAllProjectsByCompany(companyId))
        {
            ITUser projectManager = await GetProjectManagerAsync(project.Id);
            
            if (projectManager.Id == memberId)
                await RemoveProjectManagerAsync(project.Id);

            if (project.Members.Contains(employee))
                project.Members.Remove(employee);
        }

        foreach (Project project in await GetArchivedProjectsByCompany(companyId))
        {
            ITUser projectManager = await GetProjectManagerAsync(project.Id);
            
            if (projectManager.Id == memberId)
                await RemoveProjectManagerAsync(project.Id);

            if (project.Members.Contains(employee))
                project.Members.Remove(employee);
        }

        return true;
    }
    #endregion
    
    #region Restore Project
    public async Task RestoreProjectAsync(Project project)
    {
        try
        {
            project.Archived = false;
            await UpdateProjectAsync(project);
        
            // Archive the Tickets for the Project
            foreach (Ticket ticket in project.Tickets)
            {
                ticket.ArchivedByProject = false;
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error restoring project. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Delete Project
    public async Task<bool> DeleteProjectAsync(int companyId, int projectId)
    {
        Project? project = await GetProjectByIdAsync(projectId, companyId);

        if (project == null) return false;

        foreach (ITUser projectMember in project.Members)
            project.Members.Remove(projectMember);

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion
    
    
    //TODO Do I need Update project method. If I do I need to update to use projectService
    #region Update Project
    // CRUD - Update
    public async Task UpdateProjectAsync(Project project)
    {
        _context.Update(project);
        await _context.SaveChangesAsync();
    }
    #endregion
}