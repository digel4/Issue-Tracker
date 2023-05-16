using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services;

// Deriving or inheriting a child class from a parent class. 
public class ITRolesService : IITRolesService
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ITUser> _userManager;
    
    // Constructor
    public ITRolesService(
        ApplicationDbContext context, 
        RoleManager<IdentityRole> roleManager,
        // Remember that ITUser is an extension of identity user
        UserManager<ITUser> userManager
        )
    {
        // Dependency Injection /  service layer
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }
    public async Task<bool> IsUserInRoleAsync(ITUser user, string roleName)
    {
        bool result = await _userManager.IsInRoleAsync(user, roleName);

        return result;
    }

    
    public async Task<List<IdentityRole>> GetRolesAsync()
    {
        try
        {
            List<IdentityRole> result = new();
    
            result = await _context.Roles.ToListAsync();
    
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting roles. --->  {e.Message}");
            throw;
        }
    }
    
    public async Task<IEnumerable<string>> GetSingleUserRolesAsync(ITUser user)
    {
        IEnumerable<string> result = await _userManager.GetRolesAsync(user);
        
        return result;
    }

    public async Task<bool> AddUserToRoleAsync(ITUser user, string roleName)
    {
        // We wrap AddToRoleAsync in () because its basically order of operation. We want it complete before moving on to .Succeeded
        bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;

        return result;
    }

    public async Task<bool> RemoveUserFromSingleRoleAsync(ITUser user, string roleName)
    {
        bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;

        return result;
    }

    public async Task<bool> RemoveUserFromManyRolesAsync(ITUser user, IEnumerable<string> roles)
    {
        bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;

        return result;
    }

    public async Task<List<ITUser>> GetManyUsersInRoleAsync(string roleName, int companyId)
    {
        List<ITUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
        
        // Linked entities doesn't have to be used with the db directly. We can write link against other things as well
        List<ITUser> result = users.Where(u => u.CompanyId == companyId).ToList();
        
        return result;
        
    }

    public async Task<List<ITUser>> GetManyUsersNotInRoleAsync(string roleName, int companyId)
    {
        //.Select Id column and turn those ids into a string
        List<string> userIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id).ToList();
        
        // Remember where can return multiple entities
        List<ITUser> roleUsers = _context.Users.Where(u => !userIds.Contains(u.Id)).ToList();
        
        List<ITUser> result = roleUsers.Where(u => u.CompanyId == companyId).ToList();
        
        return result;
    }

    public async Task<string> GetRoleNameByIdAsync(string roleId)
    {
        // IdentityRole is built into dot net
        IdentityRole role = await _context.Roles.FindAsync(roleId);

        string result = await _roleManager.GetRoleNameAsync(role);

        return result;
    }
}