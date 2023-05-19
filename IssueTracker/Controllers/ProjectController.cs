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

namespace IssueTracker.Controllers
{
    // Authorise the entire controller with the authorize attribute
    [Authorize]
    public class ProjectController : Controller
    {
        #region Properties
        private readonly IITProjectService _projectService;
        private readonly IITRolesService _rolesService;
        private readonly IITLookUpService _lookUpService;
        private readonly IITFileService _fileService;
        private readonly UserManager<ITUser> _userManager;
        private readonly IITCompanyInfoService _companyInfoService;
        #endregion

        #region Constructor
        public ProjectController(IITProjectService IITProjectService, IITRolesService IITRolesService, IITLookUpService IITLookUpService, IITFileService IITFileService, UserManager<ITUser> userManager, IITCompanyInfoService IITCompanyInfoService)
        {
            _projectService = IITProjectService;
            _rolesService = IITRolesService;
            _lookUpService = IITLookUpService;
            _fileService = IITFileService;
            _userManager = userManager;
            _companyInfoService = IITCompanyInfoService;
        }
        #endregion

        #region Get My Projects
        // GET: MyProjects
        public async Task<IActionResult> MyProjects()
        {
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            string userId = _userManager.GetUserId(User);
            
            List<Project> projects = await _projectService.GetUserProjectsAsync(userId);

            return View(projects);
        }
        #endregion
        
        #region Get My Archived Projects
        // GET: MyProjects
        public async Task<IActionResult> MyArchivedProjects()
        {
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            string userId = _userManager.GetUserId(User);
            
            List<Project> projects = await _projectService.GetUserArchivedProjectsAsync(userId);

            return View(projects);
        }
        #endregion
        
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
            int companyId = User.Identity.GetCompanyId().Value;
            
            // Add ViewModel instance
            AddProjectWithPMViewModel model = new();
            
            // Load SelectLists with data ie. PMList & PriorityList
            model.PMList =
                new SelectList( await _rolesService.GetManyUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName" );
            model.PriorityList = new SelectList( await _lookUpService.GetProjectPrioritiesAsync(), "Id", "Name" );
            
            // Another way it could be done with a "viewBag"
            // ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id");
            return View(model);
        }
        #endregion
        
        #region Post Create
        // POST: Project/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                //Returns a nullable value so add .Value
                int companyId = User.Identity.GetCompanyId().Value;
                try
                {
                    if (model.Project.FormFile != null)
                    {
                        model.Project.FileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.FormFile);
                        model.Project.FileName = model.Project.FormFile.FileName;
                        model.Project.FileContentType = model.Project.FormFile.ContentType;
                        
                        
                    }

                    model.Project.CompanyId = companyId;
                    
                    await _projectService.AddNewProjectAsync(model.Project);
                    
                    // Add PM if one was chosen
                    if (!string.IsNullOrEmpty(model.PMId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMId, model.Project.Id);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"****ERROR**** - Error post create on ProjectController. --->  {e.Message}");
                    throw;
                }
                return RedirectToAction("MyProjects");
            }
            return RedirectToAction("Create");
        }
        #endregion
        
        #region Get Edit
        // GET: Project/Edit/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        public async Task<IActionResult> Edit(int? id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            
            // Add ViewModel instance
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            
            // Get current PM
            ITUser currentPM = await _projectService.GetProjectManagerAsync(id.Value);
            
            // Load SelectLists with data ie. PMList & PriorityList
            if (currentPM != null)
            {
                model.PMList =
                    new SelectList( await _rolesService.GetManyUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName", currentPM.Id);
            }
            else
            {
                model.PMList =
                    new SelectList( await _rolesService.GetManyUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            }

            model.PriorityList = new SelectList( await _lookUpService.GetProjectPrioritiesAsync(), "Id", "Name" );
            
            // Another way it could be done with a "viewBag"
            // ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id");
            return View(model);
        }
        #endregion
        
        #region Post Edit
        // POST: Project/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                try
                {
                    if (model.Project.FormFile != null)
                    {
                        model.Project.FileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.FormFile);
                        model.Project.FileName = model.Project.FormFile.FileName;
                        model.Project.FileContentType = model.Project.FormFile.ContentType;
                    }
                    await _projectService.UpdateProjectAsync(model.Project);
                    
                    // Add PM if one was chosen
                    if (!string.IsNullOrEmpty(model.PMId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMId, model.Project.Id);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjectExists(model.Project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MyProjects");
            }
            return RedirectToAction("MyProjects");
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

            return RedirectToAction(nameof(MyProjects));
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

            return RedirectToAction(nameof(MyProjects));
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
    }
}
