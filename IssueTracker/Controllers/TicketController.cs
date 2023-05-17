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
    [Authorize]
    public class TicketController : Controller
    {
        #region Properties
        private readonly UserManager<ITUser> _userManager;
        private readonly IITProjectService _projectService;
        private readonly IITLookUpService _lookUpService;
        private readonly IITTicketService _ticketService;
        private readonly IITFileService _fileService;
        private readonly IITTicketHistoryService _historyService;
        #endregion
        
        #region Contructor
        public TicketController( UserManager<ITUser> userManager, IITProjectService projectService, IITLookUpService lookUpService, IITTicketService ticketService, IITFileService fileService, IITTicketHistoryService historyService)
        {
            _projectService = projectService;
            _userManager = userManager;
            _lookUpService = lookUpService;
            _ticketService = ticketService;
            _fileService = fileService;
            _historyService = historyService;
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
        
        #region Get All Tickets
        // GET: Ticket
        public async Task<IActionResult> AllTickets()
        {
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;

            List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);

            if ( User.IsInRole(nameof(Roles.Developer))  || User.IsInRole(nameof(Roles.Submitter)) )
            {
                return View(tickets.Where(t => t.Archived == false).ToList());
            }
            return View(tickets);
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
        
        #region Get Unassigned Tickets
        // GET: Ticket
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> UnassignedTickets()
        {
            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;
            string itUserId = _userManager.GetUserId(User);
            
            List<Ticket> tickets = await _ticketService.GetUnassignedTicketsAsync(companyId);
            
            if ( User.IsInRole(nameof(Roles.Admin)) )
            {
                return View(tickets);
            }
            else
            {
                List<Ticket> pmTickets = new();

                foreach (Ticket ticket in tickets)
                {
                    if (await _projectService.isAssignedProjectManagerAsync(itUserId, ticket.ProjectId))
                    {
                        pmTickets.Add(ticket);
                    }
                }
                return View(pmTickets);
            }
        }
        #endregion
        
        #region Get Assign Developer

        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpGet]
        public async Task<IActionResult> AssignDeveloper(int id)
        {
            // Add ViewModel instance
            AssignDeveloperViewModel model = new();

            model.Ticket = await _ticketService.GetTicketByIdAsync(id);
            // Load SelectLists with data
            model.Developers =
                new SelectList( await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, nameof(Roles.Developer)), "Id", "FullName" );

            return View(model);
        }
        #endregion
        
        #region Post Assign Developer

        [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            if ( model.DeveloperId != null )
            {
                ITUser itUser = await _userManager.GetUserAsync(User);
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
                try
                {
                    await _ticketService.AssignTicketAsync( model.Ticket.Id, model.DeveloperId );
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
                
                await _historyService.AddHistoryAsync(oldTicket, newTicket, itUser.Id);
                
                return RedirectToAction(nameof(Details), new { id = model.Ticket.Id });
            }

            return RedirectToAction(nameof(AssignDeveloper), new { id = model.Ticket.Id });

        }
        #endregion
        
        #region Details
        // GET: Ticket/Details/5
        public async Task<IActionResult> Details(int? id)
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
        
        #region Get Create
        // GET: Ticket/Create
        public async Task<IActionResult> Create()
        {
            ITUser itUser = await _userManager.GetUserAsync(User);

            // We haven't craeted the capital U User here but .Net basically creates it for is. It is whichever User is logged in. It's similiar to the custom claim we used for company used elsewhere in the controllers
            int companyId = User.Identity.GetCompanyId().Value;

            if (User.IsInRole(nameof(Roles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(itUser.Id), "Id", "Name");
            }
            

            ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Id", "Name");
            return View();
        }
        #endregion
        
        #region Post Create
        // POST: Ticket/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            ITUser itUser = await _userManager.GetUserAsync(User);
            
            if (ModelState.IsValid)
            {
                try
                {
                    ticket.OwnerUserId = itUser.Id;
                    ticket.Created = DateTimeOffset.Now;
                    ticket.TicketStatusId =
                        (await _ticketService.LookupTicketStatusIdAsync(nameof(ITTicketStatus.New))).Value;
                
                    await _ticketService.AddNewTicketAsync(ticket);

                    Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                    await _historyService.AddHistoryAsync(null, newTicket, itUser.Id);
                    
                    //TODO: Ticket Notification
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                return RedirectToAction(nameof(MyTickets));
            }
            
            if (User.IsInRole(nameof(Roles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(itUser.CompanyId.Value), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(itUser.Id), "Id", "Name");
            }
            ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Id", "Id");
            ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Id", "Id");
            
            return View(ticket);
        }
        #endregion
        
        #region Get Edit
        // GET: Ticket/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookUpService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        #endregion
        
        #region Post Edit
        // POST: Ticket/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                ITUser itUser = await _userManager.GetUserAsync(User);
                // No tracking means entity framework doesn't track the changes or stops it from tracking the entity. We're not changing anything with this method. We're using this method for ticket histories so we just want to check the state of the ticket.
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                
                try
                {
                    ticket.Updated = DateTimeOffset.Now;
                    await _ticketService.UpdateTicketAsync(ticket);
                }
                
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                await _historyService.AddHistoryAsync(oldTicket, newTicket, itUser.Id);
                return RedirectToAction(nameof(MyTickets));
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookUpService.GetTicketPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookUpService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookUpService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        #endregion

        #region Add Ticket Comment

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id,TicketId,Comment")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ticketComment.UserId = _userManager.GetUserId(User);
                    ticketComment.Created = DateTimeOffset.Now;
                    
                    await _ticketService.AddNewTicketCommentAsync(ticketComment);

                    // We use nameof because the AddHistoryAsync expects model as a string. See Method to understand why it's a string
                    await _historyService.AddHistoryAsync(ticketComment.TicketId, nameof(TicketComment), ticketComment.UserId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return RedirectToAction("Details", new { id = ticketComment.TicketId });
        }

        #endregion
        
        #region Add Ticket Attachment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId")] TicketAttachment ticketAttachment)
        {
            string statusMessage;

            if (ModelState.IsValid && ticketAttachment.FormFile != null)
            {
                try
                {
                    ticketAttachment.FileData = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);
                    ticketAttachment.FileName = ticketAttachment.FormFile.FileName;
                    ticketAttachment.FileContentType = ticketAttachment.FormFile.ContentType;

                    ticketAttachment.Created = DateTimeOffset.Now;
                    ticketAttachment.UserId = _userManager.GetUserId(User);

                    await _ticketService.AddTicketAttachmentAsync(ticketAttachment);
                    
                    // We use nameof because the AddHistoryAsync expects model as a string. See Method to understand why it's a string
                    await _historyService.AddHistoryAsync(ticketAttachment.TicketId, nameof(TicketAttachment), ticketAttachment.UserId);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";
            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
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
        
        #region Does Ticket Exist
        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
        #endregion
    }
}
