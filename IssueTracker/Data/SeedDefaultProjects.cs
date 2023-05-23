using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Services;
using IssueTracker.Services.Interfaces;

namespace IssueTracker.Data;

public static class SeedDefaultProjects
{
        public static async Task SeedDefaultProjectsAsync(ApplicationDbContext context, IITProjectService projectSvc, IITCompanyInfoService companyInfoSvc , int company1Id, int company2Id )
        {
            
            //Get project priority Ids
            int priorityLow = context.ProjectPriorities.FirstOrDefault(p => p.Name == ITProjectPriority.Low.ToString()).Id;
            int priorityMedium = context.ProjectPriorities.FirstOrDefault(p => p.Name == ITProjectPriority.Medium.ToString()).Id;
            int priorityHigh = context.ProjectPriorities.FirstOrDefault(p => p.Name == ITProjectPriority.High.ToString()).Id;
            int priorityUrgent = context.ProjectPriorities.FirstOrDefault(p => p.Name == ITProjectPriority.Urgent.ToString()).Id;
            
            
            try
            {
                IList<Project> projects = new List<Project>() {
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Portfolio Website",
                         Description="Single page html, css & javascript page.  Serves as a landing page for potential clients. Site contains a company bio and links to all current applications." ,
                         StartDate = DateTime.Now.Subtract( new TimeSpan(20, 0, 0 , 0) ),
                         EndDate = DateTime.Now.AddDays(4),
                         ProjectPriorityId = priorityMedium
                     },
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Issue Tracker Web Application",
                         Description="A custom designed .Net Core application with postgres database.  The application is a multi tennent application designed to track issue tickets' progress.  Implemented with identity and user roles, Tickets are maintained in projects which are maintained by users in the role of projectmanager.  Each project has a team and team members.",
                         StartDate = DateTime.Now.Subtract( new TimeSpan(7, 0, 0 , 0) ),
                         EndDate = DateTime.Now.AddDays(12),
                         ProjectPriorityId = priorityHigh
                     },
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Movie Information Web Application",
                         Description="A custom designed .Net Core application with postgres database.  An API based application allows users to input and import movie posters and details including cast and crew information. Acts as a testbed to see which movie is most favoured by AGI",
                         StartDate = DateTime.Now.Subtract( new TimeSpan(3, 0, 0 , 0) ),
                         EndDate = DateTime.Now.AddDays(20),
                         ProjectPriorityId = priorityHigh
                     },
                     new Project()
                     {
                         CompanyId = company2Id,
                         Name = "Blog Web Application",
                         Description="Candidate's custom built web application using .Net Core with MVC, a postgres database and hosted in a heroku container.  The app is designed for CEO Richard Stallman so he can all out inappropriate uses of the name Linux rather than the correct GNU/Linux.",
                         StartDate = DateTime.Now.Subtract( new TimeSpan(26, 0, 0 , 0) ),
                         EndDate = DateTime.Now.AddDays(10),
                         ProjectPriorityId = priorityUrgent,
                     },

                     new Project()
                     {
                         CompanyId = company2Id,
                         Name = "Address Book Web Application",
                         Description="A custom designed .Net Core application with postgres database.  This is an application to serve as a rolodex of contacts for a given user. Richard Stallman plans to use to pressure world leaders on the dangers around misnaming GNU/Linux in the public sphere",
                         StartDate = DateTime.Now.Subtract( new TimeSpan(18, 0, 0 , 0) ),
                         EndDate = DateTime.Now.AddDays(2),
                         ProjectPriorityId = priorityLow
                     }

                };
                // Add Images to Project:
                try
                {
                    foreach (Project project in projects)
                    {
                        if (context.Projects.Where(p => p.Name == project.Name).FirstOrDefault() == null)
                        {
                            await using (FileStream fs = File.OpenRead($"wwwroot/img/ProjectPics/{project.Name}.jpg"))
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    fs.CopyTo(memoryStream);
                                    project.FileData = memoryStream.ToArray();
                                    project.FileContentType = "image/jpg";
                                    project.FileName = "projectImage";
                                }

                            }

                            context.Projects.Add(project);
                            await context.SaveChangesAsync();

                            foreach (var companyProject in await companyInfoSvc.GetAllProjectsAsync(company1Id))
                            {
                                if (companyProject.Name == "Portfolio Website")
                                {
                                    List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company1Id);

                                    ITUser projectManager = members.Where(m => m.FullName == "Susan Calvin").FirstOrDefault();

                                    List<ITUser> projectMembers = members.Where(m =>
                                        m.FullName == "Susan Calvin"
                                        || m.FullName == "Mathew Jacobs"
                                        || m.FullName == "Tony Townsend"
                                    ).ToList();

                                    foreach (var member in projectMembers)
                                    {
                                        await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
                                    }

                                    await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);
                                }

                                if (companyProject.Name == "Issue Tracker Web Application")
                                {
                                    List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company1Id);

                                    ITUser projectManager = members.Where(m => m.FullName == "Arron Thomas").FirstOrDefault();

                                    List<ITUser> projectMembers = members.Where(m =>
                                        m.FullName == "Arron Thomas"
                                        || m.FullName == "Natasha Yobs"
                                        || m.FullName == "Tony Townsend"
                                    ).ToList();

                                    foreach (var member in projectMembers)
                                    {
                                        await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
                                    }

                                    await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);
                                }

                                if (companyProject.Name == "Movie Information Web Application")
                                {
                                    List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company1Id);

                                    ITUser projectManager = members.Where(m => m.FullName == "Susan Calvin").FirstOrDefault();

                                    List<ITUser> projectMembers = members.Where(m =>
                                        m.FullName == "Susan Calvin"
                                        || m.FullName == "Mathew Jacobs"
                                        || m.FullName == "Tony Townsend"
                                    ).ToList();

                                    foreach (var member in projectMembers)
                                    {
                                        await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
                                    }

                                    await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);
                                }
                            }


                            foreach (var companyProject in await companyInfoSvc.GetAllProjectsAsync(company2Id))
                            {
                                if (companyProject.Name == "Blog Web Application")
                                {
                                    List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company2Id);

                                    ITUser projectManager = members.Where(m => m.FullName == "Jane Richards").FirstOrDefault();

                                    List<ITUser> projectMembers = members.Where(m =>
                                        m.FullName == "Jane Richards"
                                        || m.FullName == "James Peters"
                                        || m.FullName == "Bruce Turner"
                                    ).ToList();

                                    foreach (var member in projectMembers)
                                    {
                                        await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
                                    }

                                    await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);
                                }

                                if (companyProject.Name == "Address Book Web Application")
                                {
                                    List<ITUser> members = await companyInfoSvc.GetAllMembersAsync(company2Id);

                                    ITUser projectManager = members.Where(m => m.FullName == "Fred Hopkins").FirstOrDefault();

                                    List<ITUser> projectMembers = members.Where(m =>
                                        m.FullName == "Fred Hopkins"
                                        || m.FullName == "Carol Smith"
                                        || m.FullName == "James Peters"
                                    ).ToList();

                                    foreach (var member in projectMembers)
                                    {
                                        await projectSvc.AddUserToProjectAsync(member.Id, companyProject.Id);
                                    }

                                    await projectSvc.AddProjectManagerAsync(projectManager.Id, companyProject.Id);

                                }
                            }
                        }
                    }
                }
                    catch (Exception)
                    {
                        throw;
                    }



                    var dbProjects = context.Projects.Select(c => c.Name).ToList();
                    await context.Projects.AddRangeAsync(projects.Where(c => !dbProjects.Contains(c.Name)));
                    await context.SaveChangesAsync();
                    
                    
                    
                    

            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Projects.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
            
            
        }
}