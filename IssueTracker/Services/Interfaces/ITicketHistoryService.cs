using IssueTracker.Models;
using Microsoft.CodeAnalysis;

namespace IssueTracker.Services.Interfaces;

public interface ITicketHistoryService
{
//     Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId);
//     
//     // Overloaded method
//     Task AddHistoryAsync(int ticketId, string model, string userId);
//
//     Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId);
//
//     Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId);

    Task AddArchiveChangeEventAsync(Ticket ticket, string userMakingChangeId);
    Task AddAttachmentEventAsync(Ticket ticket, TicketAttachment attachment);
    Task AddDeveloperAssignmentEventAsync(Ticket ticket, string userMakingChangeId);
    Task AddTicketHistoryItemAsync(TicketHistory history);
    Task AddTicketCreatedEventAsync(int ticketId);

}

