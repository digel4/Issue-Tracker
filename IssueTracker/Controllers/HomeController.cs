using System.Diagnostics;
using IssueTracker.Extensions;
using Microsoft.AspNetCore.Mvc;
using IssueTracker.Models;
using IssueTracker.Models.ChartModels;
using IssueTracker.Models.ViewModels;
using IssueTracker.Services.Interfaces;
using IssueTracker.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Controllers;

public class HomeController : Controller
{
    #region Properties
    private readonly ILogger<HomeController> _logger;
    private readonly ICompanyInfoService _companyInfoService;
    private readonly IProjectService _projectService;
    private readonly ITicketService _ticketService;
    private readonly INotificationService _notificationService;
    private readonly SignInManager<ITUser> _signInManager;
    private readonly UserManager<ITUser> _userManager;

    #endregion

    #region Constructor
    public HomeController(
        ICompanyInfoService companyInfoService, 
        IProjectService projectService, 
        ITicketService ticketService,
        INotificationService notificationService,
        SignInManager<ITUser> signInManager,
        UserManager<ITUser> userManager
        )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _companyInfoService = companyInfoService;
        _projectService = projectService;
        _ticketService = ticketService;
        _notificationService = notificationService;
    }
    #endregion 
    
    #region Index
    public async Task<IActionResult> Index()
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("/Identity/Account/Login");

        DashboardViewModel dashboardViewModel;

        if (User.IsInRole(nameof(Roles.Admin)))
            dashboardViewModel = await GenerateAdminDashboard();

        else
            dashboardViewModel = await GenerateNonAdminDashboard();

        return View(dashboardViewModel);
    }
    #endregion

    private async Task<DashboardViewModel> GenerateAdminDashboard()
    {
        int companyId = User.Identity!.GetCompanyId().Value;

        List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);
        List<Ticket> openTickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);
        List<Ticket> completedTickets = await _ticketService.GetArchivedTicketsAsync(companyId);
        List<ITUser> employees = await _companyInfoService.GetAllMembersAsync(companyId);

        int adminCount = (await _companyInfoService.GetAllAdminsAsync(companyId)).Count();
        int projectManagerCount = (await _companyInfoService.GetAllProjectManagersAsync(companyId)).Count();
        int developerCount = (await _companyInfoService.GetAllDevelopersAsync(companyId)).Count();
        int memberCount = (await _companyInfoService.GetAllMembersAsync(companyId)).Count();

        return new DashboardViewModel()
        {
            ActiveProjects = projects,
            OpenTickets = openTickets,
            CompletedTickets = completedTickets,
            Members = employees,
            AdminCount = adminCount,
            ProjectManagerCount = projectManagerCount,
            DeveloperCount = developerCount,
            MemberCount = memberCount,
        };
    }

    private async Task<DashboardViewModel> GenerateNonAdminDashboard()
    {
        ITUser user = await _userManager.GetUserAsync(User);

        int companyId = User.Identity!.GetCompanyId().Value;

        List<Project> projects = await _projectService.GetUserProjectsAsync(user.Id);
        List<Ticket> tickets = await _ticketService.GetTicketsByUserIdAsync(user.Id, companyId);
        List<Ticket> completedTickets = await _ticketService.GetArchivedTicketsAsync(companyId);
        List<ITUser> Members = await _companyInfoService.GetAllMembersAsync(companyId);

        List<Notification> notifications = await _notificationService.GetUnseenNotificationsForUserAsync(user);

        return new DashboardViewModel()
        {
            ActiveProjects = projects,
            OpenTickets = tickets,
            CompletedTickets = completedTickets,
            Notifications = notifications,
        };
    }
    
    
    // #region Dashboard
    // public async Task<IActionResult> Dashboard()
    // {
    //     // Add ViewModel instance
    //     DashboardViewModel model = new();
    //
    //     int companyId = User.Identity.GetCompanyId().Value;
    //
    //     model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
    //     model.Projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();
    //     model.Tickets = model.Projects.SelectMany(p => p.Tickets).Where(t => t.Archived == false).ToList();
    //     model.Members = model.Company.Members.ToList();
    //
    //     return View(model);
    // }
    // #endregion
    
    
    // #region Privacy
    // public IActionResult Privacy()
    // {
    //     return View();
    // }
    // #endregion
    //
    // #region Error
    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
    // #endregion
    
        // #region Plotly Bar Chart
    // [HttpPost]
    // public async Task<JsonResult> PlotlyBarChart()
    // {
    //     PlotlyBarData plotlyData = new();
    //     List<PlotlyBar> barData = new();
    //
    //     int companyId = User.Identity.GetCompanyId().Value;
    //
    //     List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);
    //
    //     //Bar One
    //     PlotlyBar barOne = new()
    //     {
    //         X = projects.Select(p => p.Name).ToArray(),
    //         Y = projects.SelectMany(p => p.Tickets).GroupBy(t => t.ProjectId).Select(g => g.Count()).ToArray(),
    //         Name = "Tickets",
    //         Type =  "bar"
    //     };
    //
    //     //Bar Two
    //     PlotlyBar barTwo = new()
    //     {
    //         X = projects.Select(p => p.Name).ToArray(),
    //         Y = projects.Select(async p=> (await _projectService.GetProjectMembersByRoleAsync(p.Id, nameof(Roles.Developer))).Count).Select(c=>c.Result).ToArray(),
    //         Name = "Developers",
    //         Type = "bar"
    //     };
    //
    //     barData.Add(barOne);    
    //     barData.Add(barTwo);
    //
    //     plotlyData.Data = barData;
    //
    //     return Json(plotlyData);
    // }
    // #endregion
    //
    // #region Am Charts
    // [HttpPost]
    // public async Task<JsonResult> AmCharts()
    // {
    //
    //     AmChartData amChartData = new ();
    //     List<AmItem> amItems = new (); 
    //
    //     int companyId = User.Identity.GetCompanyId().Value;
    //
    //     List<Project> projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p=>p.Archived == false).ToList();
    //
    //     foreach(Project project in projects)
    //     {
    //         AmItem item = new();
    //
    //         item.Project = project.Name;
    //         item.Tickets = project.Tickets.Count;
    //         item.Developers = (await _projectService.GetProjectMembersByRoleAsync(project.Id, nameof(Roles.Developer))).Count();  
    //     
    //         amItems.Add(item);  
    //     }
    //
    //     amChartData.Data = amItems.ToArray();   
    //
    //
    //     return Json(amChartData.Data);
    // }
    // #endregion
    //
    // #region Ggl Project Tickets
    // [HttpPost]
    // public async Task<JsonResult> GglProjectTickets()
    // {
    //     int companyId = User.Identity.GetCompanyId().Value;
    //
    //     List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);
    //
    //     List<object> chartData = new();
    //     chartData.Add(new object[] { "ProjectName", "TicketCount" });
    //
    //     foreach (Project prj in projects)
    //     {
    //         chartData.Add(new object[] { prj.Name, prj.Tickets.Count() });
    //     }
    //
    //     return Json(chartData);
    // }
    // #endregion
    //
    // #region Ggl Project Priority
    // [HttpPost]
    // public async Task<JsonResult> GglProjectPriority()
    // {
    //     int companyId = User.Identity.GetCompanyId().Value;
    //
    //     List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);
    //
    //     List<object> chartData = new();
    //     chartData.Add(new object[] { "Priority", "Count" });
    //
    //
    //     foreach (string priority in Enum.GetNames(typeof(ITProjectPriority)))
    //     {
    //         int priorityCount = (await _projectService.GetAllProjectsByPriority(companyId, priority)).Count();
    //         chartData.Add(new object[] { priority, priorityCount });
    //     }
    //
    //     return Json(chartData);
    // }
    // #endregion
}