using IssueTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Data;

// Telling the application to use ITUser as the class as the type for IdentityDbContext
public class ApplicationDbContext : IdentityDbContext<ITUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}