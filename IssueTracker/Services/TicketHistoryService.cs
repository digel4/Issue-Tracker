using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services;

public class ITTicketHistoryService : ITicketHistoryService
{
    #region Properties
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ITUser> _userManager;

    #endregion
    
    #region Constructor
    // Constructor
    public ITTicketHistoryService(
        ApplicationDbContext context,
        UserManager<ITUser> userManager
        )
    {
        // Dependency Injection /  service layer
        _context = context;
        _userManager = userManager;
    }
    #endregion

    // #region Add History (1)
    // public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
    // {
    //     if (oldTicket == null && newTicket != null)
    //     {
    //         // We can access and change the properties of a newly instantiated ticket history
    //         TicketHistory history = new()
    //         {
    //             TicketId = newTicket.Id,
    //             Property = "",
    //             OldValue = "",
    //             NewValue = "",
    //             Created = DateTimeOffset.Now,
    //             UserId = userId,
    //             Description = "New Ticket Craeted"
    //         };
    //
    //         try
    //         {
    //             await _context.TicketHistories.AddAsync(history);
    //             await _context.SaveChangesAsync();
    //         }
    //         catch (Exception e)
    //         {
    //             Console.WriteLine($"****ERROR**** - Error adding new ticket history --->  {e.Message}");
    //             throw;
    //         }
    //     }
    //     else
    //     {
    //         // Check Ticket Title
    //         if (oldTicket.Title != newTicket.Title)
    //         {
    //             // We can access and change the properties of a newly instantiated ticket history
    //             TicketHistory history = new()
    //             {
    //                 TicketId = newTicket.Id,
    //                 Property = "Title",
    //                 OldValue = oldTicket.Title,
    //                 NewValue = newTicket.Title,
    //                 Created = DateTimeOffset.Now,
    //                 UserId = userId,
    //                 Description = $"New Ticket Ttile: {newTicket.Title}"
    //             };
    //             await _context.TicketHistories.AddAsync(history);
    //         }
    //         // Check Ticket Description
    //         if (oldTicket.Description != newTicket.Description)
    //         {
    //             // We can access and change the properties of a newly instantiated ticket history
    //             TicketHistory history = new()
    //             {
    //                 TicketId = newTicket.Id,
    //                 Property = "Description",
    //                 OldValue = oldTicket.Description,
    //                 NewValue = newTicket.Description,
    //                 Created = DateTimeOffset.Now,
    //                 UserId = userId,
    //                 Description = $"New Ticket Description: {newTicket.Description}"
    //             };
    //             await _context.TicketHistories.AddAsync(history);
    //         }
    //         // Check Ticket Priority
    //         if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
    //         {
    //             // We can access and change the properties of a newly instantiated ticket history
    //             TicketHistory history = new()
    //             {
    //                 TicketId = newTicket.Id,
    //                 Property = "Priority",
    //                 OldValue = oldTicket.TicketPriority.Name,
    //                 NewValue = newTicket.TicketPriority.Name,
    //                 Created = DateTimeOffset.Now,
    //                 UserId = userId,
    //                 Description = $"New Ticket Priority: {newTicket.TicketPriority.Name}"
    //             };
    //             await _context.TicketHistories.AddAsync(history);
    //         }
    //         // Check Ticket Status
    //         if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
    //         {
    //             // We can access and change the properties of a newly instantiated ticket history
    //             TicketHistory history = new()
    //             {
    //                 TicketId = newTicket.Id,
    //                 Property = "Status",
    //                 OldValue = oldTicket.TicketStatus.Name,
    //                 NewValue = newTicket.TicketStatus.Name,
    //                 Created = DateTimeOffset.Now,
    //                 UserId = userId,
    //                 Description = $"New Ticket Status: {newTicket.TicketStatus.Name}"
    //             };
    //             await _context.TicketHistories.AddAsync(history);
    //         }
    //         // Check Ticket Type
    //         if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
    //         {
    //             // We can access and change the properties of a newly instantiated ticket history
    //             TicketHistory history = new()
    //             {
    //                 TicketId = newTicket.Id,
    //                 Property = "TicketTypeId",
    //                 OldValue = oldTicket.TicketType.Name,
    //                 NewValue = newTicket.TicketType.Name,
    //                 Created = DateTimeOffset.Now,
    //                 UserId = userId,
    //                 Description = $"New Ticket Type: {newTicket.TicketType.Name}"
    //             };
    //             await _context.TicketHistories.AddAsync(history);
    //         }
    //         // Check Ticket Developer
    //         if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
    //         {
    //             // We can access and change the properties of a newly instantiated ticket history
    //             TicketHistory history = new()
    //             {
    //                 TicketId = newTicket.Id,
    //                 Property = "Developer",
    //                 OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
    //                 NewValue = newTicket.DeveloperUser?.FullName,
    //                 Created = DateTimeOffset.Now,
    //                 UserId = userId,
    //                 Description = $"New Ticket Developer: {newTicket.DeveloperUser.FullName}"
    //             };
    //             await _context.TicketHistories.AddAsync(history);
    //         }
    //
    //         try
    //         {
    //             await _context.SaveChangesAsync();
    //         }
    //         catch (Exception e)
    //         {
    //             Console.WriteLine($"****ERROR**** - Error adding ticket history --->  {e.Message}");
    //             throw;
    //         }
    //     }
    // }
    // #endregion
    //
    // #region Add History (2)
    // public async Task AddHistoryAsync(int ticketId, string model, string userId)
    // {
    //     try
    //     {
    //         // model is passed in as a string because we want to run some string methods on it. We also want to set it as the property key on the history object as a string.
    //         
    //         Ticket ticket = await _context.Tickets.FindAsync(ticketId);
    //         // Remove "Ticket" from the string
    //         string description = model.ToLower().Replace("ticket", "");
    //         description = $"New {description} added to ticket: {ticket.Title}";
    //
    //         TicketHistory history = new()
    //         {
    //             TicketId = ticket.Id,
    //             Property = model,
    //             OldValue = "",
    //             NewValue = "",
    //             Created = DateTimeOffset.Now,
    //             UserId = userId,
    //             Description = description
    //         };
    //
    //         await _context.TicketHistories.AddAsync(history);
    //         await _context.SaveChangesAsync();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine($"****ERROR**** - Error adding ticket history --->  {e.Message}");
    //         throw;
    //     }
    // }
    // #endregion
    //
    // #region Get Project Tickets Histories
    // public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
    // {
    //     try
    //     {
    //         Project project= await _context.Projects
    //             .Where(p => p.CompanyId == companyId)
    //             .Include(p => p.Tickets)
    //                 .ThenInclude(t => t.History)
    //                 .ThenInclude(h => h.User)
    //             .FirstOrDefaultAsync(p => p.Id == projectId);
    //
    //         List<TicketHistory> ticketHistory = project.Tickets.SelectMany(t => t.History).ToList();
    //
    //         return ticketHistory;
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine($"****ERROR**** - Error getting project ticket histories --->  {e.Message}");
    //         throw;
    //     }
    // }
    // #endregion
    //
    // #region Get Company Tickets Histories
    // public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
    // {
    //     try
    //     {
    //         List<Project> projects = (await _context.Companies
    //             .Include(c => c.Projects)
    //                 .ThenInclude(p => p.Tickets)
    //                 .ThenInclude(t => t.History)
    //                 .ThenInclude(h => h.User)
    //             .FirstOrDefaultAsync(c => c.Id == companyId))
    //             .Projects.ToList();
    //         
    //         List<Ticket> tickets = projects.SelectMany(p => p.Tickets).ToList();
    //
    //         List<TicketHistory> ticketHistories = tickets.SelectMany(t => t.History).ToList();
    //
    //         return ticketHistories;
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine($"****ERROR**** - Error getting company ticket histories --->  {e.Message}");
    //         throw;
    //     }
    // }
    // #endregion


    
    
    public async Task AddArchiveChangeEventAsync(Ticket ticket, string userMakingChangeId)
        {
            ITUser userMakingChange = await _userManager.FindByIdAsync(userMakingChangeId);

            string archivedOrUnarchived = ticket.Archived ? "archived" : "unarchived";

            TicketHistory archiveEvent = new TicketHistory()
            {
                TicketId = ticket.Id,
                UserId = userMakingChangeId,
                Created = DateTime.Now,
                Description = $"{userMakingChange.FullName} {archivedOrUnarchived} the ticket.",
            };

            await _context.TicketHistories.AddAsync(archiveEvent);
            await _context.SaveChangesAsync();
        }

        public async Task AddAttachmentEventAsync(Ticket ticket, TicketAttachment attachment)
        {
            ITUser attachingUser = await _userManager.FindByIdAsync(attachment.UserId);

            TicketHistory attachmentEvent = new TicketHistory()
            {
                TicketId = ticket.Id,
                UserId = attachment.UserId,
                Created = attachment.Created,
                Description = $"{attachingUser.FullName} added attachment {attachment.FileName}",
            };

            await _context.TicketHistories.AddAsync(attachmentEvent);
            await _context.SaveChangesAsync();
        }

        public async Task AddDeveloperAssignmentEventAsync(Ticket ticket, string userMakingChangeId)
        {
            ITUser userMakingChange = await _userManager.FindByIdAsync(userMakingChangeId);

            TicketHistory developerAssignmentEvent = new TicketHistory()
            {
                TicketId = ticket.Id,
                UserId = userMakingChangeId,
                Created = DateTime.Now,
                Description = $"{userMakingChange.FullName} changed the ticket assignment to {ticket.DeveloperUser!.FullName}",
            };

            await _context.TicketHistories.AddAsync(developerAssignmentEvent);
            await _context.SaveChangesAsync();
        }

        public async Task AddTicketHistoryItemAsync(TicketHistory history)
        {
            await _context.TicketHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task AddTicketCreatedEventAsync(int ticketId)
        {
            Ticket? ticket = await _context.Tickets
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.DeveloperUser)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return;

            ITUser creator = await _userManager.FindByIdAsync(ticket.OwnerUserId);

            TicketHistory createdEvent = new TicketHistory()
            {
                TicketId = ticket.Id,
                UserId = ticket.OwnerUserId,
                Created = ticket.Created,
                Description = $"{creator.FullName} created the ticket</br></br>" +
                $"Title: {ticket.Title}</br>" +
                $"Priority: {ticket.TicketPriority!.Name}</br>" +
                $"Type: {ticket.TicketType!.Name}</br>" +
                $"Assigned To: {(ticket.DeveloperUser != null ? ticket.DeveloperUser.FullName : "Unassigned")}</br></br>" +
                $"Description: {ticket.Description}"
            };
            Console.WriteLine(createdEvent);
            await _context.TicketHistories.AddAsync(createdEvent);
            await _context.SaveChangesAsync();
        }
    }
