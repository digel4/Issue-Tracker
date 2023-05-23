using System.Collections;
using IssueTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Models.Enums;
using IssueTracker.Services.Interfaces;
using Npgsql;

namespace IssueTracker.Data;

// static means that there is only a single instance of the class in the entire application.
public static class DataUtility
{
    //Company Ids
        private static int company1Id;
        private static int company2Id;
        private static int company3Id;
        private static int company4Id;
        private static int company5Id;
        
        public static string GetConnectionString(IConfiguration configuration)
        {
            //The default connection string will come from appSettings like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            //It will be automatically overwritten if we are running on Heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }
        
        public static string BuildConnectionString(string databaseUrl)
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

            if (dbContextSvc.Companies.FirstOrDefault(p => p.Name == "Asimov Intelligence Systems") == null)
            {


                // Custom Issue Tracker Seed Methods
                await SeedRolesAsync(roleManagerSvc);
                await SeedDefaultCompanies.SeedDefaultCompaniesAsync(dbContextSvc);

                int company1Id = dbContextSvc.Companies.FirstOrDefault(p => p.Name == "Asimov Intelligence Systems").Id;
                int company2Id = dbContextSvc.Companies.FirstOrDefault(p => p.Name == "GNU/Corporation").Id;

                await SeedDefaultUsers.SeedDefaultUsersAsync(userManagerSvc, companyInfoSvc, company1Id, company2Id);




                List<ITUser> AsimovIntelligenceSystemsAllMembers = await companyInfoSvc.GetAllMembersAsync(company1Id);

                //PMs
                string SusanCalvin = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Susan Calvin")
                    .FirstOrDefault().Id;
                string ArronThomas = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Arron Thomas")
                    .FirstOrDefault().Id;

                //Developers
                string MathewJacobs = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Mathew Jacobs")
                    .FirstOrDefault().Id;
                string NatashaYobs = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Natasha Yobs")
                    .FirstOrDefault().Id;
                string TonyTownsend = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Tony Townsend")
                    .FirstOrDefault().Id;

                //Submitters
                string ScottApple = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Scott Apple")
                    .FirstOrDefault().Id;

                SortedList<string, string> asimovMembers = new SortedList<string, string>()
                {
                    {
                        "SusanCalvin", SusanCalvin
                    },
                    {
                        "ArronThomas", ArronThomas
                    },
                    {
                        "MathewJacobs", MathewJacobs
                    },
                    {
                        "NatashaYobs", NatashaYobs
                    },
                    {
                        "TonyTownsend", TonyTownsend
                    },
                    {
                        "ScottApple", ScottApple
                    }
                };

                List<ITUser> GNUCorporationAllMembers = await companyInfoSvc.GetAllMembersAsync(company2Id);

                //PMs
                string JaneRichards = GNUCorporationAllMembers.Where(m => m.FullName == "Jane Richards")
                    .FirstOrDefault().Id;
                string FredHopkins = GNUCorporationAllMembers.Where(m => m.FullName == "Fred Hopkins").FirstOrDefault()
                    .Id;

                //Developers
                string JamesPeters = GNUCorporationAllMembers.Where(m => m.FullName == "James Peters").FirstOrDefault()
                    .Id;
                string CarolSmith = GNUCorporationAllMembers.Where(m => m.FullName == "Carol Smith").FirstOrDefault()
                    .Id;
                string BruceTurner = GNUCorporationAllMembers.Where(m => m.FullName == "Bruce Turner").FirstOrDefault()
                    .Id;

                //Submitters
                string SueLincoln = GNUCorporationAllMembers.Where(m => m.FullName == "Sue Lincoln").FirstOrDefault()
                    .Id;

                SortedList<string, string> linuxMembers = new SortedList<string, string>()
                {
                    {
                        "JaneRichards", JaneRichards
                    },
                    {
                        "FredHopkins", FredHopkins
                    },
                    {
                        "JamesPeters", JamesPeters
                    },
                    {
                        "CarolSmith", CarolSmith
                    },
                    {
                        "BruceTurner", BruceTurner
                    },
                    {
                        "SueLincoln", SueLincoln
                    }
                };


                // await SeedDemoUsersAsync(userManagerSvc);
                await SeedDefaultTicketTypeAsync(dbContextSvc);
                await SeedDefaultTicketStatusAsync(dbContextSvc);
                await SeedDefaultTicketPriorityAsync(dbContextSvc);
                await SeedDefaultProjectPriorityAsync(dbContextSvc);
                await SeedDefaultProjects.SeedDefaultProjectsAsync(dbContextSvc, projectSvc, companyInfoSvc,  company1Id,
                    company2Id);
                await SeedDefaultTickets.SeedDefautTicketsAsync(dbContextSvc, projectSvc, companyInfoSvc, ticketSvc,
                    ticketHistorySvc, company1Id, company2Id, asimovMembers, linuxMembers);
            }
        }

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
            // await roleManager.CreateAsync(new IdentityRole(Roles.DemoUser.ToString()));
        }
        
        public static async Task SeedDefaultProjectPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Models.ProjectPriority> projectPriorities = new List<ProjectPriority>() {
                    new ProjectPriority() { Name = ITProjectPriority.Low.ToString() },
                    new ProjectPriority() { Name = ITProjectPriority.Medium.ToString() },
                    new ProjectPriority() { Name = ITProjectPriority.High.ToString() },
                    new ProjectPriority() { Name = ITProjectPriority.Urgent.ToString() },
                };

                var dbProjectPriorities = context.ProjectPriorities.Select(c => c.Name).ToList();
                await context.ProjectPriorities.AddRangeAsync(projectPriorities.Where(c => !dbProjectPriorities.Contains(c.Name)));
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
        
                public static async Task SeedDefaultTicketTypeAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketType> ticketTypes = new List<TicketType>() {
                     new TicketType() { Name = ITTicketType.NewDevelopment.ToString() },      // Ticket involves development of a new, uncoded solution 
                     new TicketType() { Name = ITTicketType.WorkTask.ToString() },            // Ticket involves development of the specific ticket description 
                     new TicketType() { Name = ITTicketType.Defect.ToString()},               // Ticket involves unexpected development/maintenance on a previously designed feature/functionality
                     new TicketType() { Name = ITTicketType.ChangeRequest.ToString() },       // Ticket involves modification development of a previously designed feature/functionality
                     new TicketType() { Name = ITTicketType.Enhancement.ToString() },         // Ticket involves additional development on a previously designed feature or new functionality
                     new TicketType() { Name = ITTicketType.GeneralTask.ToString() }          // Ticket involves no software development but may involve tasks such as configuations, or hardware setup
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

        public static async Task SeedDefaultTicketStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketStatus> ticketStatuses = new List<TicketStatus>() {
                    new TicketStatus() { Name = ITTicketStatus.New.ToString() },                 // Newly Created ticket having never been assigned
                    new TicketStatus() { Name = ITTicketStatus.Development.ToString() },         // Ticket is assigned and currently being worked 
                    new TicketStatus() { Name = ITTicketStatus.Testing.ToString()  },            // Ticket is assigned and is currently being tested
                    new TicketStatus() { Name = ITTicketStatus.Resolved.ToString()  },           // Ticket remains assigned to the developer but work in now complete
                    new TicketStatus() { Name = ITTicketStatus.Unassigned.ToString()  },           // Ticket remains assigned to the developer but work in now complete
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

        public static async Task SeedDefaultTicketPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketPriority> ticketPriorities = new List<TicketPriority>() {
                                                    new TicketPriority() { Name = ITTicketPriority.Low.ToString()  },
                                                    new TicketPriority() { Name = ITTicketPriority.Medium.ToString() },
                                                    new TicketPriority() { Name = ITTicketPriority.High.ToString()},
                                                    new TicketPriority() { Name = ITTicketPriority.Urgent.ToString()},
                };

                var dbTicketPriorities = context.TicketPriorities.Select(c => c.Name).ToList();
                await context.TicketPriorities.AddRangeAsync(ticketPriorities.Where(c => !dbTicketPriorities.Contains(c.Name)));
                context.SaveChanges();

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
        
        
        

        // public static async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
        // {
        //     try
        //     {
        //         IList<Company> defaultcompanies = new List<Company>() {
        //             new Company() { Name = "Asimov Intelligence Systems", Description="The three laws of robotics. Now with Zeroth Law!" },
        //             new Company() { Name = "GNU/Corporation", Description="Technical means to a social end" }
        //             // new Company() { Name = "Company3", Description="This is default Company 3" },
        //             // new Company() { Name = "Company4", Description="This is default Company 4" },
        //             // new Company() { Name = "Company5", Description="This is default Company 5" }
        //         };
        //
        //         var dbCompanies = context.Companies.Select(c => c.Name).ToList();
        //         await context.Companies.AddRangeAsync(defaultcompanies.Where(c => !dbCompanies.Contains(c.Name)));
        //         await context.SaveChangesAsync();
        //
        //         //Get company Ids
        //         // company1Id = context.Companies.FirstOrDefault(p => p.Name == "Asimov Intelligence Systems").Id;
        //         // company2Id = context.Companies.FirstOrDefault(p => p.Name == "GNU/Corporation").Id;
        //         // company3Id = context.Companies.FirstOrDefault(p => p.Name == "Company3").Id;
        //         // company4Id = context.Companies.FirstOrDefault(p => p.Name == "Company4").Id;
        //         // company5Id = context.Companies.FirstOrDefault(p => p.Name == "Company5").Id;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Companies.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        // }



        // public static async Task SeedDefaultProjectsAsync(ApplicationDbContext context, IProjectService projectSvc, ICompanyInfoService companyInfoSvc)
        // {
        //
        //     //Get project priority Ids
        //     int priorityLow = context.ProjectPriorities.FirstOrDefault(p => p.Name == ITProjectPriority.Low.ToString()).Id;
        //     int priorityMedium = context.ProjectPriorities.FirstOrDefault(p => p.Name == ITProjectPriority.Medium.ToString()).Id;
        //     int priorityHigh = context.ProjectPriorities.FirstOrDefault(p => p.Name == ITProjectPriority.High.ToString()).Id;
        //     int priorityUrgent = context.ProjectPriorities.FirstOrDefault(p => p.Name == ITProjectPriority.Urgent.ToString()).Id;
        //     
        //     try
        //     {
        //         IList<Project> projects = new List<Project>() {
        //              new Project()
        //              {
        //                  CompanyId = company1Id,
        //                  Name = "Portfolio Website",
        //                  Description="Single page html, css & javascript page.  Serves as a landing page for potential clients. Site contains a company bio and links to all current applications." ,
        //                  StartDate = DateTime.Now.Subtract( new TimeSpan(20, 0, 0 , 0) ),
        //                  EndDate = DateTime.Now.AddDays(4),
        //                  ProjectPriorityId = priorityMedium
        //              },
        //              new Project()
        //              {
        //                  CompanyId = company1Id,
        //                  Name = "Issue Tracker Web Application",
        //                  Description="A custom designed .Net Core application with postgres database.  The application is a multi tennent application designed to track issue tickets' progress.  Implemented with identity and user roles, Tickets are maintained in projects which are maintained by users in the role of projectmanager.  Each project has a team and team members.",
        //                  // StartDate = new DateTime(2023,5,20),
        //                  StartDate = DateTime.Now.Subtract( new TimeSpan(7, 0, 0 , 0) ),
        //                  EndDate = DateTime.Now.AddDays(12),
        //                  ProjectPriorityId = priorityHigh
        //              },
        //              new Project()
        //              {
        //                  CompanyId = company1Id,
        //                  Name = "Movie Information Web Application",
        //                  Description="A custom designed .Net Core application with postgres database.  An API based application allows users to input and import movie posters and details including cast and crew information. Acts as a testbed to see which movie is most favoured by AGI",
        //                  StartDate = DateTime.Now.Subtract( new TimeSpan(3, 0, 0 , 0) ),
        //                  EndDate = DateTime.Now.AddDays(20),
        //                  ProjectPriorityId = priorityHigh
        //              },
        //              new Project()
        //              {
        //                  CompanyId = company2Id,
        //                  Name = "Blog Web Application",
        //                  Description="Candidate's custom built web application using .Net Core with MVC, a postgres database and hosted in a heroku container.  The app is designed for CEO Richard Stallman so he can all out inappropriate uses of the name Linux rather than the correct GNU/Linux.",
        //                  StartDate = DateTime.Now.Subtract( new TimeSpan(26, 0, 0 , 0) ),
        //                  EndDate = DateTime.Now.AddDays(10),
        //                  ProjectPriorityId = priorityUrgent,
        //              },
        //
        //              new Project()
        //              {
        //                  CompanyId = company2Id,
        //                  Name = "Address Book Web Application",
        //                  Description="A custom designed .Net Core application with postgres database.  This is an application to serve as a rolodex of contacts for a given user. Richard Stallman plans to use to pressure world leaders on the dangers around misnaming GNU/Linux in the public sphere",
        //                  StartDate = DateTime.Now.Subtract( new TimeSpan(18, 0, 0 , 0) ),
        //                  EndDate = DateTime.Now.AddDays(2),
        //                  ProjectPriorityId = priorityLow
        //              }
        //
        //         };
        //         // Add Images to Project:
        //         try
        //         {
        //             foreach (Project project in projects)
        //             {
        //                 if (context.Projects.Where(p =>
        //                         p.Name == project.Name
        //                     ).FirstOrDefault() == null)
        //                 {
        //                     await using (FileStream fs = File.OpenRead($"wwwroot/img/ProjectPics/{project.Name}.jpg"))
        //                     {
        //                         using (var memoryStream = new MemoryStream())
        //                         {
        //                             fs.CopyTo(memoryStream);
        //                             project.FileData = memoryStream.ToArray();
        //                             project.FileContentType = "image/jpg";
        //                             project.FileName = "projectImage";
        //                         }
        //
        //                     }
        //
        //                     context.Projects.Add(project);
        //                     await context.SaveChangesAsync();
        //
        //                     foreach (var companyProject in await companyInfoSvc.GetAllProjectsAsync(company1Id))
        //                     {
        //                         if (companyProject.Name == "Portfolio Website")
        //                         {
        //                             List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company1Id);
        //
        //                             ITUser projectManager = members.Where(m => m.FullName == "Susan Calvin")
        //                                 .FirstOrDefault();
        //
        //                             List<ITUser> projectMembers = members.Where(m =>
        //                                 m.FullName == "Susan Calvin"
        //                                 || m.FullName == "Mathew Jacobs"
        //                                 || m.FullName == "Tony Townsend"
        //                             ).ToList();
        //
        //                             foreach (var member in projectMembers)
        //                             {
        //                                 await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
        //                             }
        //
        //                             await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);
        //                         }
        //
        //                         if (companyProject.Name == "Issue Tracker Web Application")
        //                         {
        //                             List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company1Id);
        //
        //                             ITUser projectManager = members.Where(m => m.FullName == "Arron Thomas")
        //                                 .FirstOrDefault();
        //
        //                             List<ITUser> projectMembers = members.Where(m =>
        //                                 m.FullName == "Arron Thomas"
        //                                 || m.FullName == "Natasha Yobs"
        //                                 || m.FullName == "Tony Townsend"
        //                             ).ToList();
        //
        //                             foreach (var member in projectMembers)
        //                             {
        //                                 await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
        //                             }
        //
        //                             await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);
        //                         }
        //
        //                         if (companyProject.Name == "Movie Information Web Application")
        //                         {
        //                             List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company1Id);
        //
        //                             ITUser projectManager = members.Where(m => m.FullName == "Susan Calvin")
        //                                 .FirstOrDefault();
        //
        //                             List<ITUser> projectMembers = members.Where(m =>
        //                                 m.FullName == "Susan Calvin"
        //                                 || m.FullName == "Mathew Jacobs"
        //                                 || m.FullName == "Tony Townsend"
        //                             ).ToList();
        //
        //                             foreach (var member in projectMembers)
        //                             {
        //                                 await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
        //                             }
        //
        //                             await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);
        //                         }
        //                     }
        //
        //
        //                     foreach (var companyProject in await companyInfoSvc.GetAllProjectsAsync(company2Id))
        //                     {
        //                         if (companyProject.Name == "Blog Web Application")
        //                         {
        //                             List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company2Id);
        //
        //                             ITUser projectManager = members.Where(m => m.FullName == "Jane Richards")
        //                                 .FirstOrDefault();
        //
        //                             List<ITUser> projectMembers = members.Where(m =>
        //                                 m.FullName == "Jane Richards"
        //                                 || m.FullName == "James Peters"
        //                                 || m.FullName == "Bruce Turner"
        //                             ).ToList();
        //
        //                             foreach (var member in projectMembers)
        //                             {
        //                                 await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
        //                             }
        //
        //                             await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);
        //                         }
        //
        //                         if (companyProject.Name == "Address Book Web Application")
        //                         {
        //                             List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company2Id);
        //
        //                             ITUser projectManager = members.Where(m => m.FullName == "Fred Hopkins")
        //                                 .FirstOrDefault();
        //
        //                             List<ITUser> projectMembers = members.Where(m =>
        //                                 m.FullName == "Fred Hopkins"
        //                                 || m.FullName == "Carol Smith"
        //                                 || m.FullName == "James Peters"
        //                             ).ToList();
        //
        //                             foreach (var member in projectMembers)
        //                             {
        //                                 await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
        //                             }
        //
        //                             await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);
        //
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //             catch (Exception)
        //             {
        //                 throw;
        //             }
        //
        //
        //
        //             var dbProjects = context.Projects.Select(c => c.Name).ToList();
        //             await context.Projects.AddRangeAsync(projects.Where(c => !dbProjects.Contains(c.Name)));
        //             await context.SaveChangesAsync();
        //
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Projects.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //     
        //     
        // }
        
        // public static async Task SeedDefaultUsersAsync(UserManager<ITUser> userManager)
        // {
        //     //Seed Default Admin User
        //     var defaultUser = new ITUser
        //     {
        //         UserName = "IsaacAsimov@AsimovIntelligenceSystems.com",
        //         Email = "IsaacAsimov@AsimovIntelligenceSystems.com",
        //         FirstName = "Isaac",
        //         LastName = "Asimov",
        //         EmailConfirmed = true,
        //         CompanyId = company1Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Admin User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //     //Seed Default Admin User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "RichardStallman@GNUCorporation.com",
        //         Email = "RichardStallman@GNUCorporation.com",
        //         FirstName = "Richard",
        //         LastName = "Stallman",
        //         EmailConfirmed = true,
        //         CompanyId = company2Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Admin User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Default ProjectManager1 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "SusanCalvin@AsimovIntelligenceSystems.com",
        //         Email = "SusanCalvin@AsimovIntelligenceSystems.com",
        //         FirstName = "Susan",
        //         LastName = "Calvin",
        //         EmailConfirmed = true,
        //         CompanyId = company1Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //
        //             await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default ProjectManager1 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Default ProjectManager2 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "JaneRichards@GNUCorporation.com",
        //         Email = "JaneRichards@GNUCorporation.com",
        //         FirstName = "Jane",
        //         LastName = "Richards",
        //         EmailConfirmed = true,
        //         CompanyId = company2Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default ProjectManager2 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //     
        //                 //Seed Default ProjectManager3 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "ArronThomas@AsimovIntelligenceSystems.com",
        //         Email = "ArronThomas@AsimovIntelligenceSystems.com",
        //         FirstName = "Arron",
        //         LastName = "Thomas",
        //         EmailConfirmed = true,
        //         CompanyId = company1Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //
        //             await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default ProjectManager1 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Default ProjectManager4 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "FredHopkins@GNUCorporation.com",
        //         Email = "FredHopkins@GNUCorporation.com",
        //         FirstName = "Fred",
        //         LastName = "Hopkins",
        //         EmailConfirmed = true,
        //         CompanyId = company2Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default ProjectManager2 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Default Developer1 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "MathewJacobs@AsimovIntelligenceSystems.com",
        //         Email = "MathewJacobs@AsimovIntelligenceSystems.com",
        //         FirstName = "Mathew",
        //         LastName = "Jacobs",
        //         EmailConfirmed = true,
        //         CompanyId = company1Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Developer1 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Default Developer2 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "JamesPeters@GNUCorporation.com",
        //         Email = "JamesPeters@GNUCorporation.com",
        //         FirstName = "James",
        //         LastName = "Peters",
        //         EmailConfirmed = true,
        //         CompanyId = company2Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Developer2 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Default Developer3 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "NatashaYobs@AsimovIntelligenceSystems.com",
        //         Email = "NatashaYobs@AsimovIntelligenceSystems.com",
        //         FirstName = "Natasha",
        //         LastName = "Yobs",
        //         EmailConfirmed = true,
        //         CompanyId = company1Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Developer3 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Default Developer4 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "CarolSmith@GNUCorporation.com",
        //         Email = "CarolSmith@GNUCorporation.com",
        //         FirstName = "Carol",
        //         LastName = "Smith",
        //         EmailConfirmed = true,
        //         CompanyId = company2Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Developer4 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Default Developer5 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "TonyTownsend@AsimovIntelligenceSystems.com",
        //         Email = "TonyTownsend@AsimovIntelligenceSystems.com",
        //         FirstName = "Tony",
        //         LastName = "Townsend",
        //         EmailConfirmed = true,
        //         CompanyId = company1Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Developer5 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //     //Seed Default Developer6 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "BruceTurner@GNUCorporation.com",
        //         Email = "BruceTurner@GNUCorporation.com",
        //         FirstName = "Bruce",
        //         LastName = "Turner",
        //         EmailConfirmed = true,
        //         CompanyId = company2Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Developer5 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //     //Seed Default Submitter1 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "ScottApple@AsimovIntelligenceSystems.com",
        //         Email = "ScottApple@AsimovIntelligenceSystems.com",
        //         FirstName = "Scott",
        //         LastName = "Apple",
        //         EmailConfirmed = true,
        //         CompanyId = company1Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Submitter1 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Default Submitter2 User
        //     defaultUser = new ITUser
        //     {
        //         UserName = "SueLincoln@GNUCorporation.com",
        //         Email = "SueLincoln@GNUCorporation.com",
        //         FirstName = "Sue",
        //         LastName = "Lincoln",
        //         EmailConfirmed = true,
        //         CompanyId = company2Id
        //     };
        //     try
        //     {
        //         await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
        //         {
        //             using (var memoryStream = new MemoryStream())
        //             {
        //                 fs.CopyTo(memoryStream);
        //                 defaultUser.AvatarFileData = memoryStream.ToArray();
        //                 defaultUser.AvatarContentType = "image/jpg";
        //                 defaultUser.AvatarFileName = "profileImage";
        //             }
        //
        //         }
        //         
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Default Submitter2 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        // }
    


    
        // public static async Task SeedDefautTicketsAsync(ApplicationDbContext context, IProjectService projectSvc, ICompanyInfoService companyInfoSvc)
        // {
        //     //Get project Ids
        //     int portfolioId = context.Projects.FirstOrDefault(p => p.Name == "Portfolio Website").Id;
        //     int blogId = context.Projects.FirstOrDefault(p => p.Name == "Blog Web Application").Id;
        //     int bugtrackerId = context.Projects.FirstOrDefault(p => p.Name == "Issue Tracker Web Application").Id;
        //     int movieId = context.Projects.FirstOrDefault(p => p.Name == "Movie Information Web Application").Id;
        //     int addressBookId = context.Projects.FirstOrDefault(p => p.Name == "Address Book Web Application").Id;
        //
        //     //Get ticket type Ids
        //     int typeNewDev = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.NewDevelopment.ToString()).Id;
        //     int typeWorkTask = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.WorkTask.ToString()).Id;
        //     int typeDefect = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.Defect.ToString()).Id;
        //     int typeEnhancement = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.Enhancement.ToString()).Id;
        //     int typeChangeRequest = context.TicketTypes.FirstOrDefault(p => p.Name == ITTicketType.ChangeRequest.ToString()).Id;
        //
        //     //Get ticket priority Ids
        //     int priorityLow = context.TicketPriorities.FirstOrDefault(p => p.Name == ITTicketPriority.Low.ToString()).Id;
        //     int priorityMedium = context.TicketPriorities.FirstOrDefault(p => p.Name == ITTicketPriority.Medium.ToString()).Id;
        //     int priorityHigh = context.TicketPriorities.FirstOrDefault(p => p.Name == ITTicketPriority.High.ToString()).Id;
        //     int priorityUrgent = context.TicketPriorities.FirstOrDefault(p => p.Name == ITTicketPriority.Urgent.ToString()).Id;
        //
        //     //Get ticket status Ids
        //     int statusNew = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.New.ToString()).Id;
        //     int statusDev = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.Development.ToString()).Id;
        //     int statusTest = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.Testing.ToString()).Id;
        //     int statusResolved = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.Resolved.ToString()).Id;
        //     int statusUnassigned = context.TicketStatuses.FirstOrDefault(p => p.Name == ITTicketStatus.Unassigned.ToString()).Id;
        //
        //
        //     try
        //     {
        //         List<ITUser> GNUCorporationAllMembers = await companyInfoSvc.GetAllMembersAsync(company2Id);
        //         List<ITUser> AsimovIntelligenceSystemsAllMembers = await companyInfoSvc.GetAllMembersAsync(company1Id);
        //         
        //         #region Asimov Members
        //         //PMs
        //         string SusanCalvin = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Susan Calvin").FirstOrDefault().Id;
        //         string ArronThomas = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Arron Thomas").FirstOrDefault().Id;
        //         
        //         //Developers
        //         string MathewJacobs = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Mathew Jacobs").FirstOrDefault().Id;
        //         string NatashaYobs = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Natasha Yobs").FirstOrDefault().Id;
        //         string TonyTownsend = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Tony Townsend").FirstOrDefault().Id;
        //         
        //         //Submitters
        //         string ScottApple = AsimovIntelligenceSystemsAllMembers.Where(m => m.FullName == "Scott Apple").FirstOrDefault().Id;
        //         
        //         #endregion
        //         
        //         #region GNU Members
        //         //PMs
        //         string JaneRichards = GNUCorporationAllMembers.Where(m => m.FullName == "Jane Richards").FirstOrDefault().Id;
        //         string FredHopkins = GNUCorporationAllMembers.Where(m => m.FullName == "Fred Hopkins").FirstOrDefault().Id;
        //             
        //         //Developers
        //         string JamesPeters = GNUCorporationAllMembers.Where(m => m.FullName == "James Peters").FirstOrDefault().Id;
        //         string CarolSmith = GNUCorporationAllMembers.Where(m => m.FullName == "Carol Smith").FirstOrDefault().Id;
        //         string BruceTurner = GNUCorporationAllMembers.Where(m => m.FullName == "Bruce Turner").FirstOrDefault().Id;
        //         
        //         //Submitters
        //         string SueLincoln = GNUCorporationAllMembers.Where(m => m.FullName == "Sue Lincoln").FirstOrDefault().Id;
        //         
        //         #endregion
        //
        //         IList<Ticket> tickets = new List<Ticket>() {
        //             
        //                         #region Portfolio Tickets
        //                         
        //                         #region Ticket 1
        //                         new Ticket()
        //                         {
        //                             Title = "Fix Styling to Email Form", 
        //                             Description = "Fix styling so it fits in the container", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = portfolioId,
        //                             OwnerUserId = SusanCalvin,
        //                             TicketPriorityId = priorityLow, 
        //                             TicketStatusId = statusUnassigned, 
        //                             TicketTypeId = typeNewDev
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 2
        //                         new Ticket()
        //                         {
        //                             Title = "Nav Bar Not Working on Edge Browser", 
        //                             Description = "Navbar works on all browsers apart from Edge. I guess it's an edge case.", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = portfolioId,
        //                             OwnerUserId = SueLincoln,
        //                             DeveloperUserId = MathewJacobs,
        //                             TicketPriorityId = priorityMedium, 
        //                             TicketStatusId = statusResolved,
        //                             Archived = true,
        //                             TicketTypeId = typeChangeRequest
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 3
        //                         new Ticket()
        //                         {
        //                             Title = "Change Styling to SaSS", 
        //                             Description = "We want to move away from basic CSS and use SaSS from now on", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = portfolioId,
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev, 
        //                             TicketTypeId = typeEnhancement,
        //                             DeveloperUserId = TonyTownsend,
        //                             OwnerUserId = SueLincoln,
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 4
        //                         new Ticket() {
        //                             Title = "Change Images to PNG", 
        //                             Description = "Change images to PNG to speed up site load", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = portfolioId, 
        //                             TicketPriorityId = priorityUrgent,
        //                             TicketStatusId = statusTest, 
        //                             TicketTypeId = typeDefect,
        //                             DeveloperUserId = MathewJacobs,
        //                             OwnerUserId = SusanCalvin
        //                         },
        //                         
        //                         #endregion
        //                          
        //                         #region Ticket 5
        //                         new Ticket()
        //                         {
        //                             Title = "Make Navbar Responsive", 
        //                             Description = "Navbar doesn't change size when the website is loaded on smaller handheld devices", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = portfolioId, 
        //                             TicketPriorityId = priorityLow, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeNewDev,
        //                             DeveloperUserId = MathewJacobs,
        //                             OwnerUserId = SusanCalvin
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 6
        //                         new Ticket()
        //                         {
        //                             Title = "Add Current Time on Navbar", 
        //                             Description = "Can we add a link in the nav bar to a new page that shows the current time?", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = portfolioId, 
        //                             TicketPriorityId = priorityMedium, 
        //                             TicketStatusId = statusResolved, 
        //                             TicketTypeId = typeChangeRequest,
        //                             DeveloperUserId = MathewJacobs,
        //                             OwnerUserId = SusanCalvin
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 7
        //                         new Ticket()
        //                         {
        //                             Title = "Add New contact info", 
        //                             Description = "See the attachment, can we please update the 'Contact Us' page with this new information?" , 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = portfolioId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev, 
        //                             TicketTypeId = typeEnhancement,
        //                             DeveloperUserId = TonyTownsend,
        //                             OwnerUserId = SusanCalvin,
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 8
        //                         new Ticket()
        //                         {
        //                             Title = "Add New UI Colours", 
        //                             Description = "The UI colors need to be updated per the attached list.", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = portfolioId, 
        //                             TicketPriorityId = priorityUrgent, 
        //                             TicketStatusId = statusTest, 
        //                             TicketTypeId = typeDefect,
        //                             DeveloperUserId = TonyTownsend,
        //                             OwnerUserId = SusanCalvin,
        //                         },
        //                         #endregion
        //                         
        //                         #endregion
        //                         
        //                         #region Blog Tickets
        //
        //                             #region Ticket 1
        //                             new Ticket() {Title = "Blog Ticket 1", Description = "Ticket details for blog ticket 1", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeDefect},
        //                             #endregion
        //                             
        //                             #region Ticket 2
        //                             new Ticket() {Title = "Blog Ticket 2", Description = "Ticket details for blog ticket 2", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev, TicketTypeId = typeEnhancement},
        //                             #endregion
        //
        //                             #region Ticket 3
        //                             new Ticket() {Title = "Blog Ticket 3", Description = "Ticket details for blog ticket 3", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeChangeRequest},
        //                             #endregion
        //                             
        //                             #region Ticket 4
        //                             new Ticket() {Title = "Blog Ticket 4", Description = "Ticket details for blog ticket 4", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
        //                             #endregion
        //                             
        //                             #region Ticket 5
        //                             new Ticket() {Title = "Blog Ticket 5", Description = "Ticket details for blog ticket 5", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusDev,  TicketTypeId = typeDefect},
        //                             #endregion
        //                             
        //                             #region Ticket 6
        //                             new Ticket() {Title = "Blog Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusTest,  TicketTypeId = typeEnhancement},
        //                             #endregion
        //                             
        //                             #region Ticket 7
        //                             new Ticket() {Title = "Blog Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusResolved, TicketTypeId = typeChangeRequest},
        //                             #endregion
        //                             
        //                             #region Ticket 8
        //                             new Ticket() {Title = "Blog Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusDev,  TicketTypeId = typeWorkTask},
        //                             #endregion
        //                             
        //                             #region Ticket 9
        //                             new Ticket() {Title = "Blog Ticket 9", Description = "Ticket details for blog ticket 9", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusNew,  TicketTypeId = typeDefect},
        //                             #endregion
        //                             
        //                             #region Ticket 10
        //                             new Ticket()
        //                             {
        //                                 Title = "Blog Ticket 10", 
        //                                 Description = "Ticket details for blog ticket 10", 
        //                                 Created = DateTimeOffset.Now, 
        //                                 ProjectId = blogId, 
        //                                 TicketPriorityId = priorityMedium, 
        //                                 TicketStatusId = statusNew, 
        //                                 TicketTypeId = typeEnhancement
        //                             },
        //                             #endregion
        //                             
        //                             #region Ticket 11
        //                             new Ticket()
        //                             {
        //                                 Title = "Blog Ticket 11", 
        //                                 Description = "Ticket details for blog ticket 11", 
        //                                 Created = DateTimeOffset.Now, 
        //                                 ProjectId = blogId, 
        //                                 TicketPriorityId = priorityHigh, 
        //                                 TicketStatusId = statusResolved,  
        //                                 TicketTypeId = typeChangeRequest
        //                             },
        //                             #endregion
        //                             
        //                             #region Ticket 12
        //                             new Ticket()
        //                             {
        //                                 Title = "Blog Ticket 12", 
        //                                 Description = "Ticket details for blog ticket 12", 
        //                                 Created = DateTimeOffset.Now, 
        //                                 ProjectId = blogId, 
        //                                 TicketPriorityId = priorityUrgent, 
        //                                 TicketStatusId = statusNew,  
        //                                 TicketTypeId = typeWorkTask
        //                             },
        //                             #endregion
        //                             
        //                             #region Ticket 13
        //                             new Ticket()
        //                             {
        //                                 Title = "Blog Ticket 13", 
        //                                 Description = "Ticket details for blog ticket 13", 
        //                                 Created = DateTimeOffset.Now, 
        //                                 ProjectId = blogId, 
        //                                 TicketPriorityId = priorityLow, 
        //                                 TicketStatusId = statusNew, 
        //                                 TicketTypeId = typeDefect
        //                             },
        //                             #endregion
        //                             
        //                             #region Ticket 14
        //                             new Ticket()
        //                             {
        //                                 Title = "Blog Ticket 14", 
        //                                 Description = "Ticket details for blog ticket 14", 
        //                                 Created = DateTimeOffset.Now, 
        //                                 ProjectId = blogId, 
        //                                 TicketPriorityId = priorityMedium, 
        //                                 TicketStatusId = statusDev,  
        //                                 TicketTypeId = typeEnhancement
        //                             },
        //                             #endregion
        //                             
        //                             #region Ticket 15
        //                             new Ticket()
        //                             {
        //                                 Title = "Blog Ticket 15", 
        //                                 Description = "Ticket details for blog ticket 15", 
        //                                 Created = DateTimeOffset.Now, 
        //                                 ProjectId = blogId, 
        //                                 TicketPriorityId = priorityHigh, 
        //                                 TicketStatusId = statusTest,  
        //                                 TicketTypeId = typeChangeRequest
        //                             },
        //                             #endregion
        //                             
        //                             #region Ticket 16
        //                             new Ticket()
        //                             {
        //                                 Title = "Blog Ticket 16", 
        //                                 Description = "Ticket details for blog ticket 16", 
        //                                 Created = DateTimeOffset.Now, 
        //                                 ProjectId = blogId, 
        //                                 TicketPriorityId = priorityUrgent, 
        //                                 TicketStatusId = statusNew, 
        //                                 TicketTypeId = typeNewDev
        //                             },
        //                             #endregion
        //                             
        //                             #region Ticket 17
        //                             new Ticket()
        //                             {
        //                                 Title = "Blog Ticket 17", 
        //                                 Description = "Ticket details for blog ticket 17", 
        //                                 Created = DateTimeOffset.Now, 
        //                                 ProjectId = blogId, 
        //                                 TicketPriorityId = priorityHigh, 
        //                                 TicketStatusId = statusResolved,  
        //                                 TicketTypeId = typeNewDev
        //                             },
        //                             #endregion
        //                         
        //                         #endregion
        //                         
        //                         #region Bug Tracker Tickets  
        //                         
        //                         #region Ticket 1                                                                                                         
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 1", 
        //                             Description = "Ticket details for bug tracker ticket 1", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeNewDev
        //                             
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 2
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 2", 
        //                             Description = "Ticket details for bug tracker ticket 2", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeChangeRequest
        //                             
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 3
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 3", 
        //                             Description = "Ticket details for bug tracker ticket 3", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusTest,
        //                              TicketTypeId = typeChangeRequest
        //                              
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 4
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 4", 
        //                             Description = "Ticket details for bug tracker ticket 4", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeDefect
        //                             
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 5
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 5", 
        //                             Description = "Ticket details for bug tracker ticket 5", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusTest,
        //                              TicketTypeId = typeWorkTask
        //                              
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 6
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 6", 
        //                             Description = "Ticket details for bug tracker ticket 6", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeEnhancement
        //                             
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 7
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 7", 
        //                             Description = "Ticket details for bug tracker ticket 7", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusResolved, 
        //                             TicketTypeId = typeWorkTask
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 8
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 8", 
        //                             Description = "Ticket details for bug tracker ticket 8", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeWorkTask
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 9 
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 9", 
        //                             Description = "Ticket details for bug tracker ticket 9", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeWorkTask
        //                             
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 10
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 10", 
        //                             Description = "Ticket details for bug tracker 10", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeNewDev},
        //                             
        //                         #endregion
        //                         
        //                         #region Ticket 11
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 11", 
        //                             Description = "Ticket details for bug tracker 11", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev, 
        //                             TicketTypeId = typeDefect},
        //                             
        //                         #endregion
        //                         
        //                         #region Ticket 12
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 12", 
        //                             Description = "Ticket details for bug tracker 12", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusTest,
        //                              TicketTypeId = typeDefect},
        //                              
        //                         #endregion
        //                         
        //                         #region Ticket 13
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 13", 
        //                             Description = "Ticket details for bug tracker 13", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev, 
        //                             TicketTypeId = typeEnhancement
        //                             
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 14
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 14", 
        //                             Description = "Ticket details for bug tracker 14", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusTest,
        //                              TicketTypeId = typeDefect},
        //                              
        //                         #endregion
        //                         
        //                         #region Ticket 15
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 15", 
        //                             Description = "Ticket details for bug tracker 15", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev, 
        //                             TicketTypeId = typeNewDev},
        //                             
        //                         #endregion
        //                         
        //                         #region Ticket 16
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 16", 
        //                             Description = "Ticket details for bug tracker 16", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeEnhancement
        //                             
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 17
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 17", 
        //                             Description = "Ticket details for bug tracker 17", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusTest,
        //                              TicketTypeId = typeDefect},
        //                              
        //                         #endregion
        //                         
        //                         #region Ticket 18
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 18", 
        //                             Description = "Ticket details for bug tracker 18", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev, 
        //                             TicketTypeId = typeNewDev},
        //                             
        //                         #endregion
        //                         
        //                         #region Ticket 19
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 19", 
        //                             Description = "Ticket details for bug tracker 19", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusResolved, 
        //                             TicketTypeId = typeDefect
        //                             
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 20
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 20", 
        //                             Description = "Ticket details for bug tracker 20", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeEnhancement
        //                             
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 21
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 21", 
        //                             Description = "Ticket details for bug tracker 21", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusResolved, TicketTypeId = typeDefect
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 22
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 22", 
        //                             Description = "Ticket details for bug tracker 22", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev, 
        //                             TicketTypeId = typeNewDev
        //                             
        //                         },
        //                             
        //                         #endregion
        //                         
        //                         #region Ticket 23
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 23", 
        //                             Description = "Ticket details for bug tracker 23", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeNewDev
        //                             
        //                         },
        //                             
        //                         #endregion
        //                         
        //                         #region Ticket 24
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 24", 
        //                             Description = "Ticket details for bug tracker 24", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev, 
        //                             TicketTypeId = typeDefect
        //                             
        //                         },
        //                             
        //                         #endregion
        //                         
        //                         #region Ticket 25
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 25", 
        //                             Description = "Ticket details for bug tracker 25", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev, 
        //                             TicketTypeId = typeChangeRequest
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 26
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 26", 
        //                             Description = "Ticket details for bug tracker 26", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeDefect},
        //                             
        //                         #endregion
        //                         
        //                         #region Ticket 27
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 27", 
        //                             Description = "Ticket details for bug tracker 27", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeWorkTask
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 28
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 28", 
        //                             Description = "Ticket details for bug tracker 28", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusResolved, 
        //                             TicketTypeId = typeWorkTask
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 29
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 29", 
        //                             Description = "Ticket details for bug tracker 29", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusTest,
        //                              TicketTypeId = typeWorkTask
        //                              },
        //                         #endregion
        //                         
        //                         #region Ticket 30
        //                         new Ticket() {
        //                             Title = "Bug Tracker Ticket 30", 
        //                             Description = "Ticket details for bug tracker 30", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = bugtrackerId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeNewDev},
        //                             
        //                         #endregion
        //                         
        //                         #endregion
        //                         
        //                         #region Movie Tickets
        //                         
        //                         #region Ticket 1
        //                         new Ticket() {
        //                         Title = "Movie Ticket 1", 
        //                         Description = "Ticket details for movie ticket 1", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityLow, 
        //                         TicketStatusId = statusNew, 
        //                         TicketTypeId = typeDefect
        //                         
        //                         },
        //
        //                         #endregion
        //                         
        //                         #region Ticket 2
        //                         new Ticket() {
        //                         Title = "Movie Ticket 2", 
        //                         Description = "Ticket details for movie ticket 2", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityMedium, 
        //                         TicketStatusId = statusDev, 
        //                         TicketTypeId = typeEnhancement
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 3
        //                         new Ticket() {
        //                         Title = "Movie Ticket 3", 
        //                         Description = "Ticket details for movie ticket 3", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityHigh,
        //                         TicketStatusId = statusNew, 
        //                         TicketTypeId = typeChangeRequest
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 4
        //                         new Ticket() {
        //                         Title = "Movie Ticket 4", 
        //                         Description = "Ticket details for movie ticket 4", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityUrgent, 
        //                         TicketStatusId = statusNew, 
        //                         TicketTypeId = typeWorkTask
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 5
        //                         new Ticket() {
        //                         Title = "Movie Ticket 5", 
        //                         Description = "Ticket details for movie ticket 5", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityLow, 
        //                         TicketStatusId = statusResolved,  
        //                         TicketTypeId = typeDefect
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 6
        //                         new Ticket() {
        //                         Title = "Movie Ticket 6", 
        //                         Description = "Ticket details for movie ticket 6", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityMedium, 
        //                         TicketStatusId = statusNew,  
        //                         TicketTypeId = typeEnhancement
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 7
        //                         new Ticket() {
        //                         Title = "Movie Ticket 7", 
        //                         Description = "Ticket details for movie ticket 7", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityHigh,
        //                         TicketStatusId = statusResolved, 
        //                         TicketTypeId = typeChangeRequest
        //                          
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 8
        //                         new Ticket() {
        //                         Title = "Movie Ticket 8", 
        //                         Description = "Ticket details for movie ticket 8", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityUrgent, 
        //                         TicketStatusId = statusDev,  
        //                         TicketTypeId = typeWorkTask
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 9
        //                         new Ticket() {
        //                         Title = "Movie Ticket 9", 
        //                         Description = "Ticket details for movie ticket 9", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityLow, 
        //                         TicketStatusId = statusNew,  
        //                         TicketTypeId = typeDefect
        //                         
        //                         },
        //
        //                         #endregion
        //                         
        //                         #region Ticket 10
        //                         new Ticket() {
        //                         Title = "Movie Ticket 10", 
        //                         Description = "Ticket details for movie ticket 10", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityMedium, 
        //                         TicketStatusId = statusTest, 
        //                         TicketTypeId = typeEnhancement
        //                         
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 11
        //                         new Ticket() {
        //                         Title = "Movie Ticket 11", 
        //                         Description = "Ticket details for movie ticket 11", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityHigh, 
        //                         TicketStatusId = statusDev,  
        //                         TicketTypeId = typeChangeRequest
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 12
        //                         new Ticket() {
        //                         Title = "Movie Ticket 12", 
        //                         Description = "Ticket details for movie ticket 12", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityUrgent, 
        //                         TicketStatusId = statusTest,  
        //                         TicketTypeId = typeChangeRequest
        //                         
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 13
        //                         new Ticket() {
        //                         Title = "Movie Ticket 13", 
        //                         Description = "Ticket details for movie ticket 13", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityLow, 
        //                         TicketStatusId = statusResolved, 
        //                         TicketTypeId = typeDefect
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 14
        //                         new Ticket() {
        //                         Title = "Movie Ticket 14", 
        //                         Description = "Ticket details for movie ticket 14", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityMedium, 
        //                         TicketStatusId = statusDev,  
        //                         TicketTypeId = typeEnhancement
        //                         
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 15
        //                         new Ticket() {
        //                         Title = "Movie Ticket 15", 
        //                         Description = "Ticket details for movie ticket 15", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityHigh, 
        //                         TicketStatusId = statusNew,  
        //                         TicketTypeId = typeChangeRequest
        //                         
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 16
        //                         new Ticket() {
        //                         Title = "Movie Ticket 16", 
        //                         Description = "Ticket details for movie ticket 16", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityUrgent, 
        //                         TicketStatusId = statusResolved, 
        //                         TicketTypeId = typeWorkTask
        //                         
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 17
        //                         new Ticket() {
        //                         Title = "Movie Ticket 17", 
        //                         Description = "Ticket details for movie ticket 17", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityHigh, 
        //                         TicketStatusId = statusDev,  
        //                         TicketTypeId = typeNewDev
        //                         
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 18
        //                         new Ticket() {
        //                         Title = "Movie Ticket 18", 
        //                         Description = "Ticket details for movie ticket 18", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityMedium, 
        //                         TicketStatusId = statusDev,  
        //                         TicketTypeId = typeEnhancement
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 19
        //                         new Ticket() {
        //                         Title = "Movie Ticket 19", 
        //                         Description = "Ticket details for movie ticket 19", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityHigh, 
        //                         TicketStatusId = statusNew,  
        //                         TicketTypeId = typeChangeRequest
        //                         
        //                         },
        //                         #endregion
        //                         
        //                         #region Ticket 20
        //                         new Ticket() {
        //                         Title = "Movie Ticket 20", 
        //                         Description = "Ticket details for movie ticket 20", 
        //                         Created = DateTimeOffset.Now, 
        //                         ProjectId = movieId, 
        //                         TicketPriorityId = priorityUrgent, 
        //                         TicketStatusId = statusNew, 
        //                         TicketTypeId = typeNewDev
        //                         },
        //                         #endregion
        //                         
        //                         #endregion
        //                         
        //                         #region Address Book Tickets
        //                         
        //                         #region Ticket 1
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 1", 
        //                             Description = "Ticket details for Address Book ticket 1", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityLow, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeDefect
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 2
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 2", 
        //                             Description = "Ticket details for Address Book ticket 2", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityMedium, 
        //                             TicketStatusId = statusResolved, 
        //                             TicketTypeId = typeEnhancement
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 3
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 3", 
        //                             Description = "Ticket details for Address Book ticket 3", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusTest, 
        //                             TicketTypeId = typeChangeRequest
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 4
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 4", 
        //                             Description = "Ticket details for Address Book ticket 4", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityUrgent, 
        //                             TicketStatusId = statusTest, 
        //                             TicketTypeId = typeNewDev
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 5
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 5", 
        //                             Description = "Ticket details for Address Book ticket 5", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityLow, 
        //                             TicketStatusId = statusResolved,  
        //                             TicketTypeId = typeDefect
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 6
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 6", 
        //                             Description = "Ticket details for Address Book ticket 6", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityMedium, 
        //                             TicketStatusId = statusNew,  
        //                             TicketTypeId = typeEnhancement
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 7
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 7", 
        //                             Description = "Ticket details for Address Book ticket 7", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeChangeRequest
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 8
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 8", 
        //                             Description = "Ticket details for Address Book ticket 8", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityUrgent, 
        //                             TicketStatusId = statusDev,  
        //                             TicketTypeId = typeNewDev
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 9
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 9", 
        //                             Description = "Ticket details for Address Book ticket 9", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityLow, 
        //                             TicketStatusId = statusNew,  
        //                             TicketTypeId = typeDefect
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 10
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 10", 
        //                             Description = "Ticket details for Address Book ticket 10", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityMedium, 
        //                             TicketStatusId = statusResolved, 
        //                             TicketTypeId = typeEnhancement
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 11
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 11", 
        //                             Description = "Ticket details for Address Book ticket 11", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusDev,  
        //                             TicketTypeId = typeChangeRequest
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 12
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 12", 
        //                             Description = "Ticket details for Address Book ticket 12", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityUrgent, 
        //                             TicketStatusId = statusTest,  
        //                             TicketTypeId = typeNewDev
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 13
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 13", 
        //                             Description = "Ticket details for Address Book ticket 13", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityLow, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeDefect
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 14
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 14", 
        //                             Description = "Ticket details for Address Book ticket 14", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityMedium, 
        //                             TicketStatusId = statusDev,  
        //                             TicketTypeId = typeEnhancement
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 15
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 15", 
        //                             Description = "Ticket details for Address Book ticket 15", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew,  
        //                             TicketTypeId = typeChangeRequest
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 16
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 16", 
        //                             Description = "Ticket details for Address Book ticket 16", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityUrgent, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeNewDev
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 17
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 17", 
        //                             Description = "Ticket details for Address Book ticket 17", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusResolved,  
        //                             TicketTypeId = typeNewDev
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 18
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 18", 
        //                             Description = "Ticket details for Address Book ticket 18", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityMedium, 
        //                             TicketStatusId = statusDev,  
        //                             TicketTypeId = typeEnhancement
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 19
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 19", 
        //                             Description = "Ticket details for Address Book ticket 19", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityHigh, 
        //                             TicketStatusId = statusNew,  
        //                             TicketTypeId = typeChangeRequest
        //                             },
        //                         #endregion
        //                         
        //                         #region Ticket 20
        //                         new Ticket() {
        //                             Title = "Address Book Ticket 20", 
        //                             Description = "Ticket details for Address Book ticket 20", 
        //                             Created = DateTimeOffset.Now, 
        //                             ProjectId = addressBookId, 
        //                             TicketPriorityId = priorityUrgent, 
        //                             TicketStatusId = statusNew, 
        //                             TicketTypeId = typeNewDev
        //                             },
        //                         #endregion
        //                         
        //                         #endregion
        //         };
        //
        //
        //         var dbTickets = context.Tickets.Select(c => c.Title).ToList();
        //         await context.Tickets.AddRangeAsync(tickets.Where(c => !dbTickets.Contains(c.Title)));
        //         await context.SaveChangesAsync();
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Tickets.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        // }
}