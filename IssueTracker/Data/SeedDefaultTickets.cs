using System.Text;
using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Services.Interfaces;

namespace IssueTracker.Data;

public static class SeedDefaultTickets
{
            public static async Task SeedDefautTicketsAsync(ApplicationDbContext context, IProjectService projectSvc, ICompanyInfoService companyInfoSvc, ITicketService ticketSvc, ITicketHistoryService ticketHistorySvc, int company1Id, int company2Id, SortedList<string, string> asimovMembers, SortedList<string, string> linuxMembers)
        {

            
            //Get project Ids
            Project portfolio = context.Projects.FirstOrDefault(p => p.Name == "Portfolio Website");
            Project blog = context.Projects.FirstOrDefault(p => p.Name == "Blog Web Application");
            Project bugtracker = context.Projects.FirstOrDefault(p => p.Name == "Issue Tracker Web Application");
            Project movie = context.Projects.FirstOrDefault(p => p.Name == "Movie Information Web Application");
            Project addressBook = context.Projects.FirstOrDefault(p => p.Name == "Address Book Web Application");

            //Get ticket type Ids
            int typeNewDev = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.NewDevelopment.ToString()).Id;
            int typeWorkTask = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.WorkTask.ToString()).Id;
            int typeDefect = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.Defect.ToString()).Id;
            int typeEnhancement = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.Enhancement.ToString()).Id;
            int typeChangeRequest = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.ChangeRequest.ToString()).Id;

            //Get ticket priority Ids
            int priorityLow = context.TicketPriorities.FirstOrDefault(p => p.Name == ITTicketPriority.Low.ToString()).Id;
            int priorityMedium = context.TicketPriorities.FirstOrDefault(p => p.Name == ITTicketPriority.Medium.ToString()).Id;
            int priorityHigh = context.TicketPriorities.FirstOrDefault(p => p.Name == ITTicketPriority.High.ToString()).Id;
            int priorityUrgent = context.TicketPriorities.FirstOrDefault(p => p.Name == ITTicketPriority.Urgent.ToString()).Id;

            //Get ticket status Ids
            int statusNew = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.New.ToString()).Id;
            int statusDev = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.Development.ToString()).Id;
            int statusTest = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.Testing.ToString()).Id;
            int statusResolved = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.Resolved.ToString()).Id;
            int statusUnassigned = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.Unassigned.ToString()).Id;


            try
            {
                IList<Ticket> tickets = new List<Ticket>() {
                    
                    #region Portfolio Tickets
                    
                    #region Ticket 1
                    new Ticket()
                    {
                        Title = "Fix Styling to Email Form", 
                        Description = "Fix styling so it fits in the container", 
                        Created = portfolio.StartDate.AddDays(1), 
                        ProjectId = portfolio.Id ,
                        OwnerUserId = asimovMembers["SusanCalvin"],
                        
                        TicketPriorityId = priorityLow, 
                        TicketStatusId = statusUnassigned, 
                        TicketTypeId = typeNewDev
                    },
                    #endregion
                    
                    #region Ticket 2
                    new Ticket()
                    {
                        Title = "Nav Bar Not Working on Edge Browser", 
                        Description = "Navbar works on all browsers apart from Edge. I guess it's an edge case.", 
                        Created = portfolio.StartDate.AddDays(5), 
                        ProjectId = portfolio.Id ,
                        OwnerUserId = asimovMembers["ScottApple"],
                        DeveloperUserId = asimovMembers["MathewJacobs"],
                        TicketPriorityId = priorityMedium, 
                        TicketStatusId = statusResolved,
                        Archived = true,
                        TicketTypeId = typeChangeRequest
                    },
                    #endregion
                    
                    #region Ticket 3
                    new Ticket()
                    {
                        Title = "Change Styling to SaSS", 
                        Description = "We want to move away from basic CSS and use SaSS from now on", 
                        Created = portfolio.StartDate.AddDays(2), 
                        ProjectId = portfolio.Id ,
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev, 
                        TicketTypeId = typeEnhancement,
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        OwnerUserId = asimovMembers["ScottApple"],
                    },
                    #endregion
                    
                    #region Ticket 4
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = portfolio.Id , 
                        TicketPriorityId = priorityUrgent,
                        TicketStatusId = statusTest, 
                        TicketTypeId = typeDefect,
                        DeveloperUserId = asimovMembers["MathewJacobs"],
                        OwnerUserId = asimovMembers["SusanCalvin"],
                    },
                    
                    #endregion
                     
                    #region Ticket 5
                    new Ticket()
                    {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = portfolio.Id , 
                        TicketPriorityId = priorityLow, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeNewDev,
                        DeveloperUserId = asimovMembers["MathewJacobs"],
                        OwnerUserId = asimovMembers["SusanCalvin"]
                    },
                    #endregion
                    
                    #region Ticket 6
                    new Ticket()
                    {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = portfolio.Id , 
                        TicketPriorityId = priorityMedium, 
                        TicketStatusId = statusResolved, 
                        TicketTypeId = typeChangeRequest,
                        DeveloperUserId = asimovMembers["MathewJacobs"],
                        OwnerUserId = asimovMembers["SusanCalvin"]
                    },
                    #endregion
                    
                    #region Ticket 7
                    new Ticket()
                    {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = portfolio.Id , 
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev, 
                        TicketTypeId = typeEnhancement,
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        OwnerUserId = asimovMembers["SusanCalvin"]
                    },
                    #endregion
                    
                    #region Ticket 8
                    new Ticket()
                    {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = portfolio.Id , 
                        TicketPriorityId = priorityUrgent, 
                        TicketStatusId = statusTest, 
                        TicketTypeId = typeDefect,
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        OwnerUserId = asimovMembers["SusanCalvin"],
                    },
                    #endregion
                    
                    #endregion
                    
                    #region Bug Tracker Tickets  
                    
                    #region Ticket 1                                                                                                         
                    new Ticket() {
                        Title = "Change Images to PNG", 
                        Description = "Change images to PNG to speed up site load",
                        Created = bugtracker.StartDate.AddDays(1),
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["NatashaYobs"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeNewDev
                        
                    },
                    #endregion
                    
                    #region Ticket 2
                    new Ticket() {
                        Title = "Make Navbar Responsive", 
                        Description = "Navbar doesn't change size when the website is loaded on smaller handheld devices", 
                        Created = bugtracker.StartDate.AddDays(1),
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ScottApple"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeChangeRequest
                        
                    },
                    #endregion
                    
                    #region Ticket 3
                    new Ticket() {
                        Title = "Add Current Time on Navbar", 
                        Description = "Can we add a link in the nav bar to a new page that shows the current time?", 
                        Created = bugtracker.StartDate.AddDays(1),
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["NatashaYobs"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusTest,
                         TicketTypeId = typeChangeRequest
                         
                    },
                    #endregion
                    
                    #region Ticket 4
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeDefect
                        
                    },
                    #endregion
                    
                    #region Ticket 5
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusTest,
                         TicketTypeId = typeWorkTask
                         
                    },
                    #endregion
                    
                    #region Ticket 6
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeEnhancement
                        
                    },
                    #endregion
                    
                    #region Ticket 7
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusResolved, 
                        TicketTypeId = typeWorkTask
                        },
                    #endregion
                    
                    #region Ticket 8
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeWorkTask
                        },
                    #endregion
                    
                    #region Ticket 9 
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeWorkTask
                        
                    },
                    #endregion
                    
                    #region Ticket 10
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeNewDev},
                        
                    #endregion
                    
                    #region Ticket 11
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev, 
                        TicketTypeId = typeDefect},
                        
                    #endregion
                    
                    #region Ticket 12
                    new Ticket() {
                        Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusTest,
                         TicketTypeId = typeDefect},
                         
                    #endregion
                    
                    #region Ticket 13
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev, 
                        TicketTypeId = typeEnhancement
                        
                    },
                    #endregion
                    
                    #region Ticket 14
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusTest,
                         TicketTypeId = typeDefect},
                         
                    #endregion
                    
                    #region Ticket 15
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev, 
                        TicketTypeId = typeNewDev},
                        
                    #endregion
                    
                    #region Ticket 16
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeEnhancement
                        
                    },
                    #endregion
                    
                    #region Ticket 17
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusTest,
                         TicketTypeId = typeDefect},
                         
                    #endregion
                    
                    #region Ticket 18
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev, 
                        TicketTypeId = typeNewDev},
                        
                    #endregion
                    
                    #region Ticket 19
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusResolved, 
                        TicketTypeId = typeDefect
                        
                    },
                    #endregion
                    
                    #region Ticket 20
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeEnhancement
                        
                    },
                    #endregion
                    
                    #region Ticket 21
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusResolved, TicketTypeId = typeDefect
                        },
                    #endregion
                    
                    #region Ticket 22
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev, 
                        TicketTypeId = typeNewDev
                        
                    },
                        
                    #endregion
                    
                    #region Ticket 23
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeNewDev
                        
                    },
                        
                    #endregion
                    
                    #region Ticket 24
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev, 
                        TicketTypeId = typeDefect
                        
                    },
                        
                    #endregion
                    
                    #region Ticket 25
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev, 
                        TicketTypeId = typeChangeRequest
                        },
                    #endregion
                    
                    #region Ticket 26
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeDefect},
                        
                    #endregion
                    
                    #region Ticket 27
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeWorkTask
                        },
                    #endregion
                    
                    #region Ticket 28
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusResolved, 
                        TicketTypeId = typeWorkTask
                    },
                    #endregion
                    
                    #region Ticket 29
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusTest,
                         TicketTypeId = typeWorkTask
                         },
                    #endregion
                    
                    #region Ticket 30
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = bugtracker.Id , 
                        OwnerUserId = asimovMembers["ArronThomas"],
                        DeveloperUserId = asimovMembers["TonyTownsend"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeNewDev},
                        
                    #endregion
                    
                    #endregion
                    
                    #region Movie Tickets
                    
                    #region Ticket 1
                    new Ticket() {
                        Title = "Add New contact info", 
                        Description = "See the attachment, can we please update the 'Contact Us' page with this new information?" , 
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    TicketPriorityId = priorityLow, 
                    TicketStatusId = statusNew, 
                    TicketTypeId = typeDefect
                    
                    },

                    #endregion
                    
                    #region Ticket 2
                    new Ticket() {
                        Title = "Add New UI Colours", 
                        Description = "The UI colors need to be updated per the attached list.", 
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityMedium, 
                    TicketStatusId = statusDev, 
                    TicketTypeId = typeEnhancement
                    },
                    #endregion
                    
                    #region Ticket 3
                    new Ticket() {
                    Title = "Automatic reorder feature",
                    Description = "Customer gave us direction to add a feature. When the quantity of an item drops below 10, they want it to prompt the user to "
                                  + "to place a restock order, and generate the order for them if they choose yes.",
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityHigh,
                    TicketStatusId = statusNew, 
                    TicketTypeId = typeChangeRequest
                    },
                    #endregion
                    
                    #region Ticket 4
                    new Ticket() {
                                          Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    TicketPriorityId = priorityUrgent, 
                    TicketStatusId = statusNew, 
                    TicketTypeId = typeWorkTask
                    },
                    #endregion
                    
                    #region Ticket 5
                    new Ticket() {
                                          Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["TonyTownsend"],
                    TicketPriorityId = priorityLow, 
                    TicketStatusId = statusResolved,  
                    TicketTypeId = typeDefect
                    },
                    #endregion
                    
                    #region Ticket 6
                    new Ticket() {
                                          Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityMedium, 
                    TicketStatusId = statusNew,  
                    TicketTypeId = typeEnhancement
                    },
                    #endregion
                    
                    #region Ticket 7
                    new Ticket() {
                                          Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityHigh,
                    TicketStatusId = statusResolved, 
                    TicketTypeId = typeChangeRequest
                     
                    },
                    #endregion
                    
                    #region Ticket 8
                    new Ticket() {
                                          Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityUrgent, 
                    TicketStatusId = statusDev,  
                    TicketTypeId = typeWorkTask
                    },
                    #endregion
                    
                    #region Ticket 9
                    new Ticket() {
                                          Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityLow, 
                    TicketStatusId = statusNew,  
                    TicketTypeId = typeDefect
                    
                    },

                    #endregion
                    
                    #region Ticket 10
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityMedium, 
                    TicketStatusId = statusTest, 
                    TicketTypeId = typeEnhancement
                    
                    },
                    #endregion
                    
                    #region Ticket 11
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityHigh, 
                    TicketStatusId = statusDev,  
                    TicketTypeId = typeChangeRequest
                    },
                    #endregion
                    
                    #region Ticket 12
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityUrgent, 
                    TicketStatusId = statusTest,  
                    TicketTypeId = typeChangeRequest
                    
                    },
                    #endregion
                    
                    #region Ticket 13
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityLow, 
                    TicketStatusId = statusResolved, 
                    TicketTypeId = typeDefect
                    },
                    #endregion
                    
                    #region Ticket 14
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityMedium, 
                    TicketStatusId = statusDev,  
                    TicketTypeId = typeEnhancement
                    
                    },
                    #endregion
                    
                    #region Ticket 15
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityHigh, 
                    TicketStatusId = statusNew,  
                    TicketTypeId = typeChangeRequest
                    
                    },
                    #endregion
                    
                    #region Ticket 16
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityUrgent, 
                    TicketStatusId = statusResolved, 
                    TicketTypeId = typeWorkTask
                    
                    },
                    #endregion
                    
                    #region Ticket 17
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityHigh, 
                    TicketStatusId = statusDev,  
                    TicketTypeId = typeNewDev
                    
                    },
                    #endregion
                    
                    #region Ticket 18
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityMedium, 
                    TicketStatusId = statusDev,  
                    TicketTypeId = typeEnhancement
                    },
                    #endregion
                    
                    #region Ticket 19
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityHigh, 
                    TicketStatusId = statusNew,  
                    TicketTypeId = typeChangeRequest
                    
                    },
                    #endregion
                    
                    #region Ticket 20
                    new Ticket() {
                                            Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                    Created = DateTimeOffset.Now, 
                    ProjectId = movie.Id , 
                    OwnerUserId = asimovMembers["SusanCalvin"],
                    DeveloperUserId = asimovMembers["MathewJacobs"],
                    TicketPriorityId = priorityUrgent, 
                    TicketStatusId = statusNew, 
                    TicketTypeId = typeNewDev
                    },
                    #endregion
                    
                    #endregion
                    
                    #region Blog Tickets

                        #region Ticket 1
                        new Ticket() {
                            Title = "Crash when deleting certain products", 
                            Description = "The app is crashing when a user tries to delete certain products. We can't figure out what these items have in common, but it's repeatable and" +
                                          " always happens on the same ones. I attached the list.", 
                            Created = blog.StartDate.AddDays(1),
                            ProjectId = blog.Id ,
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["JamesPeters"],
                            TicketPriorityId = priorityLow, 
                            TicketStatusId = statusDev, 
                            TicketTypeId = typeDefect
                        },
                        #endregion
                        
                        #region Ticket 2
                        new Ticket() {
                            Title = "New report column", 
                            Description = "The customer wants to add a new column to the \"Items in Stock\" report. It should be titled \"Last Cost\" and show the last price the item"
                                          + " was purchased at", 
                            Created = blog.StartDate.AddDays(1),
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["JamesPeters"],
                            TicketPriorityId = priorityMedium, 
                            TicketStatusId = statusDev, 
                            TicketTypeId = typeEnhancement
                        },
                        #endregion

                        #region Ticket 3
                        new Ticket() {
                            Title = "UI colors", 
                            Description = "Just a reminder ticket so I don't forget about it. The UI colors need to be updated per the attached list. I'll get to it eventually but if anyone is looking for something to do"
                                          + " this would help me out.", 
                            Created = blog.StartDate.AddDays(2),
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["SueLincoln"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityHigh, 
                            TicketStatusId = statusDev, 
                            TicketTypeId = typeChangeRequest
                        },
                        #endregion
                        
                        #region Ticket 4
                        new Ticket() {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityUrgent, 
                            TicketStatusId = statusNew, 
                            TicketTypeId = typeNewDev
                        },
                        #endregion
                        
                        #region Ticket 5
                        new Ticket() {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityLow, 
                            TicketStatusId = statusDev,  
                            TicketTypeId = typeDefect
                        },
                        #endregion
                        
                        #region Ticket 6
                        new Ticket() {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityMedium, 
                            TicketStatusId = statusTest,  
                            TicketTypeId = typeEnhancement
                        },
                        #endregion
                        
                        #region Ticket 7
                        new Ticket() {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityHigh, 
                            TicketStatusId = statusResolved, 
                            TicketTypeId = typeChangeRequest
                        },
                        #endregion
                        
                        #region Ticket 8
                        new Ticket() {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityUrgent, 
                            TicketStatusId = statusDev,  
                            TicketTypeId = typeWorkTask
                        },
                        #endregion
                        
                        #region Ticket 9
                        new Ticket() {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityLow, 
                            TicketStatusId = statusNew,  
                            TicketTypeId = typeDefect
                        },
                        #endregion
                        
                        #region Ticket 10
                        new Ticket()
                        {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityMedium, 
                            TicketStatusId = statusNew, 
                            TicketTypeId = typeEnhancement
                        },
                        #endregion
                        
                        #region Ticket 11
                        new Ticket()
                        {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityHigh, 
                            TicketStatusId = statusResolved,  
                            TicketTypeId = typeChangeRequest
                        },
                        #endregion
                        
                        #region Ticket 12
                        new Ticket()
                        {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityUrgent, 
                            TicketStatusId = statusNew,  
                            TicketTypeId = typeWorkTask
                        },
                        #endregion
                        
                        #region Ticket 13
                        new Ticket()
                        {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityLow, 
                            TicketStatusId = statusNew, 
                            TicketTypeId = typeDefect
                        },
                        #endregion
                        
                        #region Ticket 14
                        new Ticket()
                        {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityMedium, 
                            TicketStatusId = statusDev,  
                            TicketTypeId = typeEnhancement
                        },
                        #endregion
                        
                        #region Ticket 15
                        new Ticket()
                        {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityHigh, 
                            TicketStatusId = statusTest,  
                            TicketTypeId = typeChangeRequest
                        },
                        #endregion
                        
                        #region Ticket 16
                        new Ticket()
                        {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityUrgent, 
                            TicketStatusId = statusNew, 
                            TicketTypeId = typeNewDev
                        },
                        #endregion
                        
                        #region Ticket 17
                        new Ticket()
                        {
                            Title = TextGenerator.Title(),
                            Description = TextGenerator.Description(),
                            Created = DateTimeOffset.Now, 
                            ProjectId = blog.Id , 
                            OwnerUserId = linuxMembers["JaneRichards"],
                            DeveloperUserId = linuxMembers["BruceTurner"],
                            TicketPriorityId = priorityHigh, 
                            TicketStatusId = statusResolved,  
                            TicketTypeId = typeNewDev
                        },
                        #endregion
                    
                    #endregion
                    
                    #region Address Book Tickets
                    
                    #region Ticket 1
                    new Ticket() {
                        Title = "Logo placement", 
                        Description = "Please tweak the placement of the customer's logo as shown in the attachment", 
                        Created = addressBook.StartDate.AddDays(1), 
                        ProjectId = addressBook.Id , 
                        OwnerUserId = linuxMembers["FredHopkins"],
                        DeveloperUserId = linuxMembers["CarolSmith"],
                        TicketPriorityId = priorityLow, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeDefect
                        },
                    #endregion
                    
                    #region Ticket 2
                    new Ticket() {
                        Title = "Database swap", 
                        Description = "Can we change the database from SQLServer to PostgreSQL. I know, I know. Let me know what this is going to take.",
                        Created = addressBook.StartDate.AddDays(2), 
                        ProjectId = addressBook.Id , 
                        OwnerUserId = linuxMembers["FredHopkins"],
                        DeveloperUserId = linuxMembers["CarolSmith"],
                        TicketPriorityId = priorityMedium, 
                        TicketStatusId = statusResolved, 
                        TicketTypeId = typeEnhancement
                        },
                    #endregion
                    
                    #region Ticket 3
                    new Ticket() {
                        Title = "Menu page",
                        Description = "Can we add a link in the nav bar to a new page that shows the current menu? See the attachment.",
                        Created = addressBook.StartDate.AddDays(1), 
                        ProjectId = addressBook.Id , 
                        OwnerUserId = linuxMembers["FredHopkins"],
                        DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusTest, 
                        TicketTypeId = typeChangeRequest
                        },
                    #endregion
                    
                    #region Ticket 4
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityUrgent, 
                        TicketStatusId = statusTest, 
                        TicketTypeId = typeNewDev
                        },
                    #endregion
                    
                    #region Ticket 5
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityLow, 
                        TicketStatusId = statusResolved,  
                        TicketTypeId = typeDefect
                        },
                    #endregion
                    
                    #region Ticket 6
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityMedium, 
                        TicketStatusId = statusNew,  
                        TicketTypeId = typeEnhancement
                        },
                    #endregion
                    
                    #region Ticket 7
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeChangeRequest
                        },
                    #endregion
                    
                    #region Ticket 8
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityUrgent, 
                        TicketStatusId = statusDev,  
                        TicketTypeId = typeNewDev
                        },
                    #endregion
                    
                    #region Ticket 9
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityLow, 
                        TicketStatusId = statusNew,  
                        TicketTypeId = typeDefect
                        },
                    #endregion
                    
                    #region Ticket 10
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityMedium, 
                        TicketStatusId = statusResolved, 
                        TicketTypeId = typeEnhancement
                        },
                    #endregion
                    
                    #region Ticket 11
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusDev,  
                        TicketTypeId = typeChangeRequest
                        },
                    #endregion
                    
                    #region Ticket 12
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityUrgent, 
                        TicketStatusId = statusTest,  
                        TicketTypeId = typeNewDev
                        },
                    #endregion
                    
                    #region Ticket 13
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityLow, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeDefect
                        },
                    #endregion
                    
                    #region Ticket 14
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityMedium, 
                        TicketStatusId = statusDev,  
                        TicketTypeId = typeEnhancement
                        },
                    #endregion
                    
                    #region Ticket 15
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew,  
                        TicketTypeId = typeChangeRequest
                        },
                    #endregion
                    
                    #region Ticket 16
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityUrgent, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeNewDev
                        },
                    #endregion
                    
                    #region Ticket 17
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusResolved,  
                        TicketTypeId = typeNewDev
                        },
                    #endregion
                    
                    #region Ticket 18
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityMedium, 
                        TicketStatusId = statusDev,  
                        TicketTypeId = typeEnhancement
                        },
                    #endregion
                    
                    #region Ticket 19
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityHigh, 
                        TicketStatusId = statusNew,  
                        TicketTypeId = typeChangeRequest
                        },
                    #endregion
                    
                    #region Ticket 20
                    new Ticket() {
                                                Title = TextGenerator.Title(),
                        Description = TextGenerator.Description(),
                        Created = DateTimeOffset.Now, 
                        ProjectId = addressBook.Id , 
                                                OwnerUserId = linuxMembers["FredHopkins"],
                                                DeveloperUserId = linuxMembers["JamesPeters"],
                        TicketPriorityId = priorityUrgent, 
                        TicketStatusId = statusNew, 
                        TicketTypeId = typeNewDev
                        },
                    #endregion
                    
                    #endregion
                };


                var dbTickets = context.Tickets.Select(c => c.Title).ToList();
                await context.Tickets.AddRangeAsync(tickets.Where(c => !dbTickets.Contains(c.Title)));
                await context.SaveChangesAsync();
                
                await SeedDefaultTicketComments.SeedAsimovCommentsAsync(context, ticketSvc, ticketHistorySvc, asimovMembers);
                await SeedDefaultTicketComments.SeedLinuxCommentsAsync(context, ticketSvc, ticketHistorySvc, linuxMembers);
                
                await SeedDefaultTicketAttachments.SeedAsimovAttachmentsAsync(context,ticketSvc, ticketHistorySvc, asimovMembers);
                await SeedDefaultTicketAttachments.SeedLinuxAttachmentsAsync(context, ticketSvc, ticketHistorySvc, linuxMembers);

                
                // await SeedDefaultTicketHistoryItems.SeedAsimovHistoryItemsAsync(context, ticketHistorySvc, asimovMembers);
                // await SeedDefaultTicketHistoryItems.SeedLinuxHistoryItemsAsync(context, ticketHistorySvc, linuxMembers);

                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Tickets.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
            // private static async Task SeedCommentsAsync()
            // {
            //    
            // }
            //
            // private static async Task SeedAttachmentsAsync()
            // {
            //     await SeedPortfolioWebsiteAttachmentsAsync();
            //     await SeedIssueTrackerWebAppAttachmentsAsync();
            //     await SeedMovieInfoWebAppAttachmentsAsync();
            //     await SeedBlogWebAppAttachmentsAsync();
            //     await SeedAddressBookWebAppAttachmentsAsync();
            // }
            //
            // private static async Task SeedHistoryItemsAsync()
            // {
            //     await SeedPortfolioWebsiteHistoryItemsAsync();
            //     await SeedIssueTrackerWebAppHistoryItemsAsync();
            //     await SeedMovieInfoWebAppHistoryItemsAsync();
            //     await SeedBlogWebAppHistoryItemsAsync();
            //     await SeedAddressBookWebAppHistoryItemsAsync();
            // }



}