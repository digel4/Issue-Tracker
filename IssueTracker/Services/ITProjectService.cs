using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services;

public class ITProjectService : IITProjectService
{
    
    private readonly ApplicationDbContext _context;
    private readonly IITRolesService  _rolesService;
    // Constructor
    public ITProjectService(
        ApplicationDbContext context,
        IITRolesService rolesService
    )
    {
        // Dependency Injection /  service layer
        _context = context;
        _rolesService = rolesService;
    }
    
    
    // CRUD - Create
    public async Task AddNewProjectAsync(Project project)
    {
        _context.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
    {
        ITUser currentPM = await GetProjectManagerAsync(projectId);

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
    
    // CRUD - Archive (Delete)
    public async Task ArchiveProjectAsync(Project project)
    {
        project.Archived = true;
        _context.Update(project);
        await _context.SaveChangesAsync();
    }

    // CRUD - Read 
    public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
    {
        // This is the same as writing List<Project> result = new List<Project>();
        List<Project> projects = new();
        // We need to make sure we get all the other tables associated with projects like project priority. We do this with .Include. This is called eager loading information. We do not get the virtual props by default only the local ones.
        projects = await _context.Projects
            .Where(p => p.CompanyId == companyId && p.Archived == false)
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

    // CRUD - Read 
    public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
    {
        // This is the same as writing List<Project> result = new List<Project>();
        List<Project> projects = new();

        projects = await GetAllProjectsByCompany(companyId);

        int priorityId = await LookupProjectPriorityId(priorityName);

        return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();
    }

    public async Task<List<ITUser>> GetAllProjectMembersExceptPMAsync(int projectId)
    {
        List<ITUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
        List<ITUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
        List<ITUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

        List<ITUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

        return teamMembers;
    }

    public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
    {
        // This is the same as writing List<Project> result = new List<Project>();
        List<Project> projects = new();

        projects = await GetAllProjectsByCompany(companyId);

        return projects.Where(p => p.Archived == true).ToList();
    }

    public async Task<List<ITUser>> GetDevelopersOnProjectAsync(int projectId)
    {
        throw new NotImplementedException();
    }

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

    // CRUD - Read 
    public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
    {
        Project project = await _context.Projects
            .Include(p => p.Tickets)
            .Include(p => p.Members)
            .Include(p => p.ProjectPriority)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
        
        return project;
    }

    public async Task<List<ITUser>> GetSubmittersOnProjectAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ITUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
    {
        List<ITUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

        return users.Where(u => u.CompanyId == companyId).ToList();
    }

    public async Task<List<Project>> GetUserProjectsAsync(string userId)
    {
        try
        {
            List<Project> userProjects = (await _context.Users
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
                .Projects.ToList();

            return userProjects;

        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error Getting user projects list. --->  {e.Message}");
            throw;
        }
    }

    public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
    {
        Project project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        bool result = false;

        if (project != null)
        {
            result = project.Members.Any(m => m.Id == userId);
        }

        return result;
    }

    public async Task<int> LookupProjectPriorityId(string priorityName)
    {
        // parenthesis means the first await is done and returns a project. Then simply use dot notation to access the id property on the returned project.
        int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

        return priorityId;
    }

    // CRUD - Delete 
    public async Task RemoveProjectManagerAsync(int projectId)
    {
        Project project = await _context.Projects
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

    // CRUD - Delete 
    public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
    {

        try
        {
            List<ITUser> members = await GetProjectMembersByRoleAsync(projectId,role);

            Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

            foreach (ITUser user in members)
            {
                try
                {
                    project.Members.Remove(user);
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

    // CRUD - Delete 
    public async Task RemoveUserFromProjectAsync(string userId, int projectId)
    {
        try
        {
            ITUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                if (await IsUserOnProjectAsync(userId, projectId))
                {
                    project.Members.Remove(user);
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

    // CRUD - Update
    public async Task UpdateProjectAsync(Project project)
    {
        _context.Update(project);
        await _context.SaveChangesAsync();
    }
}