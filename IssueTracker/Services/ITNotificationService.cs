using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace IssueTracker.Services;

public class ITNotificationService : IITNotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IITRolesService  _rolesService;
    // Constructor
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

}