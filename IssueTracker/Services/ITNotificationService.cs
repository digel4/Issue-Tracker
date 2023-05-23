using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace IssueTracker.Services;

public class ITNotificationService : IITNotificationService
{
    #region Properties
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IITRolesService  _rolesService;
    #endregion
    
    #region Constructor
    public ITNotificationService(
        ApplicationDbContext context,
        IEmailSender emailSender,
        IITRolesService rolesService
    )
    {
        // Dependency Injection /  service layer
        _context = context;
        _emailSender = emailSender;
        _rolesService = rolesService;
    }
    #endregion
    
    #region Add Notification
    public async Task AddNotificationAsync(Notification notification)
    {
        try
        {
            await _context.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error adding notification. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region GetReceived Notifications
    public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
    {
        try
        {
            List<Notification> notifications = await _context.Notifications
                .Include(n => n.Recipent)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                    .ThenInclude(t => t.Project)
                .Where(n => n.RecipentId == userId)
                .ToListAsync();

            return notifications;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting received notifications. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Sent Notifications
    public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
    {
        try
        {
            List<Notification> notifications = await _context.Notifications
                .Include(n => n.Recipent)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                    .ThenInclude(t => t.Project)
                .Where(n => n.SenderId == userId)
                .ToListAsync();

            return notifications;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting sent notifications. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Send Email Notifications By Role 
    public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
    {
        try
        {
            List<ITUser> members = await _rolesService.GetManyUsersNotInRoleAsync(role, companyId);

            foreach (ITUser itUser in members)
            {
                notification.RecipentId = itUser.Id;
                await SendEmailNotificationAsync(notification, notification.Title);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error sending email notifications by role. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Send Members Email Notifications
    public async Task SendMembersEmailNotificationsAsync(Notification notification, List<ITUser> members)
    {
        try
        {
            foreach (ITUser itUser in members)
            {
                notification.RecipentId = itUser.Id;
                await SendEmailNotificationAsync(notification, notification.Title);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error sending members email notifications. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Send Email Notification
    public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
    {
        ITUser itUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipentId);

        if (itUser != null)
        {
            string itUserEmail = itUser.Email;
            string message = notification.Message;

            //Send Email
            try
            {
                await _emailSender.SendEmailAsync(itUserEmail, emailSubject, message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"****ERROR**** - Error sending email notification. --->  {e.Message}");
                throw;
            }
        }
        return false;

    }
    #endregion
    
    
    #region Get By Id
    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications
            .Where(n => n.Id == id)
            .Include(n => n.Ticket)
            .Include(n => n.Project)
            .FirstOrDefaultAsync();
    }
    #endregion
    
    #region Get Unseen Notifications For User
    public async Task<List<Notification>> GetUnseenNotificationsForUserAsync(ITUser itUser)
    {
        return await _context.Notifications.Where(n => n.RecipentId == itUser.Id)
            .Where(n => n.Viewed == false)
            .OrderByDescending(n => n.Created)
            .Include(n => n.Ticket)
            .Include(n => n.Project)
            .ToListAsync();
    }
    #endregion
    
    #region Get All Notifications For User
    public async Task<List<Notification>> GetAllNotificationsForUserAsync(ITUser itUser)
    {
        return await _context.Notifications.Where(n => n.RecipentId == itUser.Id)
            .OrderByDescending(n => n.Created)
            .Include(n => n.Ticket)
            .Include(n => n.Project)
            .ToListAsync();
    }
    #endregion
    
    #region Mark As Read
    public async Task MarkAsRead(Notification notification)
    {
        notification.Viewed = true;

        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }
    #endregion
    
    #region Create Invited ToCompany Notification
    public async Task<bool> CreateInvitedToCompanyNotification(ITUser sendingUser, ITUser receivingUser)
    {
        Notification invitedToCompanyNotification = new Notification
        {
            RecipentId = receivingUser.Id,
            SenderId = sendingUser.Id,
            CompanyId = sendingUser.CompanyId.Value,
            Created = DateTime.Now,
            Title = $"New invite",
            Message = "You've been invited to join a company.",
            NotificationTypeId = (int)NotificationType.CompanyInvite,
        };

        try
        {
            await _context.Notifications.AddAsync(invitedToCompanyNotification);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
    
    #region Create Invite Accepted Notification
    public async Task<bool> CreateInviteAcceptedNotification(int originalInviteId, ITUser invitee)
    {
        Notification? originalInvite = await GetByIdAsync(originalInviteId);

        if (originalInvite is null)
            return false;

        Notification inviteAcceptedNotification = new Notification
        {
            SenderId = originalInvite.SenderId!,
            RecipentId = originalInvite.RecipentId,
            CompanyId = originalInvite.CompanyId,
            Created = DateTime.Now,
            Title = $"Invite accepted",
            Message = "Your company has a new member.",
            NotificationTypeId = (int)NotificationType.CompanyInviteAccepted,
        };

        try
        {
            await _context.Notifications.AddAsync(inviteAcceptedNotification);
            await RemoveAllCompanyInvitesForUserAsync(invitee); // user shouldn't be able to join another company now
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
    
    #region Create Invite Rejected Notification
    public async Task<bool> CreateInviteRejectedNotification(int originalInviteId, ITUser invitee)
    {
        Notification? originalInvite = await GetByIdAsync(originalInviteId);

        if (originalInvite is null)
            return false;

        Notification inviteRejectedNotification = new Notification
        {
            SenderId = originalInvite.SenderId!,
            CompanyId = originalInvite.CompanyId,
            Created = DateTime.Now,
            Title = $"Invite declined",
            Message = "A user declined your company invite.",
            NotificationTypeId = (int)NotificationType.CompanyInviteRejected,
        };

        try
        {
            await _context.Notifications.AddAsync(inviteRejectedNotification);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
    
    #region Create New Project Notification
	public async Task<bool> CreateNewProjectNotificationAsync(string userId, Project project)
	{
		ITUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

		if (user is null)
			return false;

		Notification newProjectNotification = new Notification
		{
			RecipentId = userId,
            SenderId = userId,
			CompanyId = user.CompanyId.Value,
			Created = DateTime.Now,
            ProjectId = project.Id,
			Title = "New project",
            Message = "You have a new project to work on.",
			NotificationTypeId = (int)NotificationType.NewProject,
		};

		await _context.Notifications.AddAsync(newProjectNotification);
		await _context.SaveChangesAsync();
		return true;
	}
    #endregion
    
    #region Create New Ticket Notification
	public async Task<bool> CreateNewTicketNotificationAsync(string developerId, Ticket ticket)
    {
        ITUser? developer = await _context.Users.FirstOrDefaultAsync(u => u.Id == developerId);

        if (developer is null)
            return false;

        Notification newTicketNotification = new Notification
        {
            SenderId = developerId,
            RecipentId = developerId,
            CompanyId = developer.CompanyId.Value,
            Created = DateTime.Now,
            ProjectId = ticket.ProjectId,
            TicketId = ticket.Id,
            Title = $"New ticket",
            Message = "You have a new ticket to work on.",
            NotificationTypeId = (int)NotificationType.NewTicket,
        };

        await _context.Notifications.AddAsync(newTicketNotification);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region Create Mention Notification
    public async Task<bool> CreateMentionNotificationAsync(string mentionedUserId, string commentingUserId, int ticketId)
    {
        ITUser? mentionedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == mentionedUserId);
        ITUser? commentingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == commentingUserId);
        Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        if (mentionedUser is null || commentingUser is null || ticket is null)
            return false;

        Notification newMentionNotification = new Notification
        {
            RecipentId = mentionedUserId,
            CompanyId = commentingUser.CompanyId.Value,
            SenderId = commentingUserId,
            TicketId = ticketId,
            ProjectId = ticket.ProjectId,
            Created = DateTime.Now,
            Title = $"New comment",
            Message = "Someone mentioned you in a comment.",
            NotificationTypeId = (int)NotificationType.Mention,
        };

        await _context.Notifications.AddAsync(newMentionNotification);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion
    
    #region Remove All Company Invites For User
    private async Task RemoveAllCompanyInvitesForUserAsync(ITUser appUser)
    {
        List<Notification> companyInvites = await _context.Notifications
            .Where(n => n.RecipentId == appUser.Id && n.NotificationTypeId == (int)NotificationType.CompanyInvite)
            .ToListAsync();

        foreach (Notification notification in companyInvites)
        {
            _context.Notifications.Remove(notification);
        }

        await _context.SaveChangesAsync();
    }
    #endregion
    
    #region Create Removed From Company Notification
    public async Task<bool> CreateRemovedFromCompanyNotification(int companyId, ITUser member)
    {
        Notification removedFromCompanyNotification = new()
        {
            RecipentId = member.Id,
            CompanyId = companyId,
            Created = DateTime.Now,
            Title = $"You were removed from your company.",
            Message = "You were removed from your company.",
            NotificationTypeId = (int)NotificationType.RemovedFromCompany,
        };

        try
        {
			await _context.Notifications.AddAsync(removedFromCompanyNotification);
            await _context.SaveChangesAsync();
            return true;
		}
        catch
        {
            return false;
        }
    }
    #endregion
}