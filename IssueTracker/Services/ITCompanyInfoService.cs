using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services;

// Deriving or inheriting a child class from a parent class. 
public class ITCompanyInfoService : IITCompanyInfoService
{
    #region Properties
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ITUser> _userManager;
    #endregion

    #region Constructor
    public ITCompanyInfoService(
        ApplicationDbContext context,
        UserManager<ITUser> userManager
    )
    {
        // Dependency Injection /  service layer
        _context = context;
        _userManager = userManager;
    }
    #endregion

    #region Get Company Info By Id
    public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
    {
        Company result = new();

        if (companyId != null)
        {
            result = await _context.Companies
                .Include(c => c.Members)
                .Include(c => c.Projects)
                .Include(c => c.Invites)
                .FirstOrDefaultAsync(c => c.Id == companyId);
        }
        
        return result;
    }
    #endregion
    
    #region Update Member Profile 
    public async Task<ITUser?> UpdateMemberProfileAsync(ITUser updatedUser)
    {
        ITUser? user = await _context.Users.FindAsync(updatedUser.Id);

        if (user == null) return user;

        user.FirstName = updatedUser.FirstName;
        user.AvatarFileData = updatedUser.AvatarFileData;
        user.AvatarContentType = updatedUser.AvatarContentType;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }
    #endregion
    
    #region Delete Avatar Image
    public async Task DeleteAvatarImageAsync(string userId)
    {
        ITUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is not null)
        {
            user.AvatarFileData = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
    #endregion
    
    #region Add Member 
    public async Task<bool> AddMemberAsync(ITUser itUser, int companyId)
    {
        Company? company = await GetCompanyInfoByIdAsync(companyId);

        if (company is null)
            return false;

        itUser.CompanyId = companyId;

        _context.Users.Update(itUser);
        await _context.SaveChangesAsync();

        return true;
    }
    #endregion
    
    #region Get All Members 
    public async Task<List<ITUser>> GetAllMembersAsync(int companyId)
    {
        // This is the same as writing List<ITUser> result = new List<ITUser>();
        List<ITUser> result = new();

        result = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

        return result;
    }
    #endregion
    
    #region Get All Projects 
    public async Task<List<Project>> GetAllProjectsAsync(int companyId)
    {
        // This is the same as writing List<Project> result = new List<Project>();
        List<Project> result = new();
        // We need to make sure we get all the other tables associated with projects like project priority. We do this with .Include. This is called eager loading information. We do not get the virtual props by default only the local ones.
        result = await _context.Projects
            .Where(p => p.CompanyId == companyId)
            //.Include(p => p.Company)
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

        return result;
    }
    #endregion
    
    #region Get All Tickets 
    public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
    {
        // This is the same as writing List<Ticket> result = new List<Ticket>();
        List<Ticket> result = new();
        List<Project> projects = new();

        projects = await GetAllProjectsAsync(companyId);

        result = projects.SelectMany(p => p.Tickets).ToList();

        return result;
    }
    #endregion
    
    #region Get All Admins
    public async Task<List<ITUser>> GetAllAdminsAsync(int companyId)
    {
        List<ITUser> members = await GetAllMembersAsync(companyId);

        List<ITUser> admins = new List<ITUser>();

        foreach (ITUser user in members)
        {
            if (await _userManager.IsInRoleAsync(user, nameof(Roles.Admin)))
                admins.Add(user);
        }

        return admins;
    }
    #endregion
    
    #region Get All Submitters
    public async Task<List<ITUser>> GetAllSubmittersAsync(int companyId)
    {
        List<ITUser> members = await GetAllMembersAsync(companyId);

        List<ITUser> submitters = new List<ITUser>();

        foreach (ITUser user in members)
        {
            if (await _userManager.IsInRoleAsync(user, nameof(Roles.Submitter)))
                submitters.Add(user);
        }

        return submitters;
    }
    #endregion
    
    #region Get All Project Managers
    public async Task<List<ITUser>> GetAllProjectManagersAsync(int companyId)
    {
        List<ITUser> members = await GetAllMembersAsync(companyId);

        List<ITUser> projectManagers = new List<ITUser>();

        foreach (ITUser user in members)
        {
            if (await _userManager.IsInRoleAsync(user, nameof(Roles.ProjectManager)))
                projectManagers.Add(user);
        }

        return projectManagers;
    }
    #endregion
    
    #region Get All Developers
    public async Task<List<ITUser>> GetAllDevelopersAsync(int companyId)
    {
        List<ITUser> members = await GetAllMembersAsync(companyId);

        List<ITUser> developers = new List<ITUser>();

        foreach (ITUser user in members)
        {
            if (await _userManager.IsInRoleAsync(user, nameof(Roles.Developer)))
                developers.Add(user);
        }

        return developers;
    }
    #endregion
    
    public async Task<bool> RemoveMemberAsync(string memberId)
    {
        ITUser? employeeToRemove = await _context.Users.FirstOrDefaultAsync(u => u.Id == memberId);

        if (employeeToRemove is null)
            return false;

        employeeToRemove.CompanyId = null;

        await _context.SaveChangesAsync();
        return true;
    }
}