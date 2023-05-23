using IssueTracker.Models;

namespace IssueTracker.Data;

public static class SeedDefaultCompanies
{
    public static async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
    {
        try
        {
            IList<Company> defaultcompanies = new List<Company>() {
                new Company() { Name = "Asimov Intelligence Systems", Description="The three laws of robotics. Now with Zeroth Law!" },
                new Company() { Name = "GNU/Corporation", Description="Technical means to a social end" }
            };

            var dbCompanies = context.Companies.Select(c => c.Name).ToList();
            await context.Companies.AddRangeAsync(defaultcompanies.Where(c => !dbCompanies.Contains(c.Name)));
            await context.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("*************  ERROR  *************");
            Console.WriteLine("Error Seeding Companies.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("***********************************");
            throw;
        }
    }
}