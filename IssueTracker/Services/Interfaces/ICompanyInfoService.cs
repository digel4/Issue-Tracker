using IssueTracker.Models;

namespace IssueTracker.Services.Interfaces;

public interface ICompanyInfoService
{
    //<> after task is the return type
    public Task<Company?> GetCompanyInfoByIdAsync(int? companyId);

    public Task<List<ITUser>> GetAllMembersAsync(int companyId);
    
    Task<List<ITUser>> GetAllAdminsAsync(int companyId);
    Task<List<ITUser>> GetAllProjectManagersAsync(int companyId);
    Task<List<ITUser>> GetAllDevelopersAsync(int companyId);
    Task<List<ITUser>> GetAllSubmittersAsync(int companyId);

    public Task<List<Project>> GetAllProjectsAsync(int companyId);
    
    public Task<List<Ticket>> GetAllTicketsAsync(int companyId);
    
    Task<ITUser?> UpdateMemberProfileAsync(ITUser updatedUser);
    
    Task DeleteAvatarImageAsync(string userId);
    
    Task<bool> AddMemberAsync(ITUser itUser, int companyId);

    Task<bool> RemoveMemberAsync(string memberId);


}