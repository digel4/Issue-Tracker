using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.Models.ViewModels;

public class EditMemberViewModel
{
    public ITUser Member { get; set; } = default!;
    // public string JobTitle { get; set; } = default!;
    // public SelectList AvailableRoles { get; set; } = default!;
    // public string? SelectedRole { get; set; }
}