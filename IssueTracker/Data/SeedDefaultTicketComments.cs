using System.Collections;
using System.Text;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;

namespace IssueTracker.Data;

public static class SeedDefaultTicketComments
{
    
    public static async Task SeedAsimovCommentsAsync( ApplicationDbContext context, ITicketService ticketService, ITicketHistoryService ticketHistoryService, SortedList<string, string> asimovMembers)
    {
        #region PortfolioWebApp
            Ticket PortfolioTicketOne = context.Tickets.First(t => t.Title == "Nav Bar Not Working on Edge Browser");
            Ticket PortfolioTicketTwo = context.Tickets.First(t => t.Title == "Change Styling to SaSS");
            Ticket PortfolioTicketThree = context.Tickets.First(t => t.Title == "Fix Styling to Email Form");
            await ticketHistoryService.AddTicketCreatedEventAsync(PortfolioTicketOne.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(PortfolioTicketTwo.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(PortfolioTicketThree.Id);
            
            TicketComment PortfolioCommentOne = new TicketComment()
            {
                TicketId = PortfolioTicketOne.Id,
                UserId = asimovMembers["SusanCalvin"],
                Created = PortfolioTicketOne.Created + new TimeSpan(days: 1, hours: 18, minutes: 0, seconds: 54),
                Comment = TextGenerator.Description()
            };
            
            TicketComment PortfolioCommentTwo = new TicketComment()
            {
                TicketId = PortfolioTicketOne.Id,
                UserId = asimovMembers["NatashaYobs"],
                Created = PortfolioTicketOne.Created + new TimeSpan(days: 4, hours: 5, minutes: 0, seconds: 23),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment PortfolioCommentThree = new TicketComment()
            {
                TicketId = PortfolioTicketOne.Id,
                UserId = asimovMembers["TonyTownsend"],
                Created = PortfolioTicketOne.Created + new TimeSpan(days: 6, hours: 4, minutes: 25, seconds: 43),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment PortfolioCommentFour = new TicketComment()
            {
                TicketId = PortfolioTicketTwo.Id,
                UserId = asimovMembers["SusanCalvin"],
                Created = PortfolioTicketTwo.Created + new TimeSpan(days: 6, hours: 4, minutes: 28, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment PortfolioCommentFive = new TicketComment()
            {
                TicketId = PortfolioTicketThree.Id,
                UserId = asimovMembers["SusanCalvin"],
                Created = PortfolioTicketThree.Created + new TimeSpan(days: 12, hours: 12, minutes: 0, seconds: 52),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment PortfolioCommentSix = new TicketComment()
            {
                TicketId = PortfolioTicketThree.Id,
                UserId = asimovMembers["MathewJacobs"],
                Created = PortfolioTicketThree.Created + new TimeSpan(days: 14, hours: 18, minutes: 0, seconds: 2),
                Comment = TextGenerator.Description()
            };
            
            await ticketService.AddNewTicketCommentAsync(PortfolioTicketOne.Id, PortfolioCommentOne);
            await ticketService.AddNewTicketCommentAsync(PortfolioTicketOne.Id, PortfolioCommentTwo);
            await ticketService.AddNewTicketCommentAsync(PortfolioTicketOne.Id, PortfolioCommentThree);
            await ticketService.AddNewTicketCommentAsync(PortfolioTicketTwo.Id, PortfolioCommentFour);
            await ticketService.AddNewTicketCommentAsync(PortfolioTicketThree.Id, PortfolioCommentFive);
            await ticketService.AddNewTicketCommentAsync(PortfolioTicketThree.Id, PortfolioCommentSix);
            
        #endregion
        
        #region IssueTracker
            Ticket IssueTrackerTicketOne = context.Tickets.First(t => t.Title == "Change Images to PNG");
            Ticket IssueTrackerTicketTwo = context.Tickets.First(t => t.Title == "Make Navbar Responsive");
            Ticket IssueTrackerTicketThree = context.Tickets.First(t => t.Title == "Add Current Time on Navbar");
            await ticketHistoryService.AddTicketCreatedEventAsync(IssueTrackerTicketOne.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(IssueTrackerTicketTwo.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(IssueTrackerTicketThree.Id);
            
            TicketComment IssueTrackerCommentOne = new TicketComment()
            {
                TicketId = IssueTrackerTicketOne.Id,
                UserId = asimovMembers["ArronThomas"],
                Created = IssueTrackerTicketOne.Created + new TimeSpan(days: 0, hours: 1, minutes: 27, seconds: 54),
                Comment = TextGenerator.Description()
            };
            
            TicketComment IssueTrackerCommentTwo = new TicketComment()
            {
                TicketId = IssueTrackerTicketOne.Id,
                UserId = asimovMembers["TonyTownsend"],
                Created = IssueTrackerTicketOne.Created + new TimeSpan(days: 0, hours: 2, minutes: 11, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment IssueTrackerCommentThree = new TicketComment()
            {
                TicketId = IssueTrackerTicketOne.Id,
                UserId = asimovMembers["NatashaYobs"],
                Created = IssueTrackerTicketOne.Created + new TimeSpan(days: 3, hours: 13, minutes: 23, seconds: 41),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment IssueTrackerCommentFour = new TicketComment()
            {
                TicketId = IssueTrackerTicketTwo.Id,
                UserId = asimovMembers["ArronThomas"],
                Created = IssueTrackerTicketTwo.Created + new TimeSpan(days: 3, hours: 4, minutes: 56, seconds: 41),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment IssueTrackerCommentFive = new TicketComment()
            {
                TicketId = IssueTrackerTicketThree.Id,
                UserId = asimovMembers["ArronThomas"],
                Created = IssueTrackerTicketThree.Created + new TimeSpan(days: 4, hours: 15, minutes: 24, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment IssueTrackerCommentSix = new TicketComment()
            {
                TicketId = IssueTrackerTicketThree.Id,
                UserId = asimovMembers["NatashaYobs"],
                Created = IssueTrackerTicketThree.Created + new TimeSpan(days: 4, hours: 15, minutes: 50, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            await ticketService.AddNewTicketCommentAsync(IssueTrackerTicketOne.Id, IssueTrackerCommentOne);
            await ticketService.AddNewTicketCommentAsync(IssueTrackerTicketOne.Id, IssueTrackerCommentTwo);
            await ticketService.AddNewTicketCommentAsync(IssueTrackerTicketOne.Id, IssueTrackerCommentThree);
            await ticketService.AddNewTicketCommentAsync(IssueTrackerTicketTwo.Id, IssueTrackerCommentFour);
            await ticketService.AddNewTicketCommentAsync(IssueTrackerTicketThree.Id, IssueTrackerCommentFive);
            await ticketService.AddNewTicketCommentAsync(IssueTrackerTicketThree.Id, IssueTrackerCommentSix);
        #endregion
        
        #region MovieInfoWebApp
            Ticket MovieInfoWebAppTicketOne = context.Tickets.First(t => t.Title == "Add New contact info");
            Ticket MovieInfoWebAppTicketTwo = context.Tickets.First(t => t.Title == "Add New UI Colours");
            Ticket MovieInfoWebAppTicketThree = context.Tickets.First(t => t.Title == "Automatic reorder feature");
            await ticketHistoryService.AddTicketCreatedEventAsync(MovieInfoWebAppTicketOne.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(MovieInfoWebAppTicketTwo.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(MovieInfoWebAppTicketThree.Id);
            
            TicketComment MovieInfoWebAppCommentOne = new TicketComment()
            {
                TicketId = MovieInfoWebAppTicketOne.Id,
                UserId = asimovMembers["TonyTownsend"],
                Created = MovieInfoWebAppTicketOne.Created + new TimeSpan(days: 0, hours: 0, minutes: 23, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            TicketComment MovieInfoWebAppCommentTwo = new TicketComment()
            {
                TicketId = MovieInfoWebAppTicketOne.Id,
                UserId = asimovMembers["MathewJacobs"],
                Created = MovieInfoWebAppTicketOne.Created + new TimeSpan(days: 0, hours: 1, minutes: 1, seconds: 53),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment MovieInfoWebAppCommentThree = new TicketComment()
            {
                TicketId = MovieInfoWebAppTicketOne.Id,
                UserId = asimovMembers["MathewJacobs"],
                Created = MovieInfoWebAppTicketOne.Created + new TimeSpan(days: 1, hours: 12, minutes: 12, seconds: 53),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment MovieInfoWebAppCommentFour = new TicketComment()
            {
                TicketId = MovieInfoWebAppTicketTwo.Id,
                UserId = asimovMembers["TonyTownsend"],
                Created = MovieInfoWebAppTicketTwo.Created + new TimeSpan(days: 2, hours: 6, minutes: 6, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment MovieInfoWebAppCommentFive = new TicketComment()
            {
                TicketId = MovieInfoWebAppTicketThree.Id,
                UserId = asimovMembers["SusanCalvin"],
                Created = MovieInfoWebAppTicketThree.Created + new TimeSpan(days: 2, hours: 7, minutes: 42, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment MovieInfoWebAppCommentSix = new TicketComment()
            {
                TicketId = MovieInfoWebAppTicketThree.Id,
                UserId = asimovMembers["SusanCalvin"],
                Created = MovieInfoWebAppTicketThree.Created + new TimeSpan(days: 2, hours: 7, minutes: 50, seconds: 42),
                Comment = TextGenerator.Description()
            };
            
            await ticketService.AddNewTicketCommentAsync(MovieInfoWebAppTicketOne.Id, MovieInfoWebAppCommentOne);
            await ticketService.AddNewTicketCommentAsync(MovieInfoWebAppTicketOne.Id, MovieInfoWebAppCommentTwo);
            await ticketService.AddNewTicketCommentAsync(MovieInfoWebAppTicketOne.Id, MovieInfoWebAppCommentThree);
            await ticketService.AddNewTicketCommentAsync(MovieInfoWebAppTicketTwo.Id, MovieInfoWebAppCommentFour);
            await ticketService.AddNewTicketCommentAsync(MovieInfoWebAppTicketThree.Id, MovieInfoWebAppCommentFive);
            await ticketService.AddNewTicketCommentAsync(MovieInfoWebAppTicketThree.Id, MovieInfoWebAppCommentSix);
        #endregion
    }

    
    public static async Task SeedLinuxCommentsAsync( ApplicationDbContext context, ITicketService ticketService,ITicketHistoryService ticketHistoryService, SortedList<string, string> linuxMembers)
    {
        
        #region BlogWebApp
            Ticket BlogWebTicketOne = context.Tickets.First(t => t.Title == "Crash when deleting certain products");
            Ticket BlogWebTicketTwo = context.Tickets.First(t => t.Title == "New report column");
            Ticket BlogWebTicketThree = context.Tickets.First(t => t.Title == "UI colors");
            await ticketHistoryService.AddTicketCreatedEventAsync(BlogWebTicketOne.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(BlogWebTicketTwo.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(BlogWebTicketThree.Id);
            
            TicketComment BlogWebCommentOne = new TicketComment()
            {
                TicketId = BlogWebTicketOne.Id,
                UserId = linuxMembers["JaneRichards"],
                Created = BlogWebTicketOne.Created + new TimeSpan(days: 0, hours: 0, minutes: 10, seconds: 43),
                Comment = TextGenerator.Description()
            };
            
            TicketComment BlogWebCommentTwo = new TicketComment()
            {
                TicketId = BlogWebTicketOne.Id,
                UserId = linuxMembers["JamesPeters"],
                Created = BlogWebTicketOne.Created + new TimeSpan(days: 0, hours: 0, minutes: 13, seconds: 14),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment BlogWebCommentThree = new TicketComment()
            {
                TicketId = BlogWebTicketOne.Id,
                UserId = linuxMembers["JaneRichards"],
                Created = BlogWebTicketOne.Created + new TimeSpan(days: 4, hours: 4, minutes: 12, seconds: 34),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment BlogWebCommentFour = new TicketComment()
            {
                TicketId = BlogWebTicketTwo.Id,
                UserId = linuxMembers["JamesPeters"],
                Created = BlogWebTicketTwo.Created + new TimeSpan(days: 4, hours: 4, minutes: 14, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment BlogWebCommentFive = new TicketComment()
            {
                TicketId = BlogWebTicketThree.Id,
                UserId = linuxMembers["BruceTurner"],
                Created = BlogWebTicketThree.Created + new TimeSpan(days: 20, hours: 5, minutes: 23, seconds: 41),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment BlogWebCommentSix = new TicketComment()
            {
                TicketId = BlogWebTicketThree.Id,
                UserId = linuxMembers["JaneRichards"],
                Created = BlogWebTicketThree.Created + new TimeSpan(days: 20, hours: 5, minutes: 55, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            await ticketService.AddNewTicketCommentAsync(BlogWebTicketOne.Id, BlogWebCommentOne);
            await ticketService.AddNewTicketCommentAsync(BlogWebTicketOne.Id, BlogWebCommentTwo);
            await ticketService.AddNewTicketCommentAsync(BlogWebTicketOne.Id, BlogWebCommentThree);
            await ticketService.AddNewTicketCommentAsync(BlogWebTicketTwo.Id, BlogWebCommentFour);
            await ticketService.AddNewTicketCommentAsync(BlogWebTicketThree.Id, BlogWebCommentFive);
            await ticketService.AddNewTicketCommentAsync(BlogWebTicketThree.Id, BlogWebCommentSix);
        #endregion
        
        #region AddressBookWebApp
            Ticket AddressBookTicketOne = context.Tickets.First(t => t.Title == "Logo placement");
            Ticket AddressBookTicketTwo = context.Tickets.First(t => t.Title == "Database swap");
            Ticket AddressBookTicketThree = context.Tickets.First(t => t.Title == "Menu page");
            await ticketHistoryService.AddTicketCreatedEventAsync(AddressBookTicketOne.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(AddressBookTicketTwo.Id);
            await ticketHistoryService.AddTicketCreatedEventAsync(AddressBookTicketThree.Id);
            
            TicketComment AddressBookCommentOne = new TicketComment()
            {
                TicketId = AddressBookTicketOne.Id,
                UserId = linuxMembers["FredHopkins"],
                Created = AddressBookTicketOne.Created + new TimeSpan(days: 0, hours: 2, minutes: 12, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            TicketComment AddressBookCommentTwo = new TicketComment()
            {
                TicketId = AddressBookTicketOne.Id,
                UserId = linuxMembers["JamesPeters"],
                Created = AddressBookTicketOne.Created + new TimeSpan(days: 0, hours: 2, minutes: 14, seconds: 34),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment AddressBookCommentThree = new TicketComment()
            {
                TicketId = AddressBookTicketOne.Id,
                UserId = linuxMembers["JamesPeters"],
                Created = AddressBookTicketOne.Created + new TimeSpan(days: 0, hours: 4, minutes: 52, seconds: 54),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment AddressBookCommentFour = new TicketComment()
            {
                TicketId = AddressBookTicketTwo.Id,
                UserId = linuxMembers["FredHopkins"],
                Created = AddressBookTicketTwo.Created + new TimeSpan(days: 6, hours: 4, minutes: 55, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment AddressBookCommentFive = new TicketComment()
            {
                TicketId = AddressBookTicketThree.Id,
                UserId = linuxMembers["CarolSmith"],
                Created = AddressBookTicketThree.Created + new TimeSpan(days: 6, hours: 5, minutes: 12, seconds: 42),
                Comment = TextGenerator.Description()
            };
            
            
            TicketComment AddressBookCommentSix = new TicketComment()
            {
                TicketId = AddressBookTicketThree.Id,
                UserId = linuxMembers["CarolSmith"],
                Created = AddressBookTicketThree.Created + new TimeSpan(days: 9, hours: 5, minutes: 23, seconds: 12),
                Comment = TextGenerator.Description()
            };
            
            await ticketService.AddNewTicketCommentAsync(AddressBookTicketOne.Id, AddressBookCommentOne);
            await ticketService.AddNewTicketCommentAsync(AddressBookTicketOne.Id, AddressBookCommentTwo);
            await ticketService.AddNewTicketCommentAsync(AddressBookTicketOne.Id, AddressBookCommentThree);
            await ticketService.AddNewTicketCommentAsync(AddressBookTicketTwo.Id, AddressBookCommentFour);
            await ticketService.AddNewTicketCommentAsync(AddressBookTicketThree.Id, AddressBookCommentFive);
            await ticketService.AddNewTicketCommentAsync(AddressBookTicketThree.Id, AddressBookCommentSix);
        #endregion
    }
}