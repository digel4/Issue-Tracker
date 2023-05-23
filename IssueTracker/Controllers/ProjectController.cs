using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Data;
using IssueTracker.Extensions;
using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Models.ViewModels;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IssueTracker.Controllers
{
    // Authorise the entire controller with the authorize attribute
    [Authorize]
    public class ProjectController : Controller
    {
        #region Properties
        private readonly IProjectService _projectService;
        private readonly IRolesService _rolesService;
        private readonly ILookUpService _lookUpService;
        private readonly IFileService _fileService;
        private readonly UserManager<ITUser> _userManager;
        private readonly ICompanyInfoService _companyInfoService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructor
        public ProjectController(
            IProjectService ProjectService, 
            IRolesService RolesService, 
            ILookUpService LookUpService, 
            IFileService FileService, 
            UserManager<ITUser> userManager, 
            ICompanyInfoService CompanyInfoService,
            INotificationService NotificationService 
        )
        {
            _projectService = ProjectService;
            _rolesService = RolesService;
            _lookUpService =LookUpService;
            _fileService = FileService;
            _userManager = userManager;
            _companyInfoService = CompanyInfoService;
            _notificationService = NotificationService;
        }
        #endregion

        // #region Get My Projects
        // // GET: MyProjects
        // public async Task<IActionResult> MyProjects()
        // {
        //     // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
        //     string userId = _userManager.GetUserId(User);
        //     
        //     List<Project> projects = await _projectService.GetUserProjectsAsync(userId);
        //
        //     return View(projects);
        // }
        // #endregion
        //
        // #region Get My Archived Projects
        // // GET: MyProjects
        // public async Task<IActionResult> MyArchivedProjects()
        // {
        //     // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
        //     string userId = _userManager.GetUserId(User);
        //     
        //     List<Project> projects = await _projectService.GetUserArchivedProjectsAsync(userId);
        //
        //     return View(projects);
        // }
        // #endregion
        
        #region Get All Projects
        // GET: AllProjects
        public async Task<IActionResult> AllProjects(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
        {
            //
            // List<Project> projects = new();
            // // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            // int companyId = User.Identity.GetCompanyId().Value;
            //
            // if ( User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)) )
            // {
            //     projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            // }
            // else
            // {
            //     // This doesn't include archived projects
            //     projects = await _projectService.GetAllProjectsByCompany(companyId);
            // }
            // return View(projects);
            int companyId = User.Identity!.GetCompanyId().Value;
            ITUser user = await _userManager.GetUserAsync(User);

            List<Project> activeProjects = 
                User.IsInRole(nameof(Roles.Admin)) ? await _projectService.GetAllProjectsByCompany(companyId) : await _projectService.GetUserProjectsAsync(user.Id);

            ProjectListViewModel viewModel = new ProjectListViewModel()
            {
                ActiveOrArchived = "All",
                Projects = activeProjects,
                PageNumber = pageNumber.ToString(),
                PerPage = perPage.ToString(),
                SortBy = sortBy,

                SortByOptions = new SelectList(new string[] { "Project Name", "Project Manager", "Due Date", "Open Tickets" }, sortBy),
                PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
            };

            return View("AllProjects", viewModel);
        }
        #endregion
        
        #region Get Archived Projects
        // GET: ArchivedProjects
        public async Task<IActionResult> ArchivedProjects(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
        {
            int companyId = User.Identity!.GetCompanyId().Value;
            ITUser itUser = await _userManager.GetUserAsync(User);

            List<Project> archivedProjects = 
                User.IsInRole(nameof(Roles.Admin))  ? await _projectService.GetArchivedProjectsByCompany(companyId) : await _projectService.GetUserArchivedProjectsAsync(itUser.Id);

            ProjectListViewModel viewModel = new ProjectListViewModel()
            {
                ActiveOrArchived = "Archived",
                Projects = archivedProjects,
                PageNumber = pageNumber.ToString(),
                PerPage = perPage.ToString(),
                SortBy = sortBy,

                SortByOptions = new SelectList(new string[] { "Project Name", "Project Manager", "Due Date", "Open Tickets" }, sortBy),
                PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
            };

            return View("AllProjects", viewModel);
        }
        #endregion
        
        #region Get Unassigned Projects
        // GET: Ticket
        [Authorize(Roles = nameof(Roles.Admin))] 
        public async Task<IActionResult> UnassignedProjects()
        {
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;
            string itUserId = _userManager.GetUserId(User);
            
            List<Project> projects = await _projectService.GetUnassignedProjectsAsync(companyId);

            return View(projects);
        }
        #endregion
        
        #region Get Assign Project Manager

        [Authorize(Roles = nameof(Roles.Admin))] 
        [HttpGet]
        public async Task<IActionResult> AssignProjectManager(int ProjectId)
        {
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;
            
            AssignProjectManagerViewModel model = new();
            
            model.Project = await _projectService.GetProjectByIdAsync(ProjectId, companyId);
            model.ProjectManagerList = new SelectList( await _rolesService.GetManyUsersInRoleAsync(nameof(Roles.ProjectManager), companyId), "Id", "FullName" );

            return View(model);
        }
        #endregion
        
        #region Post Assign Project Manager

        [Authorize(Roles = nameof(Roles.Admin))] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignProjectManager(AssignProjectManagerViewModel model)
        {
            if ( !string.IsNullOrEmpty(model.ProjectManagerId) )
            {
                await _projectService.AddProjectManagerAsync(model.ProjectManagerId, model.Project.Id);

                return RedirectToAction(nameof(Details), new { id = model.Project.Id });
            }

            return RedirectToAction(nameof(AssignProjectManager), new { projectId = model.Project.Id });
        }
        #endregion
        
        #region Get Assign Members

        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpGet]
        public async Task<IActionResult> AssignMembers(int ProjectId)
        {
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;
            
            ProjectMembersViewModel model = new();
            
            model.Project = await _projectService.GetProjectByIdAsync(ProjectId, companyId);

            List<ITUser> developers = await _rolesService.GetManyUsersInRoleAsync(nameof(Roles.Developer), companyId); 
            List<ITUser> submitters = await _rolesService.GetManyUsersInRoleAsync(nameof(Roles.Submitter), companyId);

            List<ITUser> companyMembers = developers.Concat(submitters).ToList();

            List<string> projectMembers = model.Project.Members.Select( m => m.Id ).ToList();
            
            model.Users = new MultiSelectList( companyMembers, "Id", "FullName", projectMembers );

            return View(model);
        }
        #endregion
        
        #region Post Assign Members

        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMembers(ProjectMembersViewModel model)
        {
            if ( model.SelectedUsers != null )
            {
                List<string> memberIds = (await _projectService.GetAllProjectMembersExceptPMAsync(model.Project.Id))
                    .Select(m => m.Id).ToList();
                
                //Remove Current Members
                foreach (string member in memberIds)
                {
                    await _projectService.RemoveUserFromProjectAsync(member, model.Project.Id);
                }
                
                // Add selected members
                foreach (string member in model.SelectedUsers)
                {
                    await _projectService.AddUserToProjectAsync(member, model.Project.Id);
                }

                return RedirectToAction(nameof(Details), new { id = model.Project.Id });
            }

            return RedirectToAction(nameof(AssignMembers), new { projectId = model.Project.Id });
        }
        #endregion
        
        #region Get Details
        // GET: Project/Details/5
        public async Task<IActionResult> Details(int? projectId)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            
            ITUser itUser = await _userManager.GetUserAsync(User);

            Project? project = await _projectService.GetProjectByIdAsync(projectId.Value, companyId);

            if (project is null)
                return View(nameof(NotFound));

            if (!User.IsInRole(nameof(Roles.Admin)) && !project.Members.Contains(itUser))
                return View(nameof(NotAuthorized));

            ProjectViewModel viewModel = new ProjectViewModel
            {
                Project = project,
                ProjectManager = await _projectService.GetProjectManagerAsync(project.Id),
                Developers = await _projectService.GetDevelopersOnProjectAsync(project.Id),
                Submitters = await _projectService.GetSubmittersOnProjectAsync(project.Id),
                Tickets = project.Tickets,
            };

            return View(viewModel);
        }
        #endregion
        
        #region Get Create
        // GET: Project/Create
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        public async Task<IActionResult> Create()
        {
            ViewData["Action"] = "Create";

            CreateOrEditProjectViewModel viewModel = await GenerateCreateProjectViewModel();

            return View("CreateOrEditProject", viewModel);
        }
        #endregion
        
        #region Post Create
        // POST: Project/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( CreateOrEditProjectViewModel viewModel)
        {
            ViewData["Action"] = "Create";

            Project project = viewModel.Project;

            foreach (string key in ModelState.Keys)
                if (key != "Image") ModelState.Remove(key);

            if (viewModel.Name is null)
                ModelState.AddModelError(string.Empty, "Project name cannot be empty");

            if (viewModel.Description is null)
                ModelState.AddModelError(string.Empty, "Project description cannot be empty");

            if (viewModel.EndDate < DateTime.Now)
                ModelState.AddModelError(string.Empty, "Project due date cannot be in the past");

            if (ModelState["Image"] is not null && ModelState["Image"]!.ValidationState == ModelValidationState.Invalid)
                ModelState.AddModelError(string.Empty, "Image is too large, or the wrong extension type");

            if (ModelState.ErrorCount > 0)
            {
                CreateOrEditProjectViewModel newViewModel = await GenerateCreateProjectViewModel();
                newViewModel.Name = viewModel.Name ?? string.Empty;
                newViewModel.Description = viewModel.Description ?? string.Empty;
                newViewModel.EndDate = viewModel.EndDate;
                newViewModel.SelectedManager = viewModel.SelectedManager;
                newViewModel.SelectedDevelopers = viewModel.SelectedDevelopers;
                newViewModel.SelectedSubmitters = viewModel.SelectedSubmitters;
                newViewModel.Image = viewModel.Image;
                return View("CreateOrEditProject", newViewModel);
            }

            project.Name = viewModel.Name!;
            project.Description = viewModel.Description!;
            project.EndDate = viewModel.EndDate;
            await UpdateProjectImageAsync(viewModel, project);

            project.Id = await _projectService.AddNewProjectAsync(project);

            await AssignTeamToProjectAsync(viewModel, project);

            return RedirectToAction(nameof(Details), new { projectId = project.Id });
        }
        #endregion
        
        #region Get Edit
        // GET: Project/Edit/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        public async Task<IActionResult> Edit(int? projectId)
        {
            ViewData["Action"] = "Edit";
            
            int companyId = User.Identity.GetCompanyId().Value;
            
            Project? project = await _projectService.GetProjectByIdAsync(projectId.Value, companyId);
            ITUser user = await _userManager.GetUserAsync(User);

            if (project == null)
                return View(nameof(NotFound));

            ITUser projectManager = await _projectService.GetProjectManagerAsync(projectId.Value);

            if (User.IsInRole(nameof(Roles.Admin)) || projectManager.Id == user.Id)
            {
                CreateOrEditProjectViewModel viewModel = await GenerateEditProjectViewModel(project);

                return View("CreateOrEditProject", viewModel);
            }
            
            return View(nameof(NotAuthorized));

        }
        #endregion
        
        #region Post Edit
        // POST: Project/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateOrEditProjectViewModel viewModel)
        {
                ViewData["Action"] = "Edit";

                Project? project = viewModel.Project;

                foreach (string key in ModelState.Keys)
                    if (key != "Image") ModelState.Remove(key);

                if (viewModel.Name == null)
                    ModelState.AddModelError(string.Empty, "Project name cannot be empty");

                if (viewModel.Description == null)
                    ModelState.AddModelError(string.Empty, "Project description cannot be empty");

                if (viewModel.EndDate < DateTime.Now)
                    ModelState.AddModelError(string.Empty, "Project due date cannot be in the past");

                if (ModelState["Image"] is not null && ModelState["Image"]!.ValidationState == ModelValidationState.Invalid)
                    ModelState.AddModelError(string.Empty, "Image is too large, or the wrong extension type");

                if (ModelState.ErrorCount > 0)
                {
                    CreateOrEditProjectViewModel newViewModel = await GenerateEditProjectViewModel(project);
                    newViewModel.Name = viewModel.Name ?? string.Empty;
                    newViewModel.Description = viewModel.Description ?? string.Empty;
                    newViewModel.EndDate = viewModel.EndDate;
                    newViewModel.SelectedManager = viewModel.SelectedManager;
                    newViewModel.SelectedDevelopers = viewModel.SelectedDevelopers;
                    newViewModel.SelectedSubmitters = viewModel.SelectedSubmitters;
                    newViewModel.Image = viewModel.Image;
                    return View("CreateOrEditProject", newViewModel);
                }

                project.Name = viewModel.Name!;
                project.Description = viewModel.Description!;
                project.EndDate = viewModel.EndDate;
                await UpdateProjectImageAsync(viewModel, project);

                await _projectService.UpdateProjectAsync(project);
                await AssignTeamToProjectAsync(viewModel, project);

                return RedirectToAction(nameof(Details), new { projectId = project.Id });
        }
        #endregion
        
        #region Get Archive
        // GET: Project/Archive/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            
            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
        #endregion
        
        #region Post Archive
        // POST: Project/Archive/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            
            Project project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(AllProjects));
        }
        #endregion
        
        #region Get Restore
        // GET: Project/Restore/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            
            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
        #endregion
        
        #region Post Restore
        // POST: Project/Restore/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {

            int companyId = User.Identity.GetCompanyId().Value;
            
            Project project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction(nameof(AllProjects));
        }
        #endregion
        
        #region Get Delete
        [HttpGet]
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
        public async Task<ViewResult> Delete(int projectId)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            
            Project? project = await _projectService.GetProjectByIdAsync(projectId, companyId);

            if (project == null)
                return View(nameof(NotFound));

            ITUser user = await _userManager.GetUserAsync(User);
            

            if ( await _projectService.isAssignedProjectManagerAsync(user.Id, projectId) || User.IsInRole(nameof(Roles.Admin))) return View(project);
                
            return View( "NotAuthorized" );
            
        }
        #endregion

        #region Post Delete
        [HttpPost]
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToActionResult> DeleteConfirmed(Project project)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            
            ITUser user = await _userManager.GetUserAsync(User);

            if (await _projectService.isAssignedProjectManagerAsync( user.Id, project.Id) || User.IsInRole(nameof(Roles.Admin)) )
            {
                await _projectService.DeleteProjectAsync(companyId, project.Id);
                return RedirectToAction(nameof(AllProjects));
            }
            return RedirectToAction("NotAuthorized");
            
        }
        #endregion
        
        [HttpGet]
        public ViewResult NotAuthorized()
        {
            return View();
        }
        
        #region Does Project Exist
        private async Task<bool> ProjectExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            
            return (await _projectService.GetAllProjectsByCompany(companyId)).Any(p => p.Id == id);
        }
        #endregion
        
        	#region Private Helper Methods
	private async Task<CreateOrEditProjectViewModel> GenerateCreateProjectViewModel()
    {
        Project createdProject = new Project
        {
            CompanyId = User.Identity!.GetCompanyId(),
            Name = String.Empty,
            Description = String.Empty,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30),
        };

        return new CreateOrEditProjectViewModel()
        {
            Project = createdProject,

            EndDate = createdProject.EndDate,
            Name = createdProject.Name,
            Description = createdProject.Description,

            SelectedManager = "Unassigned",
            ProjectManagers = new SelectList(await _companyInfoService.GetAllProjectManagersAsync(createdProject.CompanyId.Value), "Id", "FullName", "Unassigned"),

            SelectedDevelopers = new string[] { },
            Developers = new MultiSelectList(await _companyInfoService.GetAllDevelopersAsync(createdProject.CompanyId.Value), "Id", "FullName", new string[] { }),

            SelectedSubmitters = new string[] { },
            Submitters = new MultiSelectList(await _companyInfoService.GetAllSubmittersAsync(createdProject.CompanyId.Value), "Id", "FullName", new string[] { }),
        };
    }

    private async Task<CreateOrEditProjectViewModel> GenerateEditProjectViewModel(Project project)
    {
        ITUser projectManager = await _projectService.GetProjectManagerAsync(project.Id);
        string currentProjectManager = projectManager != null ? projectManager.Id : "Unassigned";
        List<ITUser> developers = await _projectService.GetDevelopersOnProjectAsync(project.Id);
        List<ITUser> submitters = await _projectService.GetSubmittersOnProjectAsync(project.Id);

        List<string> developerIds = new List<string>();
        List<string> submitterIds = new List<string>();

        foreach (ITUser developer in developers)
            developerIds.Add(developer.Id);

        foreach (ITUser submitter in submitters)
            submitterIds.Add(submitter.Id);

        return new CreateOrEditProjectViewModel()
        {
            Project = project,
            EndDate = project.EndDate,
            Name = project.Name,
            Description = project.Description,

            SelectedManager = currentProjectManager,
            ProjectManagers = new SelectList(await _companyInfoService.GetAllProjectManagersAsync(project.CompanyId.Value), "Id", "FullName", currentProjectManager),

            SelectedDevelopers = developerIds,
            Developers = new MultiSelectList(await _companyInfoService.GetAllDevelopersAsync(project.CompanyId.Value), "Id", "FullName", developerIds),

            SelectedSubmitters = submitterIds,
            Submitters = new MultiSelectList(await _companyInfoService.GetAllSubmittersAsync(project.CompanyId.Value), "Id", "FullName", submitterIds)
        };
    }

    private async Task AssignTeamToProjectAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
        await AssignProjectManagerAsync(viewModel, project);
        await AssignDevelopersAsync(viewModel, project);
        await AssignSubmittersAsync(viewModel, project);
    }

    private async Task AssignProjectManagerAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
		ITUser selectedProjectManager = await _userManager.FindByIdAsync(viewModel.SelectedManager);
        ITUser projectManager = await _projectService.GetProjectManagerAsync(project.Id);

        if (selectedProjectManager != null && selectedProjectManager.Id != projectManager.Id)
			await _notificationService.CreateNewProjectNotificationAsync(selectedProjectManager.Id, project);

		if (projectManager.Id != null)
			await _projectService.RemoveProjectManagerAsync(project.Id);

        if (selectedProjectManager != null)
            await _projectService.AddProjectManagerAsync(selectedProjectManager.Id, project.Id);
	}

    private async Task AssignDevelopersAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
        if (viewModel.SelectedDevelopers != null)
        {
            foreach (string memberId in viewModel.SelectedDevelopers)
            {
                ITUser member = await _userManager.FindByIdAsync(memberId);

                if (!project.Members.Contains(member))
                    await _notificationService.CreateNewProjectNotificationAsync(memberId, project);
            }
        }

        foreach (ITUser projectMember in project.Members)
        {
            if (false == await _userManager.IsInRoleAsync(projectMember, nameof(Roles.Developer)))
                continue;

            if (viewModel.SelectedDevelopers is null || !viewModel.SelectedDevelopers.Contains(projectMember.Id))
            {
                await _projectService.RemoveUserFromProjectAsync(projectMember.Id, project.Id);
            }
        }

        if (viewModel.SelectedDevelopers is null)
            return;

		foreach (string employeeId in viewModel.SelectedDevelopers)
		{
            if (!project.Members.Where(projectMember => projectMember.Id == employeeId).Any())
            {
                await _projectService.AddUserToProjectAsync(employeeId, project.Id);
            }
		}
	}

    private async Task AssignSubmittersAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
        if (viewModel.SelectedSubmitters is not null)
        {
            foreach (string memberId in viewModel.SelectedSubmitters)
            {
                ITUser member = await _userManager.FindByIdAsync(memberId);

                if (!project.Members.Contains(member))
                    await _notificationService.CreateNewProjectNotificationAsync(memberId, project);
            }
        }

        foreach (ITUser projectMember in project.Members)
        {
            if (false == await _userManager.IsInRoleAsync(projectMember, nameof(Roles.Submitter)))
                continue;

            if (viewModel.SelectedSubmitters is null || !viewModel.SelectedSubmitters.Contains(projectMember.Id))
            {
                await _projectService.RemoveUserFromProjectAsync(projectMember.Id, project.Id);
            }
        }

        if (viewModel.SelectedSubmitters is null)
            return;

        foreach (string memberId in viewModel.SelectedSubmitters)
        {
            if (!project.Members.Where(projectMember => projectMember.Id == memberId).Any())
            {
                await _projectService.AddUserToProjectAsync(memberId, project.Id);
            }
        }
    }

    private async Task UpdateProjectImageAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
		if (viewModel.Image != null)
		{
			byte[] imageData = await _fileService.ConvertFileToByteArrayAsync(viewModel.Image);
			string contentType = viewModel.Image.ContentType;

			project.FileContentType = contentType;
			project.FileData = imageData;
		}
	}

    #endregion
    }
}
