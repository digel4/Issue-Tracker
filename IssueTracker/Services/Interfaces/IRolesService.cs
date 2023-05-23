using IssueTracker.Models;

using Microsoft.AspNetCore.Identity;


namespace IssueTracker.Services.Interfaces;

public interface IRolesService
{
    public Task<bool> IsUserInRoleAsync(ITUser user, string roleName);


    public Task<List<IdentityRole>> GetRolesAsync();

    public Task<IEnumerable<string>> GetSingleUserRolesAsync(ITUser user);
    
    public Task<bool> AddUserToRoleAsync(ITUser user, string roleName);
    
    public Task<bool> RemoveUserFromSingleRoleAsync(ITUser user, string roleName);
    
    public Task<bool> RemoveUserFromManyRolesAsync(ITUser user, IEnumerable<string> roles);
    
    public Task<List<ITUser>> GetManyUsersInRoleAsync(string roleName, int companyId);
    
    public Task<List<ITUser>> GetManyUsersNotInRoleAsync(string roleName, int companyId);
    
    public Task<string> GetRoleNameByIdAsync(string roleId);

}