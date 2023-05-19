using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IssueTracker.Services;

public class ITTicketService : IITTicketService
{
    #region Properties
    private readonly ApplicationDbContext _context;
    private readonly IITRolesService  _rolesService;
    private readonly IITProjectService  _projectService;
    #endregion


    #region Constructor
     public ITTicketService(
         ApplicationDbContext context,
         IITRolesService rolesService,
         IITProjectService projectService
     )
     {
         // Dependency Injection /  service layer
         _context = context;
         _rolesService = rolesService;
         _projectService = projectService;
     }
     #endregion
     
    #region Add New Ticket
    public async Task<int> AddNewTicketAsync(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
        return ticket.Id;
    }
    #endregion

    #region Get All Action Required Tickets
    public async Task<List<Ticket>> GetAllActionRequiredTicketsAsync(int companyId)
    {
        List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
        return companyTickets.Where(t => 
            t.TicketStatus.Name == Enum.GetName(ITTicketStatus.Unassigned) ||
            t.TicketStatus.Name == Enum.GetName(ITTicketStatus.New)
        ).ToList();
    }
    #endregion
    
    #region Get All Open Tickets
    public async Task<List<Ticket>> GetAllOpenTicketsAsync(int companyId)
    {
        List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
        return companyTickets.Where(t => t.TicketStatus.Name != Enum.GetName(ITTicketStatus.Resolved)).ToList();
    }
    #endregion
    
    #region Get All Completed Tickets
    public async Task<List<Ticket>> GetAllCompletedTicketsAsync(int companyId)
    {
        List<Ticket> companyTickets = await GetAllTicketsByCompanyAsync(companyId);
        
        return companyTickets.Where(t => t.TicketStatus.Name == Enum.GetName(ITTicketStatus.Resolved)).ToList();
    }
    #endregion
    
    #region Get User Action Required Tickets
    public async Task<List<Ticket>> GetUserActionRequiredTicketsAsync(string userId, int companyId)
    {
        List<Ticket> userTickets = await GetTicketsByUserIdAsync(userId, companyId);
        
        return userTickets.Where(t => 
            t.TicketStatus.Name == Enum.GetName(ITTicketStatus.New) &&
            t.Archived == false
        ).ToList();
    }
    #endregion

    #region Get User Open Tickets
    public async Task<List<Ticket>> GetUserOpenTicketsAsync(string userId, int companyId)
    {

        List<Ticket> userTickets = await GetTicketsByUserIdAsync(userId, companyId);

        return userTickets.Where( t =>
            t.TicketStatus.Name != Enum.GetName(ITTicketStatus.Resolved) &&
            t.Archived == false
        ).ToList();
    }
    #endregion
    
    #region Get User Completed Tickets
    public async Task<List<Ticket>> GetUserCompletedTicketsAsync(string userId, int companyId)
    {
        List<Ticket> userTickets = await GetTicketsByUserIdAsync(userId, companyId);
        
        return userTickets.Where(t => 
            t.TicketStatus.Name == Enum.GetName(ITTicketStatus.Resolved) &&
            t.Archived == false
        ).ToList();
    }
    #endregion
    
    #region Add New Ticket Comment
    public async Task<bool> AddNewTicketCommentAsync(int ticketId, TicketComment ticketComment)
    {
        Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null) return false;

        ticket.Comments.Add(ticketComment);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion
    
    #region Add Ticket Attachment 
    public async Task<bool> AddTicketAttachmentAsync(int ticketId, TicketAttachment ticketAttachment)
    {
        Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            return false;

        ticket.Attachments.Add(ticketAttachment);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion
    
    #region Update Ticket
    public async Task UpdateTicketAsync(Ticket ticket)
    {
        try
        {
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    #endregion
    
    #region Get Ticket By Id
    public async Task<Ticket> GetTicketByIdAsync(int ticketId)
    {
        try
        {
            Ticket ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Comments)
                .Include(t => t.Attachments)
                .Include(t => t.History)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            return ticket;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
    #endregion
    
    // No tracking means entity framework doesn't track the changes or stops it from tracking the entity. We're not changing anything with this method. We're using this method for ticket histories so we just want to check the state of the ticket.
    #region GetTicketAsNoTrackingAsync
    public async Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId)
    {
        try
        {
            Ticket ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            return ticket;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting ticket As No Tracking  --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Ticket Attachment By Id
    public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
    {
        try
        {
            TicketAttachment ticketAttachment = await _context.TicketAttachments
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
            return ticketAttachment;
        }
        catch (Exception)
        {

            throw;
        }
    }
    #endregion

    #region Archive Ticket
    public async Task ArchiveTicketAsync(Ticket ticket)
    {
        try
        {
            ticket.Archived = true;
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    #endregion
    
    #region Restore Ticket
    public async Task RestoreTicketAsync(Ticket ticket)
    {
        try
        {
            ticket.Archived = false;
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    #endregion
    
    #region Delete Ticket

    public async Task DeleteTicketAsync(Ticket ticket)
    {
    try
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            // return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
        
    #endregion

    #region Assign Ticket
    public async Task AssignTicketAsync(int ticketId, string userId)
    {
        Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        try
        {
            if (ticket != null)
            {
                try
                {
                    ticket.DeveloperUserId = userId;
                    ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error assigning ticket --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Archived Tickets
    public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
    {
        try
        {
            List<Ticket> tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.Archived == true).ToList();

            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting archived ticket --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get All Tickets By Company
    public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
    {
        try
        {
            // Get projects where company id is the same and select many (all) of the tickets with those projects. Select Many selects a collection of a collection.
            List<Ticket> tickets = await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                    .Include(t => t.Attachments)
                    .Include(t => t.History)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                .ToListAsync();

            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting all tickets by company  --->  {e.Message}");
            throw;
        }

    }
    #endregion
    
    #region Get All Tickets By Priority
    public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
    {
        // .value extents int to allow it to be null. I don't know why we can't do int? PriorityId
        int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;
        try
        {
            // Get projects where company id is the same and select many (all) of the tickets with those projects. Select Many selects a collection of a collection.
            // We could reuse the GetAllTicketsByCompanyAsync() function instead and then do the .Where on the return statement but this means we're asking for more data from the db and storing it in memory. 
            // It's more efficient to pull only the data we need.
            List<Ticket> tickets = await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                    .Include(t => t.Attachments)
                    .Include(t => t.History)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                .Where(t => t.TicketPriority.Id == priorityId)
                .ToListAsync();

            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting all tickets by priority  --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get All Tickets By Status
    public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
    {
        // .value extents int to allow it to be null. I don't know why we can't do int? PriorityId
        int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;
        try
        {
            // Get projects where company id is the same and select many (all) of the tickets with those projects. Select Many selects a collection of a collection.
            // We could reuse the GetAllTicketsByCompanyAsync() function instead and then do the .Where on the return statement but this means we're asking for more data from the db and storing it in memory. 
            // It's more efficient to pull only the data we need.
            List<Ticket> tickets = await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                    .Include(t => t.Attachments)
                    .Include(t => t.History)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                .Where(t => t.TicketStatus.Id == statusId)
                .ToListAsync();
            
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting all tickets by status  --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get All Tickets By Type
    public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
    {
        // .value extents int to allow it to be null. I don't know why we can't do int? PriorityId
        int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;
        try
        {
            // Get projects where company id is the same and select many (all) of the tickets with those projects. Select Many selects a collection of a collection.
            // We could reuse the GetAllTicketsByCompanyAsync() function instead and then do the .Where on the return statement but this means we're asking for more data from the db and storing it in memory. 
            // It's more efficient to pull only the data we need.
            List<Ticket> tickets = await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                    .Include(t => t.Attachments)
                    .Include(t => t.History)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                .Where(t => t.TicketType.Id == typeId)
                .ToListAsync();
            
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting all tickets by type  --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Ticket Developer
    public async Task<ITUser> GetTicketDeveloperAsync(int ticketId, int companyId)
    {
        ITUser developer = new();

        try
        {
            Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);

            if (ticket?.DeveloperUserId != null)
            {
                developer = ticket.DeveloperUser;
            }
            return developer;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting ticket developer --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Tickets By Role
    public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
    {
        List<Ticket> tickets = new();
        
        try
        {
            if (role == Roles.Admin.ToString())
            {
                tickets = await GetAllTicketsByCompanyAsync(companyId);
            }
            else if (role == Roles.Developer.ToString())
            {
                tickets = (await GetAllTicketsByCompanyAsync(companyId))
                    .Where(t => t.DeveloperUserId == userId)
                    .ToList();
            }
            else if (role == Roles.Submitter.ToString())
            {
                tickets = (await GetAllTicketsByCompanyAsync(companyId))
                    .Where(t => t.OwnerUserId == userId)
                    .ToList();
            }
            else if (role == Roles.ProjectManager.ToString())
            {
                tickets = await GetTicketsByUserIdAsync(userId, companyId);
            }

            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting tickets by role  --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Tickets By User Id
    public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
    {
        ITUser  itUser= await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        List<Ticket> tickets = new();
        
        try
        {
            if (await _rolesService.IsUserInRoleAsync(itUser, Roles.Admin.ToString()))
            {
                tickets = (await _projectService.GetAllProjectsByCompany(companyId))
                    .SelectMany(p => p.Tickets)
                    .ToList();
            }
            else if (await _rolesService.IsUserInRoleAsync(itUser, Roles.Developer.ToString()))
            {
                tickets = (await _projectService.GetAllProjectsByCompany(companyId))
                    .SelectMany(p => p.Tickets)
                    .Where(t => t.DeveloperUserId == userId)
                    .ToList();
            }
            else if (await _rolesService.IsUserInRoleAsync(itUser, Roles.Submitter.ToString()))
            {
                tickets = (await _projectService.GetAllProjectsByCompany(companyId))
                    .SelectMany(p => p.Tickets)
                    .Where(t => t.OwnerUserId == userId)
                    .ToList();
            }
            else if (await _rolesService.IsUserInRoleAsync(itUser, Roles.ProjectManager.ToString()))
            {
                tickets = (await _projectService.GetUserProjectsAsync(userId))
                    .SelectMany(p => p.Tickets)
                    .ToList();
            }
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting tickets by user id  --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Project Tickets By Role
    public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
    {
        List<Ticket> tickets = new();
        try
        {
            tickets = (await GetTicketsByRoleAsync(role, userId, companyId)).Where(t => t.ProjectId == projectId)
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting project tickets by role  --->  {e.Message}");
            throw;
        }
        return tickets;
    }
    #endregion
    
    #region Get Project Tickets By Status
    public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
    {
        List<Ticket> tickets = new();
        try
        {
            tickets = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(t => t.ProjectId == projectId)
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting project tickets by status  --->  {e.Message}");
            throw;
        }
        return tickets;
    }
    #endregion
    
    #region Get Project Ticket sBy Priority
    public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
    {
        List<Ticket> tickets = new();
        try
        {
            tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(t => t.ProjectId == projectId)
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting project tickets by priority  --->  {e.Message}");
            throw;
        }
        return tickets;
    }
    #endregion
    
    #region Get Project Tickets By Type
    public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
    {
        List<Ticket> tickets = new();
        try
        {
            tickets = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(t => t.ProjectId == projectId)
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting project tickets by type  --->  {e.Message}");
            throw;
        }
        return tickets;
    }
    #endregion

    #region Get Unassigned Tickets 

    public async Task<List<Ticket>> GetUnassignedTicketsAsync(int companyId)
    {
        List<Ticket> tickets = new();
        try
        {
            tickets = ( await GetAllTicketsByCompanyAsync(companyId) ).Where(t => string.IsNullOrEmpty(t.DeveloperUserId)).ToList();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting unassigned tickets --->  {e.Message}");
            throw;
        }
        return tickets;
    }

    #endregion
    
    #region Lookup Ticket Priority Id
    public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
    {
        try
        {
            TicketPriority priority = await _context.TicketPriorities.FirstOrDefaultAsync(p => p.Name == priorityName);
            
            // ? is null check. if priority is not null then return priority.Id
            return priority?.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error looking up ticket priority Id  --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Lookup Ticket Status Id
    public async Task<int?> LookupTicketStatusIdAsync(string statusName)
    {
        try
        {
            TicketStatus status = await _context.TicketStatuses.FirstOrDefaultAsync(p => p.Name == statusName);
            
            // ? is null check. if priority is not null then return status.Id
            return status?.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error looking up ticket status Id  --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Lookup Ticket Type Id
    public async Task<int?> LookupTicketTypeIdAsync(string typeName)
    {
        try
        {
            TicketType type = await _context.TicketTypes.FirstOrDefaultAsync(p => p.Name == typeName);
            
            // ? is null check. if priority is not null then return priority.Id
            return type?.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error looking up ticket type Id  --->  {e.Message}");
            throw;
        }
    }
    #endregion

    #region RemoveMemberFromAllTicketsAsync

    public async Task RemoveMemberFromAllTicketsAsync(int companyId, string memberId)
    {
        List<Ticket> tickets = await _context.Tickets.Where(t => t.DeveloperUserId == memberId).ToListAsync();

        foreach (Ticket ticket in tickets)
        {
            ticket.DeveloperUserId = null;
            ticket.TicketStatusId = await LookupTicketStatusIdAsync( nameof(ITTicketStatus.Unassigned) );
        }

        await _context.SaveChangesAsync();
    }
    

    #endregion
}