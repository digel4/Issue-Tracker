using IssueTracker.Models;

namespace IssueTracker.Services.Interfaces;

public interface IITTicketService
{
    // CRUD Methods
    public Task<int> AddNewTicketAsync(Ticket ticket);
    public Task<bool>  AddNewTicketCommentAsync(int ticketId, TicketComment ticketComment);
    public Task<bool> AddTicketAttachmentAsync(int ticketId, TicketAttachment ticketAttachment);

    public Task<List<Ticket>> GetAllActionRequiredTicketsAsync(int companyId);
    public Task<List<Ticket>> GetAllOpenTicketsAsync(int companyId);
    public Task<List<Ticket>> GetAllCompletedTicketsAsync(int companyId);
    public Task<List<Ticket>> GetUserActionRequiredTicketsAsync(string userId, int companyId);
    public Task<List<Ticket>> GetUserCompletedTicketsAsync(string userId, int companyId);
    public Task<List<Ticket>> GetUserOpenTicketsAsync(string userId, int companyId);
    
    public Task UpdateTicketAsync(Ticket ticket);
    public Task<Ticket> GetTicketByIdAsync(int ticketId);
    public Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId);
    public Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId); 
    public Task ArchiveTicketAsync(Ticket ticket);
    public Task RestoreTicketAsync(Ticket ticket);
    public Task AssignTicketAsync(int ticketId, string userId);
    public Task<List<Ticket>> GetArchivedTicketsAsync(int companyId);
    public Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId);
    public Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName);
    public Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName);
    public Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName);
    public Task<ITUser> GetTicketDeveloperAsync(int ticketId, int companyId);
    public Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId);
    public Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId);
    public Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId);
    public Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId);
    public Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId);
    public Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId);
    public Task<List<Ticket>> GetUnassignedTicketsAsync(int companyId);
    public Task<int?> LookupTicketPriorityIdAsync(string priorityName);
    public Task<int?> LookupTicketStatusIdAsync(string statusName);
    public Task<int?> LookupTicketTypeIdAsync(string typeName);
    public Task RemoveMemberFromAllTicketsAsync(int companyId, string memberId);

    public Task DeleteTicketAsync(Ticket ticket);
}