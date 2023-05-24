namespace IssueTracker.Models.ViewModels;

public class DashboardViewModel
{
    public List<Project> ActiveProjects { get; set; }
    
    public List<Ticket> OpenTickets { get; set; }
    
    public List<Ticket> CompletedTickets { get; set; }
    
    public List<ITUser> Members { get; set; }
    
    public List<Notification> Notifications { get; set; }
    
    public int AdminCount { get; set; }
    
    public int ProjectManagerCount { get; set; }
    
    public int DeveloperCount { get; set; }
    
    public int SubmitterCount { get; set; }
}