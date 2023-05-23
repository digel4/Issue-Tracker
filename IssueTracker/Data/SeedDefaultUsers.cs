using IssueTracker.Models;
using IssueTracker.Models.Enums;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Data;

public static class SeedDefaultUsers
{
        public static async Task SeedDefaultUsersAsync(UserManager<ITUser> userManager, IITCompanyInfoService companyInfoSvc,  int company1Id, int company2Id)
        {
            //Seed Default Admin User
            var defaultUser = new ITUser
            {
                UserName = "IsaacAsimov@AsimovIntelligenceSystems.com",
                Email = "IsaacAsimov@AsimovIntelligenceSystems.com",
                FirstName = "Isaac",
                LastName = "Asimov",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Admin User
            defaultUser = new ITUser
            {
                UserName = "RichardStallman@GNUCorporation.com",
                Email = "RichardStallman@GNUCorporation.com",
                FirstName = "Richard",
                LastName = "Stallman",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager1 User
            defaultUser = new ITUser
            {
                UserName = "SusanCalvin@AsimovIntelligenceSystems.com",
                Email = "SusanCalvin@AsimovIntelligenceSystems.com",
                FirstName = "Susan",
                LastName = "Calvin",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");

                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default ProjectManager1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager2 User
            defaultUser = new ITUser
            {
                UserName = "JaneRichards@GNUCorporation.com",
                Email = "JaneRichards@GNUCorporation.com",
                FirstName = "Jane",
                LastName = "Richards",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default ProjectManager2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            
                        //Seed Default ProjectManager3 User
            defaultUser = new ITUser
            {
                UserName = "ArronThomas@AsimovIntelligenceSystems.com",
                Email = "ArronThomas@AsimovIntelligenceSystems.com",
                FirstName = "Arron",
                LastName = "Thomas",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");

                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default ProjectManager1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default ProjectManager4 User
            defaultUser = new ITUser
            {
                UserName = "FredHopkins@GNUCorporation.com",
                Email = "FredHopkins@GNUCorporation.com",
                FirstName = "Fred",
                LastName = "Hopkins",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default ProjectManager2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer1 User
            defaultUser = new ITUser
            {
                UserName = "MathewJacobs@AsimovIntelligenceSystems.com",
                Email = "MathewJacobs@AsimovIntelligenceSystems.com",
                FirstName = "Mathew",
                LastName = "Jacobs",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer2 User
            defaultUser = new ITUser
            {
                UserName = "JamesPeters@GNUCorporation.com",
                Email = "JamesPeters@GNUCorporation.com",
                FirstName = "James",
                LastName = "Peters",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer3 User
            defaultUser = new ITUser
            {
                UserName = "NatashaYobs@AsimovIntelligenceSystems.com",
                Email = "NatashaYobs@AsimovIntelligenceSystems.com",
                FirstName = "Natasha",
                LastName = "Yobs",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer3 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer4 User
            defaultUser = new ITUser
            {
                UserName = "CarolSmith@GNUCorporation.com",
                Email = "CarolSmith@GNUCorporation.com",
                FirstName = "Carol",
                LastName = "Smith",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer4 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Developer5 User
            defaultUser = new ITUser
            {
                UserName = "TonyTownsend@AsimovIntelligenceSystems.com",
                Email = "TonyTownsend@AsimovIntelligenceSystems.com",
                FirstName = "Tony",
                LastName = "Townsend",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer5 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Developer6 User
            defaultUser = new ITUser
            {
                UserName = "BruceTurner@GNUCorporation.com",
                Email = "BruceTurner@GNUCorporation.com",
                FirstName = "Bruce",
                LastName = "Turner",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Developer5 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Submitter1 User
            defaultUser = new ITUser
            {
                UserName = "ScottApple@AsimovIntelligenceSystems.com",
                Email = "ScottApple@AsimovIntelligenceSystems.com",
                FirstName = "Scott",
                LastName = "Apple",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Submitter1 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }


            //Seed Default Submitter2 User
            defaultUser = new ITUser
            {
                UserName = "SueLincoln@GNUCorporation.com",
                Email = "SueLincoln@GNUCorporation.com",
                FirstName = "Sue",
                LastName = "Lincoln",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                await using (FileStream fs = File.OpenRead($"wwwroot/img/ProfilePics/{defaultUser.FullName}.jpg"))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fs.CopyTo(memoryStream);
                        defaultUser.AvatarFileData = memoryStream.ToArray();
                        defaultUser.AvatarContentType = "image/jpg";
                        defaultUser.AvatarFileName = "profileImage";
                    }

                }
                
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Submitter2 User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
}