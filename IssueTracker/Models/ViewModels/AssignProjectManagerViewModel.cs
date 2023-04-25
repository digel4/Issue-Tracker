using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.Models.ViewModels;

public class AssignProjectManagerViewModel
{
    public Project Project { get; set; }

    public SelectList ProjectManagerList { get; set; }

    public String ProjectManagerId { get; set; }
}