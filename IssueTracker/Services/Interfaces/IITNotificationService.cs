using IssueTracker.Models;

namespace IssueTracker.Services.Interfaces;

public interface IITNotificationService
{
    
    public Task AddNotificationAsync(Notification notification);
    
    public Task<List<Notification>> GetReceivedNotificationsAsync(string userId);

    public Task<List<Notification>> GetSentNotificationsAsync(string userId);
    
    public Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);
    
    public Task SendMembersEmailNotificationsAsync(Notification notification, List<ITUser> members);
    
    public Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject);
}