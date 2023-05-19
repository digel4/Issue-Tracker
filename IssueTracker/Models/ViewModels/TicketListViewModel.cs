using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.Models.ViewModels;

public class TicketListViewModel
{
    public string TicketListType { get; set; } = default!;
    public List<Ticket> Tickets { get; set; } = default!;
    public string PageNumber { get; set; } = default!;
    public string PerPage { get; set; } = default!;
    public string SortBy { get; set; } = default!;


    public SelectList SortByOptions { get; set; } = default!;
    public SelectList PerPageOptions { get; set; } = default!;

    public string GetPriorityFormatting(string priorityId) => priorityId switch
    {
        "Low" => "badge bg-success",
        "Medium" => "badge bg-info",
        "High" => "badge bg-danger",
        "Urgent" => "badge bg-danger",
        _ => "",
    };

    public string GetStatusFormatting(string statusId) => statusId switch
    {
        "Unassigned" => "fw-bold text-danger",
        "New" => "fw-bold text-danger",
        "Development" => "text-dark",
        "Testing" => "fw-bold text-danger",
        "Resolved" => "fw-bold text-success",
        _ => "",
    };
}

