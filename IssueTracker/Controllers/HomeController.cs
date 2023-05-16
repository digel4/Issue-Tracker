using System.Diagnostics;
using IssueTracker.Extensions;
using Microsoft.AspNetCore.Mvc;
using IssueTracker.Models;
using IssueTracker.Models.ChartModels;
using IssueTracker.Models.ViewModels;
using IssueTracker.Services.Interfaces;
using IssueTracker.Models.Enums;

namespace IssueTracker.Controllers;

public class HomeController : Controller
{
    #region Properties
    private readonly ILogger<HomeController> _logger;
    private readonly IITCompanyInfoService _companyInfoService;
    private readonly IITProjectService _projectService;

    #endregion

    #region Constructor
    public HomeController(ILogger<HomeController> logger, IITCompanyInfoService companyInfoService, IITProjectService projectService)
    {
        _logger = logger;
        _companyInfoService = companyInfoService;
        _projectService = projectService;
    }
    #endregion 
    
    #region Index
    public IActionResult Index()
    {
        return View();
    }
    #endregion

    #region Dashboard
    public async Task<IActionResult> Dashboard()
    {
        // Add ViewModel instance
        DashboardViewModel model = new();

        int companyId = User.Identity.GetCompanyId().Value;

        model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
        model.Projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();
        model.Tickets = model.Projects.SelectMany(p => p.Tickets).Where(t => t.Archived == false).ToList();
        model.Members = model.Company.Members.ToList();

        return View(model);
    }
    #endregion
    
    [HttpPost]
    public async Task<JsonResult> PlotlyBarChart()
    {
        PlotlyBarData plotlyData = new();
        List<PlotlyBar> barData = new();

        int companyId = User.Identity.GetCompanyId().Value;

        List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);

        //Bar One
        PlotlyBar barOne = new()
        {
            X = projects.Select(p => p.Name).ToArray(),
            Y = projects.SelectMany(p => p.Tickets).GroupBy(t => t.ProjectId).Select(g => g.Count()).ToArray(),
            Name = "Tickets",
            Type =  "bar"
        };

        //Bar Two
        PlotlyBar barTwo = new()
        {
            X = projects.Select(p => p.Name).ToArray(),
            Y = projects.Select(async p=> (await _projectService.GetProjectMembersByRoleAsync(p.Id, nameof(Roles.Developer))).Count).Select(c=>c.Result).ToArray(),
            Name = "Developers",
            Type = "bar"
        };

        barData.Add(barOne);    
        barData.Add(barTwo);

        plotlyData.Data = barData;

        return Json(plotlyData);
    }
    
    
    
    [HttpPost]
    public async Task<JsonResult> AmCharts()
    {

        AmChartData amChartData = new ();
        List<AmItem> amItems = new (); 

        int companyId = User.Identity.GetCompanyId().Value;

        List<Project> projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p=>p.Archived == false).ToList();

        foreach(Project project in projects)
        {
            AmItem item = new();

            item.Project = project.Name;
            item.Tickets = project.Tickets.Count;
            item.Developers = (await _projectService.GetProjectMembersByRoleAsync(project.Id, nameof(Roles.Developer))).Count();  
        
            amItems.Add(item);  
        }

        amChartData.Data = amItems.ToArray();   


        return Json(amChartData.Data);
    }
    
    [HttpPost]
    public async Task<JsonResult> GglProjectTickets()
    {
        int companyId = User.Identity.GetCompanyId().Value;

        List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);

        List<object> chartData = new();
        chartData.Add(new object[] { "ProjectName", "TicketCount" });

        foreach (Project prj in projects)
        {
            chartData.Add(new object[] { prj.Name, prj.Tickets.Count() });
        }

        return Json(chartData);
    }
    
    [HttpPost]
    public async Task<JsonResult> GglProjectPriority()
    {
        int companyId = User.Identity.GetCompanyId().Value;

        List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);

        List<object> chartData = new();
        chartData.Add(new object[] { "Priority", "Count" });


        foreach (string priority in Enum.GetNames(typeof(ITProjectPriority)))
        {
            int priorityCount = (await _projectService.GetAllProjectsByPriority(companyId, priority)).Count();
            chartData.Add(new object[] { priority, priorityCount });
        }

        return Json(chartData);
    }

    #region Privacy
    public IActionResult Privacy()
    {
        return View();
    }
    #endregion

    #region Error
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    #endregion
}