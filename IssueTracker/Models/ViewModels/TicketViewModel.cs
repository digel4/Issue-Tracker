using IssueTracker.Extensions;

namespace IssueTracker.Models.ViewModels;

public class TicketViewModel
{
    public Ticket Ticket { get; set; } = default!;
    public Project Project { get; set; } = default!;
    public ITUser? ProjectManager { get; set; }
    public ITUser? Developer { get; set; }
    public string NewComment { get; set; } = String.Empty;

    [MaxFileSize(1024 * 1024)]
    [AllowedExtensions(new string[] { ".css", ".doc", ".gif", ".html", ".jpg", ".js", ".pdf", ".sql", ".tif", ".txt", ".xml", ".zip", ".cs", ".cshtml", ".json" })]
    public IFormFile? NewAttachment { get; set; }
    public string? FileDescription { get; set; }

    public string StatusTextFormat => Ticket.TicketStatus is null ? "" : Ticket.TicketStatus.Name switch
    {
        "Unassigned" => "fw-bold text-danger",
        "Testing" => "fw-bold text-danger",
        "Development" => "fw-bold text-dark",
        "New" => "fw-bold text-danger",
        "Complete" => "fw-bold text-success",
        _ => "text-danger"
    };

    public string PriorityBadgeType => Ticket.TicketPriority is null ? "" : Ticket.TicketPriority!.Name switch
    {
        "Low" => "bg-success",
        "Medium" => "bg-primary",
        "High" => "bg-danger",
        "Urgent" => "bg-danger",
        _ => "bg-danger",
    };
}