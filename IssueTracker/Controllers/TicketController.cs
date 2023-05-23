using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using ASP;
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
    [Authorize]
    public class TicketController : Controller
    {
        #region Properties
        private readonly UserManager<ITUser> _userManager;
        private readonly IProjectService _projectService;
        private readonly IITLookUpService _lookUpService;
        private readonly ITicketService _ticketService;
        private readonly IFileService _fileService;
        private readonly ITicketHistoryService _historyService;
        private readonly IRolesService _rolesService;
        private readonly INotificationService _notificationService;
        private readonly ITicketHistoryService _ticketHistoryService;

        #endregion
        
        #region Contructor
        public TicketController( 
            UserManager<ITUser> userManager, 
            IProjectService projectService, 
            IITLookUpService lookUpService, 
            ITicketService ticketService, 
            IFileService fileService, 
            ITicketHistoryService historyService,
            IRolesService rolesService,
            INotificationService notificationService,
            ITicketHistoryService ticketHistoryService
            )
        {
            _projectService = projectService;
            _userManager = userManager;
            _rolesService = rolesService;
            _lookUpService = lookUpService;
            _ticketService = ticketService;
            _fileService = fileService;
            _historyService = historyService;
            _notificationService = notificationService;
            _ticketHistoryService = ticketHistoryService;
        }
        #endregion
        
        #region Get My Tickets
        // GET: Ticket
        public async Task<IActionResult> MyTickets()
        {
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            // We can also get the companyId and User id with 
            // int ITUser itUser = await _userManager.GetUserAsync(User)       itUser.Id itUser.CompanyId
            string userId = _userManager.GetUserId(User);
            int companyId = User.Identity.GetCompanyId().Value;

            List<Ticket> tickets = await _ticketService.GetTicketsByUserIdAsync(userId, companyId);

            return View(tickets);
        }
        #endregion
        
        // #region Get All Tickets
        // // GET: Ticket
        // public async Task<IActionResult> AllTickets()
        // {
        //     // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
        //     int companyId = User.Identity.GetCompanyId().Value;
        //
        //     List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);
        //
        //     if ( User.IsInRole(nameof(Roles.Developer))  || User.IsInRole(nameof(Roles.Submitter)) )
        //     {
        //         return View(tickets.Where(t => t.Archived == false).ToList());
        //     }
        //     return View(tickets);
        // }
        // #endregion
        

        #region Get All Open Tickets
        public async Task<IActionResult> AllOpenTickets(string sortBy = "Title", int pageNumber = 1, int perPage = 10)
        {
            int companyId = User.Identity!.GetCompanyId().Value;
            ITUser itUser = await _userManager.GetUserAsync(User);


            List<Ticket> activeTickets =
                await _rolesService.IsUserInRoleAsync(itUser, nameof(Roles.Admin) ) ? await _ticketService.GetAllOpenTicketsAsync(companyId) : await _ticketService.GetUserOpenTicketsAsync(itUser.Id, companyId) ;

            TicketListViewModel viewModel = new TicketListViewModel()
            {
                TicketListType = "AllOpen",
                Tickets = activeTickets,
                PageNumber = pageNumber.ToString(),
                PerPage = perPage.ToString(),
                SortBy = sortBy,

                SortByOptions = new SelectList(new string[] { "Title", "Project", "Assigned", "Priority", "Type", "Status" }, sortBy),
                PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
            };

            return View("AllTickets", viewModel);
        }
        #endregion
        
        
        // #region Get My Open Tickets
        // public async Task<IActionResult> MyOpenTickets()
        // {
        // }
        // #endregion
        
        #region Get Completed Tickets
        public async Task<IActionResult> CompletedTickets(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
        {
            ITUser itUser = await _userManager.GetUserAsync(User);
            
            int companyId = User.Identity!.GetCompanyId().Value;
            

            

            ITUser user = await _userManager.GetUserAsync(User);

            List<Ticket> activeTickets =
                await _rolesService.IsUserInRoleAsync(itUser, Roles.Admin.ToString() ) ? await _ticketService.GetAllCompletedTicketsAsync(companyId) : await _ticketService.GetUserCompletedTicketsAsync(itUser.Id, companyId);

            TicketListViewModel viewModel = new TicketListViewModel()
            {
                TicketListType = "Completed",
                Tickets = activeTickets,
                PageNumber = pageNumber.ToString(),
                PerPage = perPage.ToString(),
                SortBy = sortBy,

                SortByOptions = new SelectList(new string[] { "Project Name", "Assigned", "Priority", "Type", "Status" }, sortBy),
                PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
            };

            return View("AllTickets", viewModel);
            
            
        }
        #endregion
        
        
        #region Get Action Required Tickets
        public async Task<IActionResult> ActionRequiredTickets(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
        {
            ITUser itUser = await _userManager.GetUserAsync(User);
            
            int companyId = User.Identity!.GetCompanyId().Value;


            
            List<Ticket> actionRequiredTickets =
                await _rolesService.IsUserInRoleAsync(itUser, Roles.Admin.ToString() ) ? await _ticketService.GetAllActionRequiredTicketsAsync(companyId) : await _ticketService.GetUserActionRequiredTicketsAsync(itUser.Id, companyId);

            TicketListViewModel viewModel = new TicketListViewModel()
            {
                TicketListType = "ActionRequired",
                Tickets = actionRequiredTickets,
                PageNumber = pageNumber.ToString(),
                PerPage = perPage.ToString(),
                SortBy = sortBy,

                SortByOptions = new SelectList(new string[] { "Project Name", "Assigned", "Priority", "Type", "Status" }, sortBy),
                PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
            };

            return View("AllTickets", viewModel);
        }
        #endregion
        
        #region Get Archived Tickets
        // GET: Ticket
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        public async Task<IActionResult> ArchivedTickets()
        {
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;

            List<Ticket> tickets = await _ticketService.GetArchivedTicketsAsync(companyId);

            return View(tickets);
        }
        #endregion
        
        // #region Get Unassigned Tickets
        // // GET: Ticket
        // [Authorize(Roles="Admin, ProjectManager")]
        // public async Task<IActionResult> UnassignedTickets()
        // {
        //     // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
        //     int companyId = User.Identity.GetCompanyId().Value;
        //     string itUserId = _userManager.GetUserId(User);
        //     
        //     List<Ticket> tickets = await _ticketService.GetUnassignedTicketsAsync(companyId);
        //     
        //     if ( User.IsInRole(nameof(Roles.Admin)) )
        //     {
        //         return View(tickets);
        //     }
        //     else
        //     {
        //         List<Ticket> pmTickets = new();
        //
        //         foreach (Ticket ticket in tickets)
        //         {
        //             if (await _projectService.isAssignedProjectManagerAsync(itUserId, ticket.ProjectId))
        //             {
        //                 pmTickets.Add(ticket);
        //             }
        //         }
        //         return View(pmTickets);
        //     }
        // }
        // #endregion
        
        // #region Get Assign Developer
        //
        // [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        // [HttpGet]
        // public async Task<IActionResult> AssignDeveloper(int id)
        // {
        //     // Add ViewModel instance
        //     AssignDeveloperViewModel model = new();
        //
        //     model.Ticket = await _ticketService.GetTicketByIdAsync(id);
        //     // Load SelectLists with data
        //     model.Developers =
        //         new SelectList( await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, nameof(Roles.Developer)), "Id", "FullName" );
        //
        //     return View(model);
        // }
        // #endregion
        //
        // #region Post Assign Developer
        //
        // [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        // {
        //     if ( model.DeveloperId != null )
        //     {
        //         ITUser itUser = await _userManager.GetUserAsync(User);
        //         Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
        //         try
        //         {
        //             await _ticketService.AssignTicketAsync( model.Ticket.Id, model.DeveloperId );
        //             
        //         }
        //         catch (Exception e)
        //         {
        //             Console.WriteLine(e);
        //             throw;
        //         }
        //         
        //         Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
        //         
        //         await _historyService.AddHistoryAsync(oldTicket, newTicket, itUser.Id);
        //         
        //         return RedirectToAction(nameof(Details), new { id = model.Ticket.Id });
        //     }
        //
        //     return RedirectToAction(nameof(AssignDeveloper), new { id = model.Ticket.Id });
        //
        // }
        // #endregion
        
        #region Details
        // GET: Ticket/Details/5
        public async Task<IActionResult> Details(int? ticketId)
        {
            if (ticketId == null)
            {
                return View(nameof(NotFound));
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(ticketId.Value);
            
            if (ticket == null)
            {
                return View(nameof(NotFound));
            }
            int companyId = User.Identity!.GetCompanyId().Value;
            
            Project? project = await _projectService.GetProjectByIdAsync(ticket.ProjectId, companyId);
            
            ITUser? itUser = await _userManager.GetUserAsync(User);

            if (project == null || itUser == null)
            {
                return View(nameof(NotFound));
            }

            //ITUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
            bool isAdmin = await _rolesService.IsUserInRoleAsync(itUser, Roles.Admin.ToString());

            if (isAdmin == false && !project.Members.Contains(itUser))
            {
                return View("NotAuthorized");
            }
            
            // if (!UserIsAdmin && project.ProjectManagerId != user.Id && !project.Team.Contains(user))
            //     return View(nameof(NotAuthorized));

            TicketViewModel viewModel = new TicketViewModel
            {
                Ticket = ticket,
                Project = ticket.Project!,
                ProjectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId),
                Developer = ticket.DeveloperUser
            };

            return View(viewModel);
            
            
            
            
        }
        #endregion
        
        #region Get Create
        // GET: Ticket/Create
        public async Task<IActionResult> Create(int projectId)
        {
            ViewData["Action"] = "Create";
            
            ITUser itUser = await _userManager.GetUserAsync(User);

            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;
            
            Project? project = await _projectService.GetProjectByIdAsync(projectId, companyId);
            
            if (project == null || itUser == null) return View(nameof(NotFound));
            
            if (!await _rolesService.IsUserInRoleAsync(itUser, nameof(Roles.Admin) ) && !project.Members.Contains(itUser))
                return View(nameof(NotAuthorized));
            
            CreateOrEditTicketViewModel viewModel = await GenerateCreateTicketViewModel(projectId);

            return View("CreateOrEditTicket", viewModel);

            // if (User.IsInRole(nameof(Roles.Admin)))
            // {
            //     ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(companyId), "Id", "Name");
            // }
            // else
            // {
            //     ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(itUser.Id), "Id", "Name");
            // }
            //
            //
            // ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Id", "Name");
            // ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Id", "Name");
            //return View();
            
            
            // ViewData["Action"] = "Create";

            // Project? project = await _projectService.GetProjectByIdAsync(projectId);
            // AppUser? user = await _userManager.GetUserAsync(User);

            // if (project is null || user is null)
            //     return View(nameof(NotFound));
            //
            // if (!UserIsAdmin && project.ProjectManagerId != user.Id && !project.Team.Contains(user))
            //     return View(nameof(NotAuthorized));
            //
            // CreateOrEditTicketViewModel viewModel = await GenerateCreateTicketViewModel(projectId);
            //
            // return View("CreateOrEditTicket", viewModel);
            
            
        }
        #endregion
        
        #region Post Create
        // POST: Ticket/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrEditTicketViewModel viewModel)
        {
            // ITUser itUser = await _userManager.GetUserAsync(User);
            //
            // if (ModelState.IsValid)
            // {
            //     try
            //     {
            //         ticket.OwnerUserId = itUser.Id;
            //         ticket.Created = DateTimeOffset.Now;
            //         ticket.TicketStatusId =
            //             (await _ticketService.LookupTicketStatusIdAsync(nameof(ITTicketStatus.New))).Value;
            //     
            //         await _ticketService.AddNewTicketAsync(ticket);
            //
            //         Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
            //         await _historyService.AddHistoryAsync(null, newTicket, itUser.Id);
            //         
            //         //TODO: Ticket Notification
            //     }
            //     catch (Exception e)
            //     {
            //         Console.WriteLine(e);
            //         throw;
            //     }
            //     return RedirectToAction(nameof(MyTickets));
            // }
            //
            // if (User.IsInRole(nameof(Roles.Admin)))
            // {
            //     ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(itUser.CompanyId.Value), "Id", "Name");
            // }
            // else
            // {
            //     ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(itUser.Id), "Id", "Name");
            // }
            // ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Id", "Id");
            // ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Id", "Id");
            //
            // return View(ticket);
            
            
            ViewData["Action"] = "Create";

            Ticket ticket = viewModel.Ticket;
            ticket.Title = viewModel.Title;
            ticket.Description = viewModel.Description;
            ticket.Created = DateTime.Now;
            ticket.DeveloperUserId =  viewModel.SelectedDeveloper;
            
            ticket.TicketTypeId = int.Parse(viewModel.SelectedType) ;
            ticket.TicketPriorityId = int.Parse(viewModel.SelectedPriority);
            ticket.TicketStatusId = ticket.DeveloperUserId != null ? 1 : 5;
            
            // ticket.TicketType.Name = viewModel.SelectedType;
            // ticket.TicketPriority.Name = viewModel.SelectedPriority;
            // ticket.TicketStatus.Name = ticket.DeveloperUserId != null ? "New" : "Unassigned";

            ticket.Id = await _ticketService.AddNewTicketAsync(ticket);

            await _ticketHistoryService.AddTicketCreatedEventAsync(ticket.Id);

            if (ticket.DeveloperUserId != null)
                await _notificationService.CreateNewTicketNotificationAsync(ticket.DeveloperUserId, ticket);

            return RedirectToAction("Details", new { ticketId = ticket.Id });
        }
        #endregion
        
        #region Get Edit
        // GET: Ticket/Edit/5
        public async Task<IActionResult> Edit(int? ticketId)
        {
            // if (id == null)
            // {
            //     return NotFound();
            // }
            //
            // Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            // if (ticket == null)
            // {
            //     return NotFound();
            // }
            // ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            // ViewData["TicketStatusId"] = new SelectList(await _lookUpService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            // ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);
            // return View(ticket);
            
            ViewData["Action"] = "Edit";

            Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId.Value);

            if (ticket == null) return View(nameof(NotFound));
            
            int companyId = User.Identity.GetCompanyId().Value;
            Project? project = await _projectService.GetProjectByIdAsync(ticket.ProjectId, companyId);
            ITUser? user = await _userManager.GetUserAsync(User);

            if (project == null || user == null)
                return View(nameof(NotFound));

            if (!User.IsInRole(nameof(Roles.Admin)) && ticket.DeveloperUserId != user.Id)
                return View(nameof(NotAuthorized));

            if (User.IsInRole(nameof(Roles.Developer)) && ticket.DeveloperUserId != user.Id)
                return View(nameof(NotAuthorized));

            CreateOrEditTicketViewModel viewModel = await GenerateEditTicketViewModel(ticket);

            return View("CreateOrEditTicket", viewModel);
        }
        #endregion
        
        #region Post Edit
        // POST: Ticket/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateOrEditTicketViewModel viewModel)
        {
            // if (id != ticket.Id)
            // {
            //     return NotFound();
            // }
            //
            // if (ModelState.IsValid)
            // {
            //     ITUser itUser = await _userManager.GetUserAsync(User);
            //     // No tracking means entity framework doesn't track the changes or stops it from tracking the entity. We're not changing anything with this method. We're using this method for ticket histories so we just want to check the state of the ticket.
            //     Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
            //     
            //     try
            //     {
            //         ticket.Updated = DateTimeOffset.Now;
            //         await _ticketService.UpdateTicketAsync(ticket);
            //     }
            //     
            //     catch (DbUpdateConcurrencyException)
            //     {
            //         if (!await TicketExists(ticket.Id))
            //         {
            //             return NotFound();
            //         }
            //         else
            //         {
            //             throw;
            //         }
            //     }
            //     
            //     Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
            //     await _historyService.AddHistoryAsync(oldTicket, newTicket, itUser.Id);
            //     return RedirectToAction(nameof(MyTickets));
            // }
            //
            // ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            // ViewData["TicketStatusId"] = new SelectList(await _lookUpService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            // ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);
            // return View(ticket);
            
            ViewData["Action"] = "Edit";

            Ticket ticket = viewModel.Ticket;

            bool aPropertyWasChanged = DetermineIfTicketChangesWereMade(viewModel);

            if (aPropertyWasChanged)
            {
                TicketHistory historyItem = await CreateTicketChangeHistory(viewModel);

                ticket.Title = viewModel.Title;
                ticket.Description = viewModel.Description;
                
                // ticket.TicketStatus.Name = viewModel.SelectedStatus!;
                // ticket.TicketPriority.Name = viewModel.SelectedPriority!;
                // ticket.TicketType.Name = viewModel.SelectedType!;
                ticket.TicketTypeId = int.Parse(viewModel.SelectedType) ;
                ticket.TicketPriorityId = int.Parse(viewModel.SelectedPriority);
                ticket.TicketStatusId = ticket.DeveloperUserId != null ? 1 : 5;
                
                ticket.Updated = DateTime.Now;

                if (viewModel.SelectedDeveloper == null)
                {
                    ticket.DeveloperUserId = null;
                    ticket.TicketStatus.Name = "Unassigned";
                }

                if (ticket.DeveloperUserId != viewModel.SelectedDeveloper && viewModel.SelectedDeveloper is not null)
                {
                    ticket.TicketStatus.Name = "Testing";
                    ticket.DeveloperUserId = viewModel.SelectedDeveloper;
                    await _notificationService.CreateNewTicketNotificationAsync(ticket.DeveloperUserId, ticket);
                }

                await _ticketHistoryService.AddTicketHistoryItemAsync(historyItem);
                await _ticketService.UpdateTicketAsync(ticket);
            }
        
            return RedirectToAction("Details", new { ticketId = ticket.Id });
        }
        #endregion

        #region Add Ticket Comment

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment(TicketViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.NewComment))
            {
                TempData["CommentError"] = "Comment cannot be empty.";
                return Redirect(Url.RouteUrl(new { controller = "Ticket", action = "Details", ticketId = viewModel.Ticket.Id }) + "#comments");
            }
            int companyId = User.Identity.GetCompanyId().Value;
            ITUser user = await _userManager.GetUserAsync(User);
            Project project = await _projectService.GetProjectByIdAsync( viewModel.Ticket.ProjectId, companyId );

            TicketComment comment = new TicketComment()
            {
                TicketId = viewModel.Ticket.Id,
                UserId = user.Id,
                Created = DateTime.Now,
                Comment = viewModel.NewComment,
            };

            await _ticketService.AddNewTicketCommentAsync(viewModel.Ticket.Id, comment);

            foreach (ITUser teamMember in project.Members)
            {
                if (comment.Comment.Contains($"@{teamMember.FullName}"))
                    await _notificationService.CreateMentionNotificationAsync(teamMember.Id, user.Id, viewModel.Ticket.Id);
            }

            return Redirect(Url.RouteUrl(new { controller = "Ticket", action = "Details", ticketId = viewModel.Ticket.Id }) + "#comments");
        }

        #endregion
        
        #region Add Ticket Attachment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment(TicketViewModel viewModel)
        {
            {
                ITUser user = await _userManager.GetUserAsync(User);

                if (viewModel.NewAttachment == null)
                    TempData["AttachmentError"] += "Please choose a file to add.\n";

                if (viewModel.FileDescription == null)
                    TempData["AttachmentError"] += "Please provide a brief file description.\n";

                if (ModelState["NewAttachment"] !=  null && ModelState["NewAttachment"]!.ValidationState == ModelValidationState.Invalid)
                    TempData["AttachmentError"] += "The file extension type is not allowed.\n";

                if (TempData["AttachmentError"] == null)
                    return Redirect(Url.RouteUrl(new { controller = "Ticket", action = "Details", ticketId = viewModel.Ticket.Id }) + "#attachments");

                TicketAttachment attachment = new TicketAttachment()
                {
                    TicketId = viewModel.Ticket.Id,
                    UserId = user.Id,
                    Created = DateTime.Now,
                    Description = viewModel.FileDescription,
                    FileData = await _fileService.ConvertFileToByteArrayAsync(viewModel.NewAttachment!),
                    FileName = viewModel.NewAttachment!.FileName,
                    FileContentType = viewModel.NewAttachment!.ContentType,
                };

                await _ticketService.AddTicketAttachmentAsync(viewModel.Ticket.Id, attachment);
                await _ticketHistoryService.AddAttachmentEventAsync(viewModel.Ticket, attachment);
                return Redirect(Url.RouteUrl(new { controller = "Ticket", action = "Details", ticketId = viewModel.Ticket.Id }) + "#attachments");
            }
        }
        #endregion

        #region Show File
        public async Task<IActionResult> ShowFile(int id)
        {
            TicketAttachment ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(id);
            string fileName = ticketAttachment.FileName;
            byte[] fileData = ticketAttachment.FileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }
        #endregion
        
        #region Get Archive
        // GET: Ticket/Archive/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
        #endregion
        
        #region Post Archive
        // POST: Ticket/Archive/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            await _ticketService.ArchiveTicketAsync(ticket);

            return RedirectToAction(nameof(MyTickets));
        }
        #endregion
        
        #region Get Restore
        // GET: Ticket/Restore/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
        #endregion
        
        #region Post Restore
        // POST: Ticket/Restore/5
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            await _ticketService.RestoreTicketAsync(ticket);

            return RedirectToAction(nameof(MyTickets));
        }
        #endregion
        
        #region Get Delete
        [HttpGet]
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
        public async Task<ViewResult> Delete(int ticketId)
        {
            Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId);

            if (ticket == null)
                return View(nameof(NotFound));
            
            int companyId = User.Identity!.GetCompanyId().Value;
            Project project = await _projectService.GetProjectByIdAsync(ticket.ProjectId, companyId );
            ITUser itUser = await _userManager.GetUserAsync(User);
            
            ITUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
            bool isAdmin = await _rolesService.IsUserInRoleAsync(itUser, Roles.Admin.ToString());
            
            if (!isAdmin && projectManager.Id != itUser.Id && ticket.DeveloperUserId != itUser.Id)
                return View(nameof(NotAuthorized));

            return View(ticket);
        }
        #endregion
        
        #region Post Delete
        [HttpPost]
        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Ticket ticket)
        {
            Ticket? ticketToDelete = await _ticketService.GetTicketByIdAsync(ticket.Id);

            if (ticketToDelete is null)
                return View(nameof(NotFound));

            await _ticketService.DeleteTicketAsync(ticketToDelete);
            return RedirectToAction("Details", "Project", new { projectId = ticketToDelete.ProjectId });
        }
        #endregion

        [HttpGet]
        public ViewResult NotAuthorized()
        {
            return View();
        }
        
        #region Private helper methods
        #region Does Ticket Exist
        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
        #endregion
        private async Task<CreateOrEditTicketViewModel> GenerateCreateTicketViewModel(int projectId)
	    {
		    ITUser itUser = await _userManager.GetUserAsync(User);

		    Ticket ticket = new Ticket()
		    {
			    ProjectId = projectId,
			    OwnerUserId = itUser.Id,
		    };

		    List<ITUser> usersAvailableToAssign = await _projectService.GetDevelopersOnProjectAsync(ticket.ProjectId);

             
            
		    if (!usersAvailableToAssign.Contains(itUser))
			    usersAvailableToAssign.Add(itUser); // creator should be able to assign themself the ticket

		    CreateOrEditTicketViewModel viewModel = new CreateOrEditTicketViewModel
		    {
                
			    Ticket = ticket,
			    Developers = new SelectList(usersAvailableToAssign, "Id", "FullName", "Unassigned"   ),
			    Priorities = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Id", "Name", nameof(ITTicketPriority.Medium)  ),
                Types = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Id", "Name", nameof(ITTicketType.GeneralTask) ),
			    Statuses = new SelectList(await _lookUpService.GetTicketStatusesAsync(), "Id", "Name", nameof(ITTicketStatus.New) )
		    };

		    return viewModel;
	    }

	    private async Task<CreateOrEditTicketViewModel> GenerateEditTicketViewModel(Ticket ticket)
	    {
		    ITUser itUser = await _userManager.GetUserAsync(User);

            List<ITUser> usersAvailableToAssign = await _projectService.GetDevelopersOnProjectAsync(ticket.ProjectId);

            if (!usersAvailableToAssign.Contains(itUser))
                usersAvailableToAssign.Add(itUser); // creator/editor should be able to assign themself the ticket

            CreateOrEditTicketViewModel viewModel = new CreateOrEditTicketViewModel()
		    {
			    Ticket = ticket,
			    Title = ticket.Title,
			    Description = ticket.Description,
			    Developers = new SelectList(usersAvailableToAssign, "Id", "FullName", ticket.DeveloperUserId),
			    Priorities = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Name", "Name", ticket.TicketPriority.Name),
			    Types = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Name", "Name", ticket.TicketType.Name),
			    Statuses = new SelectList(await _lookUpService.GetTicketStatusesAsync(), "Name", "Name", ticket.TicketStatus.Name),
			    IsStatusDropdownEnabled = itUser.Id == ticket.DeveloperUserId ? true : false,
            };

		    return viewModel;
	    }

	    private bool DetermineIfTicketChangesWereMade(CreateOrEditTicketViewModel viewModel)
	    {
            Ticket ticket = viewModel.Ticket;

		    return !(ticket.Title == viewModel.Title
			    && ticket.Description == viewModel.Description
			    && ticket.TicketType.Name == viewModel.SelectedType
			    && ticket.TicketPriority.Name == viewModel.SelectedPriority
			    && ticket.TicketStatus.Name == viewModel.SelectedStatus
			    && ticket.DeveloperUserId == viewModel.SelectedDeveloper);
        }

	    private async Task<TicketHistory> CreateTicketChangeHistory(CreateOrEditTicketViewModel viewModel)
	    {
            Ticket ticket = viewModel.Ticket;
            ITUser editingUser = await _userManager.GetUserAsync(User);

            TicketHistory historyItem = new TicketHistory()
            {
                TicketId = ticket.Id,
                UserId = (await _userManager.GetUserAsync(User)).Id,
                Created = DateTime.Now,
                Description = $"{editingUser.FullName} made the following changes:</br><ul>",
            };

            if (ticket.Title != viewModel.Title)
                historyItem.Description += $"<li>Changed the ticket title to <strong>{viewModel.Title}</strong></li>";

            if (ticket.Description != viewModel.Description)
                historyItem.Description += $"<li>Changed the ticket description to:</br>{viewModel.Description}</li>";

            if (ticket.TicketType.Name != viewModel.SelectedType)
                // historyItem.Description += $"<li>Changed the ticket type to <strong>{await _lookUpService.GetTicketTypeDescriptionByIdAsync(viewModel.SelectedType)}</strong></li>";
                historyItem.Description += $"<li>Changed the ticket type to <strong>{  viewModel.SelectedType }</strong></li>";

            if (ticket.TicketPriority.Name != viewModel.SelectedPriority)
                historyItem.Description += $"<li>Changed the ticket priority to <strong>{ viewModel.SelectedPriority }</strong></li>";

            if (ticket.TicketStatus.Name != viewModel.SelectedStatus && ticket.TicketStatus.Name != "Unassigned")
                historyItem.Description += $"<li>Changed the ticket status to <strong>{ viewModel.SelectedStatus }</strong></li>";

            if (ticket.DeveloperUserId != viewModel.SelectedDeveloper)
            {
			    if (viewModel.SelectedDeveloper is null)
				    historyItem.Description += $"<li>Changed the ticket developer to <strong>Unassigned</strong></li>";

			    else
			    {
				    ITUser? newDeveloper = await _userManager.FindByIdAsync(viewModel.SelectedDeveloper);
				    historyItem.Description += $"<li>Changed the ticket developer to <strong>{newDeveloper.FullName ?? "Unassigned"}</strong></li>";
			    }
            }

		    historyItem.Description += "</ul>";

		    return historyItem;
        }

	#endregion
    }
}
