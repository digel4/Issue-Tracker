using System.Timers;
using IssueTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Models.Enums;
using IssueTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;

namespace IssueTracker.Data;

// static means that there is only a single instance of the class in the entire application.
public static class DataUtility
{
    //Company Ids
    // private static int company1Id;
    // private static int company2Id;
    // private static int company3Id;
    // private static int company4Id;
    // private static int company5Id;

    private static System.Timers.Timer dataBaseResetTimer;

    public static string GetConnectionString(IConfiguration configuration)
    {
        //The default connection string will come from appSettings like usual
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        //It will be automatically overwritten if we are running on Heroku
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
#pragma warning disable CS8603
        return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
#pragma warning restore CS8603
    }

    private static string BuildConnectionString(string databaseUrl)
    {
        //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Prefer,
            TrustServerCertificate = true
        };
        return builder.ToString();
    }
    

    public static async Task ManageDataAsync(IHost host)
    {
        using var svcScope = host.Services.CreateScope();
        var svcProvider = svcScope.ServiceProvider;
        //Service: An instance of RoleManager
        var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        //Service: An instance of RoleManager
        var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var projectSvc = svcProvider.GetRequiredService<IProjectService>();
        var companyInfoSvc = svcProvider.GetRequiredService<ICompanyInfoService>();
        var ticketSvc = svcProvider.GetRequiredService<ITicketService>();
        var ticketHistorySvc = svcProvider.GetRequiredService<ITicketHistoryService>();
        //Service: An instance of the UserManager
        var userManagerSvc = svcProvider.GetRequiredService<UserManager<ITUser>>();
        //Migration: This is the programmatic equivalent to Update-Database
        await dbContextSvc.Database.MigrateAsync();


        List<Company> companies = await dbContextSvc.Companies.ToListAsync();
        List<Project> projects = await dbContextSvc.Projects.ToListAsync();
        // List<TicketPriority> ticketPriorities = await dbContextSvc.TicketPriorities.ToListAsync();
        // List<TicketType> ticketTypes = await dbContextSvc.TicketTypes.ToListAsync();
        // List<TicketStatus> ticketStatuses = await dbContextSvc.TicketStatuses.ToListAsync();
        List<Ticket> tickets = await dbContextSvc.Tickets.ToListAsync();
        List<ITUser> users = await dbContextSvc.Users.ToListAsync();
        List<Notification> notifications = await dbContextSvc.Notifications.ToListAsync();
        
        
        dbContextSvc.Notifications.RemoveRange(notifications);
        
        // dbContextSvc.TicketPriorities.RemoveRange(ticketPriorities);
        // dbContextSvc.TicketTypes.RemoveRange(ticketTypes);
        // dbContextSvc.TicketStatuses.RemoveRange(ticketStatuses);
        
        dbContextSvc.Tickets.RemoveRange(tickets);
        dbContextSvc.Projects.RemoveRange(projects);
        dbContextSvc.Users.RemoveRange(users);
        dbContextSvc.Companies.RemoveRange(companies);
        await dbContextSvc.SaveChangesAsync();
        
            // Custom Issue Tracker Seed Methods
            await SeedRolesAsync(roleManagerSvc);
            await SeedDefaultCompanies.SeedDefaultCompaniesAsync(dbContextSvc);

            
            
            
            
            
            
            
            Company? asimovIntelligenceSystemsCompany = dbContextSvc.Companies
                .Include(c => c.Members)
                .FirstOrDefault(p => p.Name == "Asimov Intelligence Systems");
            Company? linuxCompany = dbContextSvc.Companies
                .Include(c => c.Members)
                .FirstOrDefault(p => p.Name == "GNU/Corporation");

            if (asimovIntelligenceSystemsCompany == null)
                throw new ArgumentNullException(nameof(asimovIntelligenceSystemsCompany));
            if (linuxCompany == null)
                throw new ArgumentNullException(nameof(linuxCompany));

            await SeedDefaultUsers.SeedDefaultUsersAsync(userManagerSvc, companyInfoSvc,
                asimovIntelligenceSystemsCompany.Id, linuxCompany.Id);




            // List<ITUser> AsimovIntelligenceSystemsAllMembers = await companyInfoSvc.GetAllMembersAsync(company1Id);
            List<ITUser> asimovIntelligenceSystemsAllMembers = asimovIntelligenceSystemsCompany.Members.ToList();
            List<ITUser> linuxCompanyAllMembers = linuxCompany.Members.ToList();
#pragma warning disable CS8602
            SortedList<string, string> asimovMembers = new SortedList<string, string>()
            {
                {
                    "SusanCalvin", asimovIntelligenceSystemsAllMembers
                        .FirstOrDefault(m => m.FullName == "Susan Calvin").Id
                },
                {

                    "ArronThomas", asimovIntelligenceSystemsAllMembers
                        .FirstOrDefault(m => m.FullName == "Arron Thomas").Id

                },
                {
                    "MathewJacobs", asimovIntelligenceSystemsAllMembers
                        .FirstOrDefault(m => m.FullName == "Mathew Jacobs").Id
                },
                {
                    "NatashaYobs", asimovIntelligenceSystemsAllMembers
                        .FirstOrDefault(m => m.FullName == "Natasha Yobs").Id
                },
                {
                    "TonyTownsend", asimovIntelligenceSystemsAllMembers
                        .FirstOrDefault(m => m.FullName == "Tony Townsend").Id
                },
                {
                    "ScottApple", asimovIntelligenceSystemsAllMembers
                        .FirstOrDefault(m => m.FullName == "Scott Apple").Id
                }
            };

            SortedList<string, string> linuxMembers = new SortedList<string, string>()
            {
                {
                    "JaneRichards", linuxCompanyAllMembers
                        .FirstOrDefault(m => m.FullName == "Jane Richards").Id
                },
                {
                    "FredHopkins", linuxCompanyAllMembers.FirstOrDefault(m => m.FullName == "Fred Hopkins")
                        .Id
                },
                {
                    "JamesPeters", linuxCompanyAllMembers.FirstOrDefault(m => m.FullName == "James Peters")
                        .Id
                },
                {
                    "CarolSmith", linuxCompanyAllMembers.FirstOrDefault(m => m.FullName == "Carol Smith")
                        .Id
                },
                {
                    "BruceTurner", linuxCompanyAllMembers.FirstOrDefault(m => m.FullName == "Bruce Turner")
                        .Id

                },
                {
                    "SueLincoln", linuxCompanyAllMembers.FirstOrDefault(m => m.FullName == "Sue Lincoln")
                        .Id

                }
            };
#pragma warning restore CS8602

            // await SeedDemoUsersAsync(userManagerSvc);
            await SeedDefaultTicketTypeAsync(dbContextSvc);
            await SeedDefaultTicketStatusAsync(dbContextSvc);
            await SeedDefaultTicketPriorityAsync(dbContextSvc);
            await SeedDefaultProjectPriorityAsync(dbContextSvc);
            await SeedDefaultProjects.SeedDefaultProjectsAsync(dbContextSvc, projectSvc, companyInfoSvc,
                asimovIntelligenceSystemsCompany.Id,
                linuxCompany.Id);
            await SeedDefaultTickets.SeedDefaultTicketsAsync(dbContextSvc, projectSvc, companyInfoSvc, ticketSvc,
                ticketHistorySvc, asimovIntelligenceSystemsCompany.Id, linuxCompany.Id, asimovMembers, linuxMembers);
        
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        //Seed Roles
        await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
    }

    private static async Task SeedDefaultProjectPriorityAsync(ApplicationDbContext context)
    {
        try
        {
            IList<ProjectPriority> projectPriorities = new List<ProjectPriority>
            {
                new() { Name = ITProjectPriority.Low.ToString() },
                new() { Name = ITProjectPriority.Medium.ToString() },
                new() { Name = ITProjectPriority.High.ToString() },
                new() { Name = ITProjectPriority.Urgent.ToString() }
            };

            var dbProjectPriorities = context.ProjectPriorities.Select(c => c.Name).ToList();
            await context.ProjectPriorities.AddRangeAsync(projectPriorities.Where(c =>
                !dbProjectPriorities.Contains(c.Name)));
            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding Project Priorities.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }

    private static async Task SeedDefaultTicketTypeAsync(ApplicationDbContext context)
    {
        try
        {
            IList<TicketType> ticketTypes = new List<TicketType>
            {
                new()
                {
                    Name = ITTicketType.NewDevelopment.ToString()
                }, // Ticket involves development of a new, un coded solution 
                new()
                {
                    Name = ITTicketType.WorkTask.ToString()
                }, // Ticket involves development of the specific ticket description 
                new()
                {
                    Name = ITTicketType.Defect.ToString()
                }, // Ticket involves unexpected development/maintenance on a previously designed feature/functionality
                new()
                {
                    Name = ITTicketType.ChangeRequest.ToString()
                }, // Ticket involves modification development of a previously designed feature/functionality
                new()
                {
                    Name = ITTicketType.Enhancement.ToString()
                }, // Ticket involves additional development on a previously designed feature or new functionality
                new()
                {
                    Name = ITTicketType.GeneralTask.ToString()
                } // Ticket involves no software development but may involve tasks such as configuration, or hardware setup
            };

            var dbTicketTypes = context.TicketTypes.Select(c => c.Name).ToList();
            await context.TicketTypes.AddRangeAsync(ticketTypes.Where(c => !dbTicketTypes.Contains(c.Name)));
            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding Ticket Types.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }

    private static async Task SeedDefaultTicketStatusAsync(ApplicationDbContext context)
    {
        try
        {
            IList<TicketStatus> ticketStatuses = new List<TicketStatus>
            {
                new()
                    { Name = ITTicketStatus.New.ToString() }, // Newly Created ticket having never been assigned
                new()
                    { Name = ITTicketStatus.Development.ToString() }, // Ticket is assigned and currently being worked 
                new()
                    { Name = ITTicketStatus.Testing.ToString() }, // Ticket is assigned and is currently being tested
                new()
                {
                    Name = ITTicketStatus.Resolved.ToString()
                }, // Ticket remains assigned to the developer but work in now complete
                new()
                {
                    Name = ITTicketStatus.Unassigned.ToString()
                } // Ticket remains assigned to the developer but work in now complete
            };

            var dbTicketStatuses = context.TicketStatuses.Select(c => c.Name).ToList();
            await context.TicketStatuses.AddRangeAsync(ticketStatuses.Where(c => !dbTicketStatuses.Contains(c.Name)));
            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding Ticket Statuses.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }

    private static async Task SeedDefaultTicketPriorityAsync(ApplicationDbContext context)
    {
        try
        {
            IList<TicketPriority> ticketPriorities = new List<TicketPriority>()
            {
                new() { Name = ITTicketPriority.Low.ToString() },
                new() { Name = ITTicketPriority.Medium.ToString() },
                new() { Name = ITTicketPriority.High.ToString() },
                new() { Name = ITTicketPriority.Urgent.ToString() },
            };

            var dbTicketPriorities = context.TicketPriorities.Select(c => c.Name).ToList();
            await context.TicketPriorities.AddRangeAsync(ticketPriorities.Where(c =>
                !dbTicketPriorities.Contains(c.Name)));
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding Ticket Priorities.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }
}