using IssueTracker.Models;

namespace IssueTracker.Services.Interfaces;

public interface INotificationService
{
    
    public Task AddNotificationAsync(Notification notification);
    
    public Task<List<Notification>> GetReceivedNotificationsAsync(string userId);

    public Task<List<Notification>> GetSentNotificationsAsync(string userId);
    
    public Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);
    
    public Task SendMembersEmailNotificationsAsync(Notification notification, List<ITUser> members);
    
    public Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject);
    
    
    Task<Notification?> GetByIdAsync(int id);
    Task<List<Notification>> GetUnseenNotificationsForUserAsync(ITUser itUser);
    Task<List<Notification>> GetAllNotificationsForUserAsync(ITUser itUser);
    Task MarkAsRead(Notification notification);
    Task<bool> CreateInvitedToCompanyNotification(ITUser sendingUser, ITUser receivingUser);
    Task<bool> CreateInviteAcceptedNotification(int originalInviteId, ITUser invitee);
    Task<bool> CreateInviteRejectedNotification(int originalInviteId, ITUser invitee);
    Task<bool> CreateNewProjectNotificationAsync(string userId, Project project);
    Task<bool> CreateNewTicketNotificationAsync(string developerId, Ticket ticket);
    Task<bool> CreateMentionNotificationAsync(string mentionedUserId, string commentingUserId, int ticketId);
    Task<bool> CreateRemovedFromCompanyNotification(int companyId, ITUser member);
}