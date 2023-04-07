using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services;

public class ITTicketHistoryService : IITTicketHistoryService
{
    private readonly ApplicationDbContext _context;
    // Constructor
    public ITTicketHistoryService(
        ApplicationDbContext context 
    )
    {
        // Dependency Injection /  service layer
        _context = context;
    }


    public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
    {
        if (oldTicket == null && newTicket != null)
        {
            // We can access and change the properties of a newly instantiated ticket history
            TicketHistory history = new()
            {
                TicketId = newTicket.Id,
                Property = "",
                OldValue = "",
                NewValue = "",
                Created = DateTimeOffset.Now,
                UserId = userId,
                Description = "New Ticket Craeted"
            };

            try
            {
                await _context.TicketHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"****ERROR**** - Error adding new ticket history --->  {e.Message}");
                throw;
            }
        }
        else
        {
            // Check Ticket Title
            if (oldTicket.Title != newTicket.Title)
            {
                // We can access and change the properties of a newly instantiated ticket history
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "Title",
                    OldValue = oldTicket.Title,
                    NewValue = newTicket.Title,
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = $"New Ticket Ttile: {newTicket.Title}"
                };
                await _context.TicketHistories.AddAsync(history);
            }
            // Check Ticket Description
            if (oldTicket.Description != newTicket.Description)
            {
                // We can access and change the properties of a newly instantiated ticket history
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "Description",
                    OldValue = oldTicket.Description,
                    NewValue = newTicket.Description,
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = $"New Ticket Description: {newTicket.Description}"
                };
                await _context.TicketHistories.AddAsync(history);
            }
            // Check Ticket Priority
            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                // We can access and change the properties of a newly instantiated ticket history
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "Priority",
                    OldValue = oldTicket.TicketPriority.Name,
                    NewValue = newTicket.TicketPriority.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = $"New Ticket Priority: {newTicket.TicketPriority.Name}"
                };
                await _context.TicketHistories.AddAsync(history);
            }
            // Check Ticket Status
            if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                // We can access and change the properties of a newly instantiated ticket history
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "Status",
                    OldValue = oldTicket.TicketStatus.Name,
                    NewValue = newTicket.TicketStatus.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = $"New Ticket Status: {newTicket.TicketStatus.Name}"
                };
                await _context.TicketHistories.AddAsync(history);
            }
            // Check Ticket Type
            if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
            {
                // We can access and change the properties of a newly instantiated ticket history
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "TicketTypeId",
                    OldValue = oldTicket.TicketType.Name,
                    NewValue = newTicket.TicketType.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = $"New Ticket Type: {newTicket.TicketType.Name}"
                };
                await _context.TicketHistories.AddAsync(history);
            }
            // Check Ticket Developer
            if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
            {
                // We can access and change the properties of a newly instantiated ticket history
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "Developer",
                    OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                    NewValue = newTicket.DeveloperUser?.FullName,
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = $"New Ticket Developer: {newTicket.DeveloperUser.FullName}"
                };
                await _context.TicketHistories.AddAsync(history);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"****ERROR**** - Error adding editing ticket history --->  {e.Message}");
                throw;
            }
        }
    }

    public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
    {
        try
        {
            Project project= await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.History)
                    .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            List<TicketHistory> ticketHistory = project.Tickets.SelectMany(t => t.History).ToList();

            return ticketHistory;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting project ticket histories --->  {e.Message}");
            throw;
        }
    }

    public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
    {
        try
        {
            List<Project> projects = (await _context.Companies
                .Include(c => c.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.History)
                    .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(c => c.Id == companyId))
                .Projects.ToList();
            
            List<Ticket> tickets = projects.SelectMany(p => p.Tickets).ToList();

            List<TicketHistory> ticketHistories = tickets.SelectMany(t => t.History).ToList();

            return ticketHistories;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting company ticket histories --->  {e.Message}");
            throw;
        }
    }
}