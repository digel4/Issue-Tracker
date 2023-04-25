using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IssueTracker.Models.ViewModels;

public class AddProjectWithPMViewModel
{
    public Project Project { get; set; }
    
    public SelectList PMList { get; set; }
    
    public string PMId { get; set; }
    
    public SelectList PriorityList { get; set; }
}