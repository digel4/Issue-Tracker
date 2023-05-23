using IssueTracker.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.Models.ViewModels;

public class CreateOrEditProjectViewModel
{
    public Project Project { get; set; } = default!;

    [MaxFileSize(1024 * 1024)]
    [AllowedExtensions(new string[] { ".gif", ".jpg", ".png", ".jpeg" })]
    public IFormFile? Image { get; set; }
    public DateTimeOffset EndDate { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public SelectList ProjectManagers { get; set; } = default!;
    public string SelectedManager { get; set; } = default!;
    public MultiSelectList Developers { get; set; } = default!;
    public IEnumerable<string>? SelectedDevelopers { get; set; } = default!;
    public MultiSelectList Submitters { get; set; } = default!;
    public IEnumerable<string>? SelectedSubmitters { get; set; } = default!;
}