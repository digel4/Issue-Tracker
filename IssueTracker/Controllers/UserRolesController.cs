using IssueTracker.Extensions;
using IssueTracker.Models;
using IssueTracker.Models.ViewModels;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.Controllers;

[Authorize]
public class UserRolesController : Controller
{
    private readonly IRolesService _rolesService;
    private readonly ICompanyInfoService _companyInfoService;
    public UserRolesController(
        IRolesService rolesService, 
        ICompanyInfoService companyInfoService
    )
    {
        _rolesService = rolesService;
        _companyInfoService = companyInfoService;
    }

    [HttpGet]
    public async Task<IActionResult> ManageUserRoles()
    {
        // Add an instance of the ViewModel as a list (model)
        List<ManageUserRolesViewModel> model = new();
        
        // Get CompanyId
        // GetCompanyId returns a nullable int but companyId expects a normal int. .Value solves this
        int companyId = User.Identity.GetCompanyId().Value;
        
        // Get all company User
        List<ITUser> users = await _companyInfoService.GetAllMembersAsync(companyId);
        
        // Loop over the users to populate the ViewModel
        // - instantiate ViewModel
        // - use _rolesService
        // - Create multi-selects
        foreach (ITUser user in users)
        {
            ManageUserRolesViewModel viewModel = new();
            viewModel.ITUser = user;

            IEnumerable<string> selected = await _rolesService.GetSingleUserRolesAsync(user);

            viewModel.Roles = new MultiSelectList( await _rolesService.GetRolesAsync(), "Name", "Name", selected );
            
            model.Add(viewModel);
        }
        // Return the model to the view
        return View(model);
    }

    [HttpPost]
    // protects from CORS
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
    {
        // Get the company Id
        int companyId = User.Identity.GetCompanyId().Value;

        // Instantiate the ITUser
        ITUser itUser = (await _companyInfoService.GetAllMembersAsync(companyId))
            .FirstOrDefault(u => u.Id == member.ITUser.Id);

        // Get Roles for the User
        IEnumerable<string> roles = await _rolesService.GetSingleUserRolesAsync(itUser);

        // Grab the selected role
        string userRole = member.SelectedRoles.FirstOrDefault();

        if (!string.IsNullOrEmpty(userRole))
        {
            // Remove User from their roles
            if (await _rolesService.RemoveUserFromManyRolesAsync(itUser, roles))
            {
                // Add User to the new role
                await _rolesService.AddUserToRoleAsync(itUser, userRole);
            }
        }
        // Navigate back to the view
        return RedirectToAction(nameof(ManageUserRoles));
    }
}