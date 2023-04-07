using IssueTracker.Models;
using Microsoft.CodeAnalysis;

namespace IssueTracker.Services.Interfaces;

public interface IITTicketHistoryService
{
    Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId);

    Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId);

    Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId);
}