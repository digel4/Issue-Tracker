using IssueTracker.Models;

namespace IssueTracker.Services.Interfaces;

public interface IITCompanyInfoService
{
    //<> after task is the return type
    public Task<Company> GetCompanyInfoByIdAsync(int? companyId);

    public Task<List<ITUser>> GetAllMembersAsync(int companyId);

    public Task<List<Project>> GetAllProjectsAsync(int companyId);
    
    public Task<List<Ticket>> GetAllTicketsAsync(int companyId);
}