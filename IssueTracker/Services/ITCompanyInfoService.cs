using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services;

// Deriving or inheriting a child class from a parent class. 
public class ITCompanyInfoService : IITCompanyInfoService
{
    private readonly ApplicationDbContext _context;
    // Constructor
    public ITCompanyInfoService(
        ApplicationDbContext context
    )
    {
        // Dependency Injection /  service layer
        _context = context;
    }

    public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
    {
        Company result = new();

        if (companyId != null)
        {
            result = await _context.Companies
                .Include(c => c.Members)
                .Include(c => c.Projects)
                .Include(c => c.Invites)
                .FirstOrDefaultAsync(c => c.Id == companyId);
        }
        
        return result;
    }

    public async Task<List<ITUser>> GetAllMembersAsync(int companyId)
    {
        // This is the same as writing List<ITUser> result = new List<ITUser>();
        List<ITUser> result = new();

        result = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

        return result;
    }

    public async Task<List<Project>> GetAllProjectsAsync(int companyId)
    {
        // This is the same as writing List<Project> result = new List<Project>();
        List<Project> result = new();
        // We need to make sure we get all the other tables associated with projects like project priority. We do this with .Include. This is called eager loading information. We do not get the virtual props by default only the local ones.
        result = await _context.Projects
            .Where(p => p.CompanyId == companyId)
            .Include(p => p.Members)
            .Include(p => p.Tickets)
                //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.Comments)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.Attachments)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.History)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.Notifications)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.DeveloperUser)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.OwnerUser)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.TicketStatus)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.TicketPriority)
            .Include(p => p.Tickets)
            //.Then is chained to the previous include. It's operating on Tickets
                .ThenInclude(t => t.TicketType)
            .Include(p => p.ProjectPriority)
            .ToListAsync();

        return result;
    }

    public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
    {
        // This is the same as writing List<Ticket> result = new List<Ticket>();
        List<Ticket> result = new();
        List<Project> projects = new();

        projects = await GetAllProjectsAsync(companyId);

        result = projects.SelectMany(p => p.Tickets).ToList();

        return result;
    }
}