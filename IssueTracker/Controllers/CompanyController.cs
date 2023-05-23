// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using IssueTracker.Data;
// using IssueTracker.Models;
//
// namespace IssueTracker.Controllers
// {
//     public class CompanyController : Controller
//     {
//         private readonly ApplicationDbContext _context;
//         
//         // contructur
//         public CompanyController(ApplicationDbContext context)
//         {
//             // Dependency Injection /  service layer
//             _context = context;
//         }
//
//         // GET: Company
//         public async Task<IActionResult> Index()
//         {
//               return _context.Companies != null ? 
//                           View(await _context.Companies.ToListAsync()) :
//                           Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
//         }
//
//         // GET: Company/Details/5
//         public async Task<IActionResult> Details(int? id)
//         {
//             if (id == null || _context.Companies == null)
//             {
//                 return NotFound();
//             }
//
//             var company = await _context.Companies
//                 .FirstOrDefaultAsync(m => m.Id == id);
//             if (company == null)
//             {
//                 return NotFound();
//             }
//
//             return View(company);
//         }
//
//         // GET: Company/Create
//         public IActionResult Create()
//         {
//             return View();
//         }
//
//         // POST: Company/Create
//         // To protect from overposting attacks, enable the specific properties you want to bind to.
//         // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create([Bind("Id,Name,Description")] Company company)
//         {
//             if (ModelState.IsValid)
//             {
//                 _context.Add(company);
//                 await _context.SaveChangesAsync();
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(company);
//         }
//
//         // GET: Company/Edit/5
//         public async Task<IActionResult> Edit(int? id)
//         {
//             if (id == null || _context.Companies == null)
//             {
//                 return NotFound();
//             }
//
//             var company = await _context.Companies.FindAsync(id);
//             if (company == null)
//             {
//                 return NotFound();
//             }
//             return View(company);
//         }
//
//         // POST: Company/Edit/5
//         // To protect from overposting attacks, enable the specific properties you want to bind to.
//         // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Company company)
//         {
//             if (id != company.Id)
//             {
//                 return NotFound();
//             }
//
//             if (ModelState.IsValid)
//             {
//                 try
//                 {
//                     _context.Update(company);
//                     await _context.SaveChangesAsync();
//                 }
//                 catch (DbUpdateConcurrencyException)
//                 {
//                     if (!CompanyExists(company.Id))
//                     {
//                         return NotFound();
//                     }
//                     else
//                     {
//                         throw;
//                     }
//                 }
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(company);
//         }
//
//         // GET: Company/Delete/5
//         public async Task<IActionResult> Delete(int? id)
//         {
//             if (id == null || _context.Companies == null)
//             {
//                 return NotFound();
//             }
//
//             var company = await _context.Companies
//                 .FirstOrDefaultAsync(m => m.Id == id);
//             if (company == null)
//             {
//                 return NotFound();
//             }
//
//             return View(company);
//         }
//
//         // POST: Company/Delete/5
//         [HttpPost, ActionName("Delete")]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> DeleteConfirmed(int id)
//         {
//             if (_context.Companies == null)
//             {
//                 return Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
//             }
//             var company = await _context.Companies.FindAsync(id);
//             if (company != null)
//             {
//                 _context.Companies.Remove(company);
//             }
//             
//             await _context.SaveChangesAsync();
//             return RedirectToAction(nameof(Index));
//         }
//
//         private bool CompanyExists(int id)
//         {
//           return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
//         }
//     }
// }

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using IssueTracker.Extensions;
using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Models.ViewModels;
using IssueTracker.Services.Interfaces;
using System.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IssueTracker.Controllers;

public class CompanyController : Controller
{
    #region Properties
    private readonly UserManager<ITUser> _userManager;
    private readonly ICompanyInfoService _companyInfoService;
    private readonly IProjectService _projectService;
    private readonly ITicketService _ticketService;
    private readonly INotificationService _notificationService;
    private readonly IRolesService _rolesService;
    private readonly IFileService _fileService;
    #endregion

    #region Contstructor
    public CompanyController(UserManager<ITUser> userManager, 
        ICompanyInfoService companyService,
        IProjectService projectService,
        ITicketService ticketService,
        INotificationService notificationService,
        IRolesService rolesService,
        IFileService fileService)
    {
        _userManager = userManager;
        _companyInfoService = companyService;
        _projectService = projectService;
        _ticketService = ticketService;
        _notificationService = notificationService;
        _rolesService = rolesService;
        _fileService = fileService;
    }
    #endregion
    
    #region View Member
    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ViewMember(string userId)
    {
        ITUser? itUser = await _userManager.FindByIdAsync(userId);

        if (itUser is null)
            return View("NotFound");

        if (itUser.CompanyId != User.Identity!.GetCompanyId())
            return View("NotAuthorized");

        return View(itUser);
    }
    #endregion
    
    #region Update Member Profile
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateMemberProfile(ITUser updatedUser)
    {
        ITUser user = await _userManager.GetUserAsync(User);

        if (ModelState["AvatarFormFile"] != null && ModelState["AvatarFormFile"]!.ValidationState == ModelValidationState.Invalid)
        {
            ModelState.AddModelError(string.Empty, "Profile picture is too large or the wrong extension type.");
            return View("ViewMember", user);
        }

        if (updatedUser.AvatarFormFile != null)
        {
            updatedUser.AvatarFileData = await _fileService.ConvertFileToByteArrayAsync(updatedUser.AvatarFormFile);
            updatedUser.AvatarContentType = updatedUser.AvatarFormFile.ContentType;
        }
        else
        {
            updatedUser.AvatarFileData = user.AvatarFileData;
            updatedUser.AvatarContentType = user.AvatarContentType;
        }

        await _companyInfoService.UpdateMemberProfileAsync(updatedUser);

        return RedirectToAction(nameof(ViewMember), new { userId = user.Id });
    }
    #endregion
    
    #region Delete Avatar Image
    [HttpGet]
    [Authorize]
    public async Task<RedirectToActionResult> DeleteAvatarImage(string userId)
    {
        if ((await _userManager.GetUserAsync(User)).Id != userId)
            return RedirectToAction(nameof(NotAuthorized));

        await _companyInfoService.DeleteAvatarImageAsync(userId);

        return RedirectToAction(nameof(ViewMember), new { userId = userId });
    }
    #endregion
    
    #region Manage Members
    [HttpGet]
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<ViewResult> ManageMembers()
    {
        int companyId = User.Identity!.GetCompanyId().Value;

        List<ITUser> companyMembers = await _companyInfoService.GetAllMembersAsync(companyId);
        return View(companyMembers);
    }
    #endregion
    
    #region Edit Member (1)
    [HttpGet]
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<ViewResult> EditMember(string id)
    {
        ITUser? memberToEdit = await _userManager.FindByIdAsync(id);

        if (memberToEdit == null)
            return View("NotFound");

        IEnumerable<IdentityRole> roles = await _rolesService.GetRolesAsync();
        // string? selectedRole = (await _rolesService.GetUserRolesAsync(employeeToEdit)).FirstOrDefault();

        EditMemberViewModel viewModel = new()
        {
            Member = memberToEdit,
            // AvailableRoles = new SelectList(roles, "Name", "Name", selectedRole),
            // SelectedRole = selectedRole
        };

        return View(viewModel);
    }
    #endregion
    
    #region Edit Member (2)
    [HttpPost]
    [Authorize(Roles = nameof(Roles.Admin))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMember(EditMemberViewModel viewModel)
    {
        ITUser? employeeToEdit = await _userManager.FindByIdAsync(viewModel.Member.Id);

        if (employeeToEdit is null)
            return View("NotFound");

        if (await _userManager.IsInRoleAsync(employeeToEdit, nameof(Roles.Admin)))
        {
            List<ITUser> admins = await _companyInfoService.GetAllAdminsAsync(employeeToEdit.CompanyId.Value);

            if (admins.Count == 1)
            {
                ViewData["Error"] = "You cannot remove the last administrator for your company! You must designate another user as an administrator first.";
                IEnumerable<IdentityRole> roles = await _rolesService.GetRolesAsync();

                EditMemberViewModel newViewModel = new()
                {
                    Member = employeeToEdit,
                    // JobTitle = viewModel.JobTitle,
                    // AvailableRoles = new SelectList(roles, "Name", "Name", "Admin"),
                    // SelectedRole = "Admin",
                };

                return View(newViewModel);
            }
        }

        // if (viewModel.JobTitle != employeeToEdit.JobTitle)
        // {
        //     employeeToEdit.JobTitle = viewModel.JobTitle;
        // }

        // if (viewModel.SelectedRole is not null)
        // {
        //     IEnumerable<string> oldRoles = await _roleService.GetUserRolesAsync(employeeToEdit);
        //     await _roleService.RemoveUserFromRolesAsync(employeeToEdit, oldRoles);
        //     await _roleService.AddUserToRoleAsync(employeeToEdit, viewModel.SelectedRole);
        // }

        TempData["Message"] = "Employee changes saved!";
        return RedirectToAction(nameof(ManageMembers));
    }
    #endregion
    
    #region Confirm Remove Member
    [HttpGet]
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<IActionResult> ConfirmRemoveMember(string employeeId)
    {
        ITUser? employeeToRemove = await _userManager.FindByIdAsync(employeeId);

        if (employeeToRemove is null)
            return View("NotFound");

        if (await _userManager.IsInRoleAsync(employeeToRemove, nameof(Roles.Admin)))
        {
            List<ITUser> admins = await _companyInfoService.GetAllAdminsAsync(employeeToRemove.CompanyId.Value);

            if (admins.Count == 1)
            {
                TempData["Error"] = "You cannot remove the last administrator for your company! You must designate another user as an administrator first.";
                IEnumerable<IdentityRole> roles = await _rolesService.GetRolesAsync();

                return RedirectToAction(nameof(ManageMembers));
            }
        }

        return View(employeeToRemove);
    }
    #endregion
    
    #region Remove Member Confirmed
    [HttpPost]
    [Authorize(Roles = nameof(Roles.Admin))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMemberConfirmed([Bind("Id")] ITUser employee)
    {
        int companyId = User.Identity!.GetCompanyId().Value;
        ITUser? userRemoved = await _userManager.FindByIdAsync(employee.Id);

        if (userRemoved is null)
            return View("NotFound");

        await _companyInfoService.RemoveMemberAsync(userRemoved.Id);
        await _projectService.RemoveMemberFromAllProjectsAsync(companyId, userRemoved.Id);
        await _ticketService.RemoveMemberFromAllTicketsAsync(companyId, userRemoved.Id);
        await _notificationService.CreateRemovedFromCompanyNotification(companyId, userRemoved);

        TempData["Message"] = $"{userRemoved.FirstName} {userRemoved.LastName} was successfully removed.";
        return RedirectToAction(nameof(ManageMembers));
	}
    #endregion
    
    #region Invite User To Company
    [HttpGet]
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<ViewResult> InviteUserToCompany()
    {
        int companyId = User.Identity!.GetCompanyId().Value;
        Company? company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
        ViewData["CompanyName"] = company is not null ? company.Name : "your company";
      
        return View(new ITUser());
    }
    #endregion
    
    #region Invite User To Company
    [HttpPost]
    [Authorize(Roles = nameof(Roles.Admin))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InviteUserToCompany([Bind("Email")] ITUser user)
    {
        ITUser? receivingUser = await _userManager.FindByEmailAsync(user.Email!);

        ITUser sendingUser = (await _userManager.GetUserAsync(User))!;
        Company? company = await _companyInfoService.GetCompanyInfoByIdAsync(sendingUser.CompanyId!.Value);
        ViewData["CompanyName"] = company is not null ? company.Name : "your company";
        
        ViewData["Errors"] = await GetInviteErrors(sendingUser, receivingUser);

        if (ViewData["Errors"] is not null)
            return View();

        if (!await _notificationService.CreateInvitedToCompanyNotification(sendingUser, receivingUser!))
        {
            ViewData["Errors"] = "Something went wrong, please contact the site administrator.";
            return View();
        }

        ViewData["Message"] = $"{receivingUser!.Email} ({receivingUser.FullName}) has been invited!";
        return View();
    }
    #endregion
    
    #region Accept Company Invite
    [HttpGet]
    [Authorize]
    public async Task<ViewResult> AcceptCompanyInvite(int notificationId)
    {
        Notification? notification = await _notificationService.GetByIdAsync(notificationId);

        if (notification is null || notification.NotificationTypeId != (int)NotificationType.CompanyInvite)
            return View("NotAuthorized");

        Company? company = await _companyInfoService.GetCompanyInfoByIdAsync(notification.CompanyId);
        ITUser? appUser = await _userManager.GetUserAsync(User);

        if (appUser is null || company is null)
            return View("NotFound");

        IEnumerable<string> currentUserRoles = await _rolesService.GetSingleUserRolesAsync(appUser);
        await _rolesService.RemoveUserFromManyRolesAsync(appUser, currentUserRoles);
        
        await _companyInfoService.AddMemberAsync(appUser, company.Id);
		await _rolesService.AddUserToRoleAsync(appUser, nameof(Roles.Submitter));
		await _notificationService.CreateInviteAcceptedNotification(notification.Id, appUser);

        ViewData["CompanyName"] = $"{company.Name}";

        return View();
    }
    #endregion
    
    #region Reject Company Invite
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> RejectCompanyInvite(int notificationId)
    {
        Notification? notification = await _notificationService.GetByIdAsync(notificationId);

        if (notification is null || notification.NotificationTypeId != (int)NotificationType.CompanyInvite)
            return View("NotAuthorized");

        Company? company = await _companyInfoService.GetCompanyInfoByIdAsync(notification.CompanyId);
        ITUser? appUser = await _userManager.GetUserAsync(User);

        if (appUser is null || company is null)
            return View("NotFound");

        notification.Viewed = true;
        await _notificationService.CreateInviteRejectedNotification(notification.Id, appUser);

        ViewData["CompanyName"] = $"{company.Name}";

        return RedirectToAction(nameof(NotificationController.MyNotifications), "Notification");
    }
    #endregion
    
    #region Not Authorized
    [HttpGet]
    public ViewResult NotAuthorized()
    {
        return View();
    }
    #endregion

	#region Private Helper Methods
	private async Task<string?> GetInviteErrors(ITUser sendingUser, ITUser? receivingUser)
    {
        if (receivingUser is null)
            return "Could not find a user with that e-mail address.";

        if (receivingUser.CompanyId == sendingUser.CompanyId)
            return "That user has already joined your company!";

        if (receivingUser.CompanyId is not null)
            return "That user has already joined a company.";

        List<Notification> existingNotifications = await _notificationService.GetUnseenNotificationsForUserAsync(receivingUser);

        if (existingNotifications
            .Where(n => (n.CompanyId == sendingUser.CompanyId) && (n.NotificationType == NotificationType.CompanyInvite))
            .Any())
            return "That user already has a pending invite to your company.";

        return null;
    }
    #endregion
}
