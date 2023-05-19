namespace IssueTracker.Models.ViewModels;


public class ProjectViewModel
{
    public Project Project { get; set; } = default!;
    public ITUser? ProjectManager { get; set; }
    public List<ITUser> Developers { get; set; } = default!;
    public List<ITUser> Submitters { get; set; } = default!;
    public IEnumerable<Ticket> Tickets { get; set; } = default!;

    public string GetDeveloperTextFormat(Ticket ticket) => ticket.DeveloperUserId != null ? "" : "text-danger";
    public string GetPriorityBadgeFormat(Ticket ticket) => ticket.TicketPriority!.Name switch
    {
        "Low" => "bg-success",
        "Medium" => "bg-primary",
        "High" => "bg-danger",
        "Urgent" => "bg-danger",
        _ => "bg-danger"
    };

    public string GetStatusTextFormat(Ticket ticket) => ticket.TicketStatus!.Name switch
    {
        "Unassigned" => "fw-bold text-danger",
        "Testing" => "fw-bold text-danger",
        "Development" => "fw-bold text-dark",
        "New" => "fw-bold text-danger",
        "Complete" => "fw-bold text-success",
        _ => "text-danger"
    };

}