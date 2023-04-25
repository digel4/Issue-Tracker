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
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Controllers
{
    public class ProjectController : Controller
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly IITProjectService _projectService;
        private readonly IITRolesService _rolesService;
        private readonly IITLookUpService _lookUpService;
        private readonly IITFileService _fileService;
        private readonly UserManager<ITUser> _userManager;
        private readonly IITCompanyInfoService _companyInfoService;
        #endregion

        #region Constructor
        public ProjectController(ApplicationDbContext context, IITProjectService IITProjectService, IITRolesService IITRolesService, IITLookUpService IITLookUpService, IITFileService IITFileService, UserManager<ITUser> userManager, IITCompanyInfoService IITCompanyInfoService)
        {
            _context = context;
            _projectService = IITProjectService;
            _rolesService = IITRolesService;
            _lookUpService = IITLookUpService;
            _fileService = IITFileService;
            _userManager = userManager;
            _companyInfoService = IITCompanyInfoService;
        }
        #endregion
        
        #region Index
        // GET: Project
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projects.Include(p => p.Company).Include(p => p.ProjectPriority);
            return View(await applicationDbContext.ToListAsync());
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
        
        #region Get All Projects
        // GET: AllProjects
        public async Task<IActionResult> AllProjects()
        {

            List<Project> projects = new();
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;

            if ( User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)) )
            {
                projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            }
            else
            {
                // This doesn't include archived projects
                projects = await _projectService.GetAllProjectsByCompany(companyId);
            }
            return View(projects);
        }
        #endregion
        
        #region Get Archived Projects
        // GET: ArchivedProjects
        public async Task<IActionResult> ArchivedProjects()
        {
            // We haven't created the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;
            
            List<Project> projects = await _projectService.GetArchivedProjectsByCompany(companyId);

            return View(projects);
        }
        #endregion
        
        #region Get Details
        // GET: Project/Details/5
        public async Task<IActionResult> Details(int? id)
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
        
        #region Get Create
        // GET: Project/Create
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
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
        }
        #endregion
        
        #region Get Edit
        // GET: Project/Edit/5
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
                catch (Exception e)
                {
                    Console.WriteLine($"****ERROR**** - Error post Edit on ProjectController. --->  {e.Message}");
                    throw;
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        #endregion
        
        #region Get Archive
        // GET: Project/Archive/5
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
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {

            int companyId = User.Identity.GetCompanyId().Value;
            
            Project project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }
        #endregion
        
        #region Get Restore
        // GET: Project/Restore/5
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
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {

            int companyId = User.Identity.GetCompanyId().Value;
            
            Project project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }
        #endregion
        
        #region Does Project Exist
        private bool ProjectExists(int id)
        {
          return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        #endregion
    }
}
