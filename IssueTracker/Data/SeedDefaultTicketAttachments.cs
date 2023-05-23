using System.Collections;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;

namespace IssueTracker.Data;

public static class SeedDefaultTicketAttachments
{

    
    public static async Task SeedAsimovAttachmentsAsync(ApplicationDbContext context,ITicketService ticketService, ITicketHistoryService ticketHistoryService, SortedList<string,string> asimovMembers)
    {
        #region PortfolioWebApp
        Ticket PortfolioTicketOne = context.Tickets.First(t => t.Title == "Nav Bar Not Working on Edge Browser");
        Ticket PortfolioTicketTwo = context.Tickets.First(t => t.Title == "Change Styling to SaSS");
        // Ticket PortfolioTicketThree = context.Tickets.First(t => t.Title == "Fix Styling to Email Form");
        
        
        TicketAttachment PortfolioWebAppExampleAttachment = new TicketAttachment()
        {
            TicketId = PortfolioTicketOne.Id,
            UserId = asimovMembers["SusanCalvin"],
            Created = PortfolioTicketOne.Created + new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.png"
        };

        TicketAttachment PortfolioWebAppExampleAttachment2 = new TicketAttachment()
        {
            TicketId = PortfolioTicketTwo.Id,
            UserId = asimovMembers["MathewJacobs"],
            Created = PortfolioTicketTwo.Created + new TimeSpan(days: 0, hours: 20, minutes: 9, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.png"
        };

        using (FileStream fs = File.OpenRead($"wwwroot/ExampleAttachment.png"))
        {
            using (var memoryStream = new MemoryStream())
            {
                fs.CopyTo(memoryStream);
                PortfolioWebAppExampleAttachment.FileData = memoryStream.ToArray();
                PortfolioWebAppExampleAttachment.FileContentType = "application/png";
                PortfolioWebAppExampleAttachment2.FileData = memoryStream.ToArray();
                PortfolioWebAppExampleAttachment2.FileContentType = "application/png";
            }
        }

        await ticketService.AddTicketAttachmentAsync(PortfolioTicketOne.Id, PortfolioWebAppExampleAttachment);
        await ticketService.AddTicketAttachmentAsync(PortfolioTicketTwo.Id, PortfolioWebAppExampleAttachment2);
        await ticketHistoryService.AddAttachmentEventAsync(PortfolioTicketOne, PortfolioWebAppExampleAttachment);
        await ticketHistoryService.AddAttachmentEventAsync(PortfolioTicketTwo, PortfolioWebAppExampleAttachment2);
        
        

        #endregion
        
        #region IssueTracker
        Ticket IssueTrackerTicketOne = context.Tickets.First(t => t.Title == "Change Images to PNG");
        // Ticket IssueTrackerTicketTwo = context.Tickets.First(t => t.Title == "Make Navbar Responsive");
        Ticket IssueTrackerTicketThree = context.Tickets.First(t => t.Title == "Add Current Time on Navbar");
        
        TicketAttachment IssueTrackerExampleAttachment = new TicketAttachment()
        {
            TicketId = IssueTrackerTicketOne.Id,
            UserId = asimovMembers["NatashaYobs"],
            Created = IssueTrackerTicketOne.Created + new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.pdf"
        };

        TicketAttachment IssueTrackerExampleAttachment2 = new TicketAttachment()
        {
            TicketId = IssueTrackerTicketThree.Id,
            UserId = asimovMembers["TonyTownsend"],
            Created = IssueTrackerTicketThree.Created + new TimeSpan(days: 0, hours: 20, minutes: 9, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.pdf"
        };

        using (FileStream fs = File.OpenRead($"wwwroot/ExampleAttachment.pdf"))
        {
            using (var memoryStream = new MemoryStream())
            {
                fs.CopyTo(memoryStream);
                IssueTrackerExampleAttachment.FileData = memoryStream.ToArray();
                IssueTrackerExampleAttachment.FileContentType = "application/pdf";
                IssueTrackerExampleAttachment2.FileData = memoryStream.ToArray();
                IssueTrackerExampleAttachment2.FileContentType = "application/pdf";
            }
        }

        await ticketService.AddTicketAttachmentAsync(IssueTrackerTicketOne.Id, IssueTrackerExampleAttachment);
        await ticketService.AddTicketAttachmentAsync(IssueTrackerTicketThree.Id, IssueTrackerExampleAttachment2);
        await ticketHistoryService.AddAttachmentEventAsync(IssueTrackerTicketOne, IssueTrackerExampleAttachment);
        await ticketHistoryService.AddAttachmentEventAsync(IssueTrackerTicketThree, IssueTrackerExampleAttachment2);

        #endregion
        
        #region MovieInfoWebApp
        Ticket MovieInfoWebAppTicketOne = context.Tickets.First(t => t.Title == "Add New contact info");
        // Ticket MovieInfoWebAppTickeTwo = context.Tickets.First(t => t.Title == "Add New UI Colours");
        Ticket MovieInfoWebAppTicketThree = context.Tickets.First(t => t.Title == "Automatic reorder feature");
        
        TicketAttachment MovieInfoWebAppExampleAttachment = new TicketAttachment()
        {
            TicketId = MovieInfoWebAppTicketOne.Id,
            UserId = asimovMembers["TonyTownsend"],
            Created = MovieInfoWebAppTicketOne.Created + new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.png"
        };

        TicketAttachment MovieInfoWebAppExampleAttachment2 = new TicketAttachment()
        {
            TicketId = MovieInfoWebAppTicketThree.Id,
            UserId = asimovMembers["SusanCalvin"],
            Created = MovieInfoWebAppTicketThree.Created + new TimeSpan(days: 0, hours: 20, minutes: 9, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.png"
        };

        using (FileStream fs = File.OpenRead($"wwwroot/ExampleAttachment.png"))
        {
            using (var memoryStream = new MemoryStream())
            {
                fs.CopyTo(memoryStream);
                MovieInfoWebAppExampleAttachment.FileData = memoryStream.ToArray();
                MovieInfoWebAppExampleAttachment.FileContentType = "application/png";
                MovieInfoWebAppExampleAttachment2.FileData = memoryStream.ToArray();
                MovieInfoWebAppExampleAttachment2.FileContentType = "application/png";
            }
        }

        await ticketService.AddTicketAttachmentAsync(MovieInfoWebAppTicketOne.Id, MovieInfoWebAppExampleAttachment);
        await ticketService.AddTicketAttachmentAsync(MovieInfoWebAppTicketThree.Id, MovieInfoWebAppExampleAttachment2);
        await ticketHistoryService.AddAttachmentEventAsync(MovieInfoWebAppTicketOne, MovieInfoWebAppExampleAttachment);
        await ticketHistoryService.AddAttachmentEventAsync(MovieInfoWebAppTicketThree, MovieInfoWebAppExampleAttachment2);

        #endregion
    }
    
    public static async Task SeedLinuxAttachmentsAsync(ApplicationDbContext context,ITicketService ticketService, ITicketHistoryService ticketHistoryService, SortedList<string,string> linuxMembers)
    {
        #region BlogWebApp
        Ticket BlogWebTicketOne = context.Tickets.First(t => t.Title == "Crash when deleting certain products");
        Ticket BlogWebTicketTwo = context.Tickets.First(t => t.Title == "New report column");
        // Ticket BlogWebTicketThree = context.Tickets.First(t => t.Title == "UI colors");
        
        TicketAttachment BlogWebTicketOneExampleAttachment = new TicketAttachment()
        {
            TicketId = BlogWebTicketOne.Id,
            UserId = linuxMembers["BruceTurner"],
            Created = BlogWebTicketOne.Created + new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.pdf"
        };

        TicketAttachment BlogWebTicketOneExampleAttachment2 = new TicketAttachment()
        {
            TicketId = BlogWebTicketTwo.Id,
            UserId = linuxMembers["JaneRichards"],
            Created = BlogWebTicketTwo.Created + new TimeSpan(days: 0, hours: 20, minutes: 9, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.pdf"
        };

        using (FileStream fs = File.OpenRead($"wwwroot/ExampleAttachment.pdf"))
        {
            using (var memoryStream = new MemoryStream())
            {
                fs.CopyTo(memoryStream);
                BlogWebTicketOneExampleAttachment.FileData = memoryStream.ToArray();
                BlogWebTicketOneExampleAttachment.FileContentType = "application/pdf";
                BlogWebTicketOneExampleAttachment2.FileData = memoryStream.ToArray();
                BlogWebTicketOneExampleAttachment2.FileContentType = "application/pdf";
            }
        }

        await ticketService.AddTicketAttachmentAsync(BlogWebTicketOne.Id, BlogWebTicketOneExampleAttachment);
        await ticketService.AddTicketAttachmentAsync(BlogWebTicketTwo.Id, BlogWebTicketOneExampleAttachment2);
        await ticketHistoryService.AddAttachmentEventAsync(BlogWebTicketOne, BlogWebTicketOneExampleAttachment);
        await ticketHistoryService.AddAttachmentEventAsync(BlogWebTicketTwo, BlogWebTicketOneExampleAttachment2);

        #endregion
        
        #region AddressBookWebApp
        // Ticket AddressBookTicketOne = context.Tickets.First(t => t.Title == "Logo placement");
        Ticket AddressBookTicketTwo = context.Tickets.First(t => t.Title == "Database swap");
        Ticket AddressBookTicketThree = context.Tickets.First(t => t.Title == "Menu page");
        
        TicketAttachment AddressBookExampleAttachment = new TicketAttachment()
        {
            TicketId = AddressBookTicketTwo.Id,
            UserId = linuxMembers["FredHopkins"],
            Created = AddressBookTicketTwo.Created + new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.png"
        };

        TicketAttachment AddressBookExampleAttachment2 = new TicketAttachment()
        {
            TicketId = AddressBookTicketThree.Id,
            UserId = linuxMembers["CarolSmith"],
            Created = AddressBookTicketThree.Created + new TimeSpan(days: 0, hours: 20, minutes: 9, seconds: 0),
            Description = TextGenerator.Description(),
            FileName = $"{TextGenerator.Title()}.png"
        };

        using (FileStream fs = File.OpenRead($"wwwroot/ExampleAttachment.png"))
        {
            using (var memoryStream = new MemoryStream())
            {
                fs.CopyTo(memoryStream);
                AddressBookExampleAttachment.FileData = memoryStream.ToArray();
                AddressBookExampleAttachment.FileContentType = "application/png";
                AddressBookExampleAttachment2.FileData = memoryStream.ToArray();
                AddressBookExampleAttachment2.FileContentType = "application/png";
            }
        }

        await ticketService.AddTicketAttachmentAsync(AddressBookTicketTwo.Id, AddressBookExampleAttachment);
        await ticketService.AddTicketAttachmentAsync(AddressBookTicketThree.Id, AddressBookExampleAttachment2);
        await ticketHistoryService.AddAttachmentEventAsync(AddressBookTicketTwo, AddressBookExampleAttachment);
        await ticketHistoryService.AddAttachmentEventAsync(AddressBookTicketThree, AddressBookExampleAttachment2);

        #endregion
    }
}